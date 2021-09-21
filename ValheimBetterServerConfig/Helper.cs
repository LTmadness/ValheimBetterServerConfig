using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ValheimBetterServerConfig
{
    class Helper
    {
        private static readonly string colorRegex = "<color=.*?>|<\\/color>";

        public static bool IsPasswordValid(String password, World world, string serverName)
        {
            if (!password.Equals(""))
            {
                return !(world.m_name.Contains(password) || world.m_seedName.Contains(password) || serverName.Contains(password));
            }
            else
            {
                return true;
            }
        }

        public static ZNetPeer GetPeerByPlayerName(string name)
        {
            foreach (ZNetPeer peer in (List<ZNetPeer>) AccessTools.Field(typeof(ZNet), "m_peers").GetValue(ZNet.instance))
            {
                if (peer.IsReady() && peer.m_playerName.ToLower().Equals(name.ToLower()))
                {
                    return peer;
                }
            }

            return null;
        }

        public static ConfigEntry<string> GetValidServerName(ConfigEntry<string> serverName)
        {
            if (serverName.Value.IsNullOrWhiteSpace())
            {
                serverName.Value = "Server Name";
            }

            return serverName;
        }

        public static bool HasColor(string text)
        {
            return Regex.IsMatch(text, colorRegex);
        }

        public static string SetColor(string text, string color)
        {
            if (color.ToUpper().Contains("RANDOM"))
            {
                return $"<color={RandomColor(new Random())}>{text}</color>";
            }

            return $"<color={color.ToUpper()}>{text}</color>";
        }

        public static string RandomColor(Random random)
        {
            return rainbowColors[random.Next(0, rainbowColors.Count - 1)];
        }

        private static readonly List<string> rainbowColors = new List<string> { "Red", "Orange", "Yellow", "Green", "Blue", "Indigo", "Violet" };
    }
}