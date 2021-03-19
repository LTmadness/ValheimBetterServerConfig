using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ValheimBetterServerConfig
{
    class Helper
    {
        private static readonly string colorRegex = "<color=.*?>|<\\/color>";

        public bool isPasswordValid(String password, World world, string serverName)
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

        public ConfigEntry<string> getValidServerName(ConfigEntry<string> serverName)
        {
            if (serverName.Value.IsNullOrWhiteSpace())
            {
                serverName.Value = "Server Name";
            }
            return serverName;
        }

        public bool hasColor(string text)
        {
            return Regex.IsMatch(text, colorRegex);
        }

        public string setColor(string text, string color)
        {
            if (color.ToUpper().Contains("RANDOM"))
            {
                return $"<color={randomColor(new Random())}>{text}</color>";
            }

            return $"<color={color.ToUpper()}>{text}</color>";
        }

        public string randomColor(Random random)
        {
            return rainbowColors[random.Next(0, rainbowColors.Count - 1)];
        }

        private List<string> rainbowColors = new List<string> { "Red", "Orange", "Yellow", "Green", "Blue", "Indigo", "Violet" };
    }
}
