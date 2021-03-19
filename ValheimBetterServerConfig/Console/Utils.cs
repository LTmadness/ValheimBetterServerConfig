using BepInEx;
using System;
using UnityEngine;

namespace ValheimBetterServerConfig.Console
{
    class Utils
    {
        public static string RebuildString(string[] args)
        {
            args[0] = "";
            return String.Join(" ", args).Trim();
        }

        public static void Print(string text)
        {
            System.Console.WriteLine(text);
        }

        public static bool UserSupplied(string[] args)
        {
            if (args.Length <= 1 || args[1].IsNullOrWhiteSpace())
            {
                Print("No or incorrect user supplied");
                return false;
            }
            return true;
        }

        public static void Announce(string announcement)
        {
            if (!announcement.IsNullOrWhiteSpace())
            {
                string username = Runner.Instance.config.Username;
                ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ChatMessage", new object[] { new Vector3(), 2, username, announcement });
            }
        }

        public static void ArgumentSkipped(string[] args)
        {
            string message = RebuildString(args);
            if (!message.IsNullOrWhiteSpace())
            {
                Print($"Unnececery argument skipped: {message}");
            }
        }
    }
}
