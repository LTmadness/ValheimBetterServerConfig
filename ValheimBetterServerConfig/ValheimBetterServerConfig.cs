using BepInEx;
using HarmonyLib;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ValheimBetterServerConfig
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInProcess("valheim_server.exe")]
    public class ValheimBetterServerConfig : BaseUnityPlugin
    {
        public const string GUID = "org.ltmadness.valheim.betterserverconfig";
        public const string NAME = "Better Server Config";
        public const string VERSION = "0.0.70";

        private static ConfigTool config;
        private static Helper helper = new Helper();
        private static ConsoleCommands console;

        private static ZNet zNet;

        private static string[] saveTypes = { ".db", ".fwl" };

        public static bool serverInisialised = false;

        public bool runConsole = true;

        public void Awake()
        {
            config = new ConfigTool(Config);

            Harmony.CreateAndPatchAll(typeof(ValheimBetterServerConfig), GUID);

            Task.Run(async () =>
            {
                while ((zNet == null) || !serverInisialised)
                {
                    Thread.Sleep(1000);// waiting for zNets  to inisialise
                }

                console = new ConsoleCommands(zNet, config);

                while (runConsole)
                {
                    string input = System.Console.ReadLine();
                    console.runCommand(input);
                }
            });
        }

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
            if (!helper.isPasswordValid(password, createWorld, serverName))
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

            __result = helper.isPasswordValid(password, world, config.ServerName);
            return false;
        }

        [HarmonyPatch(typeof(ZNet), "Awake")]
        [HarmonyPostfix]
        public static void Awake_zNet(ZNet __instance)
        {
            zNet = __instance;
        }

        [HarmonyPatch(typeof(ZSteamMatchmaking), "RegisterServer")]
        [HarmonyPrefix]
        public static bool RegisterServer_modded(string name, bool password, string version, bool publicServer, string worldName,
            ZSteamMatchmaking __instance)
        {
            __instance.UnregisterServer();
            SteamGameServer.SetServerName(config.ServerName);
            SteamGameServer.SetMapName(config.SteamMapName);
            SteamGameServer.SetPasswordProtected(password);
            SteamGameServer.SetGameTags(version);
            SteamGameServer.EnableHeartbeats(publicServer);
            SteamGameServer.SetMaxPlayerCount(config.Size);
            SteamGameServer.SetGameDescription("Valheim");
            AccessTools.Field(typeof(ZSteamMatchmaking), "m_registerServerName").SetValue(__instance, config.ServerName);
            AccessTools.Field(typeof(ZSteamMatchmaking), "m_registerPassword").SetValue(__instance, password);
            AccessTools.Field(typeof(ZSteamMatchmaking), "m_registerVerson").SetValue(__instance, version);
            print("Registering lobby (modded)");
            return false;
        }

        [HarmonyPatch(typeof(ZNet), "SaveWorld")]
        [HarmonyPrefix]
        public static void saveExtraBackups(bool sync)
        {
            int numberOfBackups = config.NumberOfBackups * saveTypes.Count();
            if (numberOfBackups > 0) { 
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
                    print("Nothing to back up yet");
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
        public static void Start()
        {
            serverInisialised = true;
        }
    }
}