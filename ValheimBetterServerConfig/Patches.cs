using BepInEx;
using HarmonyLib;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using ValheimBetterServerConfig.Logger;
using static ZRoutedRpc;

namespace ValheimBetterServerConfig
{
    class Patches
    {
        public static ConfigTool config;

        private static readonly string[] saveTypes = { ".db", ".fwl" };

        private static readonly int SayHashCode = "Say".GetStableHashCode();

        [HarmonyPatch(typeof(FejdStartup), "ParseServerArguments")]
        [HarmonyPrefix]
        public static bool ParseServerArguments_modded(FejdStartup __instance, ref bool __result)
        {
            __instance.m_minimumPasswordLength = -1;

            string location = config.Location;

            if (!location.IsNullOrWhiteSpace())
            {
                Utils.SetSaveDataPath(location);
            }

            World createWorld = World.GetCreateWorld(config.WorldName);

            string serverName = config.ServerName;
            string password = config.Password;
            if (!Helper.IsPasswordValid(password, createWorld, serverName))
            {
                ZLog.LogError("Error bad password because its displayd in server/map name or seed");
                Application.Quit();

                __result = false;
                return false;
            }

            bool publiclyVisable = config.Visable;

            ZNet.SetServer(true, true, publiclyVisable, serverName, password, createWorld);
            ZNet.ResetServerHost();
            ZSteamSocket.SetDataPort(config.ServerPort);
            SteamManager.SetServerPort(config.ServerPort);

            __result = true;
            return false;
        }

        [HarmonyPatch(typeof(FejdStartup), "IsPublicPasswordValid")]
        [HarmonyPrefix]
        public static bool IsPublicPasswordValid_modded(string password, World world, ref bool __result)
        {

            __result = Helper.IsPasswordValid(password, world, config.ServerName);
            return false;
        }

        [HarmonyPatch(typeof(ZSteamMatchmaking), "RegisterServer")]
        [HarmonyPrefix]
        public static bool RegisterServer_modded(/*string name,*/ bool password, string version, bool publicServer,/* string worldName,*/
            ZSteamMatchmaking __instance)
        {
            ValheimBetterServerConfig.gameVersion = version;
            __instance.UnregisterServer();
            SteamGameServer.SetServerName(config.ServerName);
            SteamGameServer.SetMapName(config.SteamMapName);
            SteamGameServer.SetPasswordProtected(password);
            SteamGameServer.SetGameTags(version);
            SteamGameServer.EnableHeartbeats(publicServer);
            SteamGameServer.SetMaxPlayerCount(config.Size);
            SteamGameServer.SetGameDescription(config.GameDescription);
            AccessTools.Field(typeof(ZSteamMatchmaking), "m_registerServerName").SetValue(__instance, config.ServerName);
            AccessTools.Field(typeof(ZSteamMatchmaking), "m_registerPassword").SetValue(__instance, password);
            AccessTools.Field(typeof(ZSteamMatchmaking), "m_registerVerson").SetValue(__instance, version);
            Console.Utils.Print("Registering lobby (modded)", LoggerType.Patch);
            return false;
        }

        [HarmonyPatch(typeof(ZNet), "SaveWorld")]
        [HarmonyPrefix]
        public static void SaveExtraBackups(/*bool sync*/)
        {
            if (config.AnnounceSave)
            {
                Console.Utils.Announce($"Server is being saved: {DateTime.Now}");
            }

            int numberOfBackups = config.NumberOfBackups * saveTypes.Count();
            if (numberOfBackups > 0)
            {
                string timeNow = (DateTime.Now.ToShortDateString().Replace("/", "-") + "-" + DateTime.Now.ToShortTimeString().Replace(":", "-")).Replace(" ", "");
                string worldName = config.WorldName;
                string worldLocation = Utils.GetSaveDataPath() + "/worlds";
                string backupDirectory = worldLocation + "/" + worldName;
                // if doesn't exist create new backup store location
                Directory.CreateDirectory(backupDirectory);

                try
                {
                    foreach (string type in saveTypes)
                    {
                        string worldFile = (worldName + type).Replace(" ", "");
                        string worldBackup = (timeNow + worldName + type + ".old").Replace(" ", "");
                        string sourceFile = Path.Combine(worldLocation, worldFile);
                        string destFile = Path.Combine(backupDirectory, worldBackup);
                        File.Copy(sourceFile, destFile, true);
                    }
                }
                catch
                {
                    Console.Utils.Print("Nothing to back up yet", LoggerType.Patch, LoggerLevel.Error);
                    return;
                }

                List<FileInfo> files = new DirectoryInfo(backupDirectory).EnumerateFiles()
                                                                         .OrderByDescending(f => f.CreationTime)
                                                                         .Skip(numberOfBackups)
                                                                         .ToList();
                files.ForEach(f => f.Delete());
            }
        }

        [HarmonyPatch(typeof(DungeonDB), "Start")]
        [HarmonyPostfix]
        public static void Start_DungeonDB()
        {
            ValheimBetterServerConfig.serverInisialised = true; // just to make sure that server is fully running before accepting console commands
            config.PostInisilisation();
        }

        [HarmonyPatch(typeof(Chat), "OnNewChatMessage")]
        [HarmonyPostfix]
        public static void OnNewChatMessage(/*GameObject go, long senderID, Vector3 pos, */Talker.Type type, string user, string text)
        {
            if (config.ShowChatYell && !user.Equals(config.Username))
            {
                if (type == Talker.Type.Shout)
                {
                    Console.Utils.Print($"{user} yelled {text}", LoggerType.Chat);
                }
            }
        }

        [HarmonyPatch(typeof(ZRoutedRpc), "HandleRoutedRPC")]
        [HarmonyPrefix]
        public static void HandleRoutedRPC(RoutedRPCData data)
        {
            if (data?.m_methodHash == SayHashCode)
            {
                try
                {
                    ZNetPeer peer = ZNet.instance.GetPeer(data.m_senderPeerID);
                    ZPackage package = new ZPackage(data.m_parameters.GetArray());
                    int messageType = package.ReadInt();
                    string userName = package.ReadString();
                    string message = package.ReadString();
                    if (!message.IsNullOrWhiteSpace())
                    {
                        if (messageType.Equals((int)Talker.Type.Normal))
                        {
                            if (config.ShowChat)
                            {
                                Console.Utils.Print($"{userName} said: {message}", LoggerType.Chat);
                            }
                            if (message.StartsWith("/"))
                            {
                                string potentialUser = ((ZRpc)AccessTools.Field(typeof(ZNetPeer), "m_rpc").GetValue(peer)).GetSocket().GetHostName();
                                bool admin = ((SyncedList)AccessTools.Field(typeof(ZNet), "m_adminList").GetValue(ZNet.instance)).Contains(potentialUser);
                                string temp = message.Trim(new char[] { '/', ' ' }).ToLower();
                                if (admin)
                                {
                                    Runner.Instance.RunCommand(temp, false);
                                }
                                bool mod = config.modList.Contains(potentialUser);
                                if(mod)
                                {
                                    Runner.Instance.RunCommand(temp, true);
                                }
                            }
                        }
                        else if (messageType.Equals((int)Talker.Type.Whisper))
                        {
                            if (config.ShowChat)
                            {
                                Console.Utils.Print($"{userName} whispered: {message}", LoggerType.Chat);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Utils.Print("ZRoutedRPC Patch ERROR: " + ex, LoggerType.Patch, LoggerLevel.Error);
                }
            }
        }
    }
}
