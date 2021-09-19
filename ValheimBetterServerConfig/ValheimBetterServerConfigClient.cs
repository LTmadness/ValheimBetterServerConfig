using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine.UI;

namespace ValheimBetterServerConfig
{
    [BepInPlugin(
        ValheimBetterServerConfig.GUID,
        ValheimBetterServerConfig.NAME,
        ValheimBetterServerConfig.VERSION)]
    [BepInProcess("valheim.exe")]
    class ValheimBetterServerConfigClient : BaseUnityPlugin
    {
        public void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(ValheimBetterServerConfigClient), ValheimBetterServerConfig.GUID);
        }

        [HarmonyPatch(typeof(Chat), "InputText")]
        [HarmonyPrefix]
        public static bool InputText(Chat __instance)
        {
            string text = ((InputField) AccessTools.Field(typeof(Chat), "m_input").GetValue(__instance)).text;
            if (text.Length == 0)
                return false;
            if (!TryRunCommand(text[0] != '/' ? "say " + text : text.Substring(1), __instance))
            {
                TryRunCommand("say " + text, __instance);
            }
            return false;
        }

        public static bool TryRunCommand(string text, Terminal instance)
        {
            string[] strArray = text.Split(' ');
            Terminal.ConsoleCommand consoleCommand;
            Dictionary<string, Terminal.ConsoleCommand> commands = (Dictionary<string, Terminal.ConsoleCommand>) AccessTools.Field(typeof(Terminal), "commands").GetValue(instance);
            if (commands.TryGetValue(strArray[0].ToLower(), out consoleCommand))
            {
                if (consoleCommand.IsValid(instance, false))
                {
                    consoleCommand.RunAction(new Terminal.ConsoleEventArgs(text, instance));
                    return true;
                }
            }
            return false;
        }
    }
}
