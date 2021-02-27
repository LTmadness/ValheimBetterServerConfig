using BepInEx;
using HarmonyLib;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        public const string VERSION = "0.0.30";

        private static ValheimBetterServerConfig m_instance;

        private static readonly ConfigTool config = new ConfigTool();
        private static readonly Helper helper = new Helper();
        private static readonly ConsoleCommands console = new ConsoleCommands();

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

            Harmony harmony = new Harmony(GUID);

            MethodInfo originIsPublicPasswordValid = AccessTools.Method(typeof(FejdStartup), "IsPublicPasswordValid");
            MethodInfo patchIsPublicPasswordValid = AccessTools.Method(typeof(ValheimBetterServerConfig), "IsPublicPasswordValid_modded");
            harmony.Patch(originIsPublicPasswordValid, new HarmonyMethod(patchIsPublicPasswordValid));


            MethodInfo originParseServerArguments = AccessTools.Method(typeof(FejdStartup), "ParseServerArguments");
            MethodInfo patchParseServerArguments = AccessTools.Method(typeof(ValheimBetterServerConfig), "ParseServerArguments_modded");
            harmony.Patch(originParseServerArguments, new HarmonyMethod(patchParseServerArguments));

            //seting server private and size not supported by Valheim??????
            /*MethodInfo originAwakeZNet = AccessTools.Method(typeof(ZNet), "Awake");
            MethodInfo patchAwakeZNet = AccessTools.Method(typeof(ValheimBetterServerConfig), "Awake_modded");
            harmony.Patch(originAwakeZNet, new HarmonyMethod(patchAwakeZNet));*/

            MethodInfo originRegisterServer = AccessTools.Method(typeof(ZSteamMatchmaking), "RegisterServer");
            MethodInfo patchRegisterServer = AccessTools.Method(typeof(ValheimBetterServerConfig), "RegisterServer_modded");
            harmony.Patch(originRegisterServer, new HarmonyMethod(patchRegisterServer));

            MethodInfo originZNet = AccessTools.Method(typeof(ZNet), "Awake");
            MethodInfo patchZNet = AccessTools.Method(typeof(ValheimBetterServerConfig), "Awake_modded");
            harmony.Patch(originZNet, null, new HarmonyMethod(patchZNet));

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
            ZLog.Log("Registering lobby (modded)");
            return false;
        }
    }
}