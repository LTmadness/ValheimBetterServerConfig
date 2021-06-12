using BepInEx;
using HarmonyLib;
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
        public const string VERSION = "0.1.3";

        public static string gameVersion = "NOT SET";

        public static bool serverInisialised = false;

        private static Runner console;

        public bool runConsole = true;

        private ConfigTool config;

        private Logger.Logger logger;

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
                    string input = "";
                    try
                    {
                        input = System.Console.ReadLine();
                        console.RunCommand(input, false);
                        input = "";
                    }
                    catch
                    {
                        if (!input.Trim().IsNullOrWhiteSpace())
                        {
                            Console.Utils.Print($"Please don't use {input} as its causing error," +
                                $" please report it on https://github.com/LTmadness/ValheimBetterServerConfig, please include input command used");
                        }
                    }
                }
            });
        }

        public void Awake()
        {
            config = new ConfigTool(Config);
            logger = new Logger.Logger(config.Location, config.LoggerLevel);
            Patches.config = config;

            Harmony.CreateAndPatchAll(typeof(Patches), GUID);
        }
    }
}