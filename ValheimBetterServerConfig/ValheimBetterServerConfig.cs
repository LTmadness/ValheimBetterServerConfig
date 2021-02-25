using BepInEx;
using HarmonyLib;
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

        }

        [HarmonyPatch(typeof(FejdStartup), "ParseServerArguments")]
        [HarmonyPrefix]
        public static bool ParseServerArguments_simplify(ref bool __result)
        {
            Utils.SetSaveDataPath(config.getLocation());
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
            ZNet.SetServer(true, true, config.isVisable(), serverName, password, createWorld);
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
    }
}
