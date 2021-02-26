using BepInEx;
using HarmonyLib;
using Steamworks;
using System.Reflection;
using UnityEngine;

namespace ValheimBetterServerConfig
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInProcess("valheim_server.exe")]
    public class ValheimBetterServerConfig : BaseUnityPlugin
    {
        public const string GUID = "org.ltmadness.valheim.betterserverconfig";
        public const string NAME = "Better Server Config";
        public const string VERSION = "0.0.1";

        private static ValheimBetterServerConfig m_instance;

        private static readonly ConfigHelper config = new ConfigHelper();
        private static readonly Validator validator = new Validator();

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
            MethodInfo patchIsPublicPasswordValid = AccessTools.Method(typeof(ValheimBetterServerConfig), "IsPublicPasswordValid_redirect");
            harmony.Patch(originIsPublicPasswordValid, new HarmonyMethod(patchIsPublicPasswordValid));


            MethodInfo originParseServerArguments = AccessTools.Method(typeof(FejdStartup), "ParseServerArguments");
            MethodInfo patchParseServerArguments = AccessTools.Method(typeof(ValheimBetterServerConfig), "ParseServerArguments_simplify");
            harmony.Patch(originParseServerArguments, new HarmonyMethod(patchParseServerArguments));

            //seting server private and size not supported by Valheim??????
            /*MethodInfo originAwakeZNet = AccessTools.Method(typeof(ZNet), "Awake");
            MethodInfo patchAwakeZNet = AccessTools.Method(typeof(ValheimBetterServerConfig), "Awake_setUp");
            harmony.Patch(originAwakeZNet, new HarmonyMethod(patchAwakeZNet));

            MethodInfo originRegisterServer = AccessTools.Method(typeof(ZSteamMatchmaking), "RegisterServer");
            MethodInfo patchRegisterServer = AccessTools.Method(typeof(ValheimBetterServerConfig), "RegisterServer_updated");
            harmony.Patch(originRegisterServer, new HarmonyMethod(patchRegisterServer));*/
        }

        [HarmonyPatch(typeof(FejdStartup), "ParseServerArguments")]
        [HarmonyPrefix]
        public static bool ParseServerArguments_simplify(FejdStartup __instance, ref bool __result)
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
            if (!validator.isPasswordValid(password, createWorld, serverName))
            {
                ZLog.LogError("Error bad password because its displayd in server/map name or seed");
                Application.Quit();

                __result = false;
                return false;
            }

            bool publiclyVisable = config.isVisable();

            __instance.m_publicServerToggle.isOn = publiclyVisable;

            ZNet.SetServer(true, true, publiclyVisable, serverName, password, createWorld);
            ZNet.SetServerHost("", 0);
            SteamManager.SetServerPort(config.getServerPort());

            __result = true;
            return false;
        }

        [HarmonyPatch(typeof(FejdStartup), "IsPublicPasswordValid")]
        [HarmonyPrefix]
        public static bool IsPublicPasswordValid_redirect(string password, World world, ref bool __result)
        {

            __result = validator.isPasswordValid(password, world, config.getServerName());
            return false;
        }

        /*[HarmonyPatch(typeof(ZNet), "Awake")]
        [HarmonyPrefix]
        public static void Awake_setUp(ZNet __instance)
        {
            __instance.m_serverPlayerLimit = config.getSize();
        }

        [HarmonyPatch(typeof(ZSteamMatchmaking), "RegisterServer")]
        [HarmonyPrefix]
        public static bool RegisterServer_updated(string name, bool password, string version, bool publicServer, string worldName,
            ZSteamMatchmaking __instance)
        {
            __instance.UnregisterServer();
            if (!publicServer) 
            {
                SteamGameServer.SetServerName("");
                SteamGameServer.SetMapName("");
                SteamGameServer.SetPasswordProtected(true);
                SteamGameServer.SetGameTags(null);
                AccessTools.Field(typeof(ZSteamMatchmaking), "m_registerServerName").SetValue(__instance, "");
                AccessTools.Field(typeof(ZSteamMatchmaking), "m_registerPassword").SetValue(__instance, true);
                AccessTools.Field(typeof(ZSteamMatchmaking), "m_registerVerson").SetValue(__instance, null);
            }
            else
            {
                SteamGameServer.SetServerName(name);
                SteamGameServer.SetMapName(name);
                SteamGameServer.SetPasswordProtected(password);
                SteamGameServer.SetGameTags(version);
                AccessTools.Field(typeof(ZSteamMatchmaking), "m_registerServerName").SetValue(__instance, name);
                AccessTools.Field(typeof(ZSteamMatchmaking), "m_registerPassword").SetValue(__instance, password);
                AccessTools.Field(typeof(ZSteamMatchmaking), "m_registerVerson").SetValue(__instance, version);
            }
            SteamGameServer.EnableHeartbeats(true);

            ZLog.Log("Registering lobby (advanced)");
            return false;*/
        }
    }
}
