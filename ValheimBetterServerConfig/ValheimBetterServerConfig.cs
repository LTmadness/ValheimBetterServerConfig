using BepInEx;
using HarmonyLib;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public const string VERSION = "0.0.50";

        private static ValheimBetterServerConfig m_instance;

        private static ConfigTool config = new ConfigTool();
        private static Helper helper = new Helper();
        private static ConsoleCommands console = new ConsoleCommands();

        private static string[] saveTypes = { ".db", ".fwl" } ;

        public ValheimBetterServerConfig instance
        {
            get
            {
                return ValheimBetterServerConfig.m_instance;
            }
        }

        public void Awake()
        {
            ValheimBetterServerConfig.m_instance = this;
            config.setConfigFile(Config);
            config.loadConfig();

            Harmony.CreateAndPatchAll(typeof(ValheimBetterServerConfig), GUID);

            Task.Run(async () =>
            {
                while (true)
                {
                    if (console.getZNet() != null)
                    {
                        string input = System.Console.ReadLine();
                        console.runCommand(input);
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            });
        }

        [HarmonyPatch(typeof(FejdStartup), "ParseServerArguments")]
        [HarmonyPrefix]
        public static bool ParseServerArguments_modded(FejdStartup __instance, ref bool __result)
        {
            __instance.m_minimumPasswordLength = -1;

            string location = config.getLocation();

            if (!location.IsNullOrWhiteSpace())
            {
                Utils.SetSaveDataPath(location);
            }

            World createWorld = World.GetCreateWorld(config.getWorldName());

            string serverName = config.getServerName();
            string password = config.getPassword();
            if (!helper.isPasswordValid(password, createWorld, serverName))
            {
                ZLog.LogError("Error bad password because its displayd in server/map name or seed");
                Application.Quit();

                __result = false;
                return false;
            }

            bool publiclyVisable = true; //config.isVisable(); - publicly visable doesn't really zhange anything in Valheim code as the server visability can't be changed

            __instance.m_publicServerToggle.isOn = publiclyVisable;

            ZNet.SetServer(true, true, publiclyVisable, serverName, password, createWorld);
            ZNet.SetServerHost("", 0);
            SteamManager.SetServerPort(config.getServerPort());

            __result = true;
            return false;
        }

        [HarmonyPatch(typeof(FejdStartup), "IsPublicPasswordValid")]
        [HarmonyPrefix]
        public static bool IsPublicPasswordValid_modded(string password, World world, ref bool __result)
        {

            __result = helper.isPasswordValid(password, world, config.getServerName());
            return false;
        }

        [HarmonyPatch(typeof(ZNet), "Awake")]
        [HarmonyPrefix]
        public static void Awake_modded(ZNet __instance)
        {
            console.setZNet(__instance);
        }

        [HarmonyPatch(typeof(ZSteamMatchmaking), "RegisterServer")]
        [HarmonyPrefix]
        public static bool RegisterServer_modded(string name, bool password, string version, bool publicServer, string worldName,
            ZSteamMatchmaking __instance)
        {
            __instance.UnregisterServer();
            SteamGameServer.SetServerName(config.getServerName());
            SteamGameServer.SetMapName(config.getSteamMapName());
            SteamGameServer.SetPasswordProtected(password);
            SteamGameServer.SetGameTags(version);
            AccessTools.Field(typeof(ZSteamMatchmaking), "m_registerServerName").SetValue(__instance, config.getServerName());
            AccessTools.Field(typeof(ZSteamMatchmaking), "m_registerPassword").SetValue(__instance, password);
            AccessTools.Field(typeof(ZSteamMatchmaking), "m_registerVerson").SetValue(__instance, version);
            SteamGameServer.EnableHeartbeats(true);
            SteamGameServer.SetMaxPlayerCount(config.getSize());
            SteamGameServer.SetGameDescription("Valheim");
            print("Registering lobby (modded)");
            return false;
        }

        [HarmonyPatch(typeof(ZNet), "SaveWorld")]
        [HarmonyPrefix]
        public static void saveExtraBackups(bool sync)
        {
            int numberOfBackups = config.getNumberOfBackups() * saveTypes.Count();
            if (numberOfBackups > 0) { 
                string timeNow = (DateTime.Now.ToShortDateString().Replace("/", "-") + "-" + DateTime.Now.ToShortTimeString().Replace(":", "-")).Replace(" ", "");
                string worldName = config.getWorldName();
                string worldLocation = Utils.GetSaveDataPath() + "/worlds";
                string backupDirectory = worldLocation + "/" + worldName;
                // if doesn't exist create new backup store location
                Directory.CreateDirectory(backupDirectory);

                foreach (string type in saveTypes)
                {
                    string worldFile = (worldName + type).Replace(" ", "");
                    string worldBackup = (timeNow + worldName + type + ".old").Replace(" ", "");
                    string sourceFile = Path.Combine(worldLocation, worldFile);
                    string destFile = Path.Combine(backupDirectory, worldBackup);
                    File.Copy(sourceFile, destFile, true);
                }

                List<FileInfo> files = new DirectoryInfo(backupDirectory).EnumerateFiles()
                                                 .OrderByDescending(f => f.CreationTime)
                                                 .Skip(numberOfBackups)
                                                 .ToList();
                files.ForEach(f => f.Delete());
            }
        }
    }
}