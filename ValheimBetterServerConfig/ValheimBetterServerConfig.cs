using BepInEx;
using HarmonyLib;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ValheimBetterServerConfig
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInProcess("valheim_server.exe")]
    public class ValheimBetterServerConfig : BaseUnityPlugin
    {
        public const string GUID = "org.ltmadness.valheim.betterserverconfig";
        public const string NAME = "Better Server Config";
        public const string VERSION = "0.1.1";

        public static string gameVersion = "NOT SET";

        private static Runner console;

        public static bool serverInisialised = false;

        public bool runConsole = true;

        private ConfigTool config;

        public async void Start()
        { 
            await Task.Run(() =>
            {
                while ((ZNet.instance == null) || !serverInisialised)
                {
                    Thread.Sleep(1000);// waiting for zNets  to inisialise
                }

                console = new Runner(config);

                while (runConsole)
                {
                    string input = System.Console.ReadLine();
                    console.RunCommand(input);
                }
            });
        }

        public void Awake()
        {
            config = new ConfigTool(Config);
            Patches.config = config;

            Harmony.CreateAndPatchAll(typeof(Patches), GUID);
        }
    }
}