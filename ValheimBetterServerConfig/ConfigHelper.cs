using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace ValheimBetterServerConfig
{
    class ConfigHelper
    {
        private static ConfigHelper m_instance;
        public static ConfigFile config;

        private const string DEFAULT_SETTINGS = "Default settings";
        private const string ADVANCED_SETTINGS = "Default settings";

        //Default settings
        private ConfigEntry<string> serverName;
        private ConfigEntry<int> serverPort;
        private ConfigEntry<string> worldName;
        private ConfigEntry<string> password;
        private ConfigEntry<bool> publiclyVisable;
        private ConfigEntry<string> saveLocation;

        //Advanced settings
        private ConfigEntry<int> serverSize;

        public ConfigHelper instance
        {
            get
            {
                return ConfigHelper.m_instance;
            }
        }

        public void loadConfig()
        {
            //default settings
            serverName = config.AddSetting(DEFAULT_SETTINGS, "Server name", "My Server Name", "Server name, please change");
            serverPort = config.AddSetting<int>(DEFAULT_SETTINGS, "Server Port", 2456);
            worldName = config.AddSetting(DEFAULT_SETTINGS, "World Name", "ServerWorld", "World name, please change");
            password = config.AddSetting(DEFAULT_SETTINGS, "Server password", "changeMe", "server password, please change");
            publiclyVisable = config.AddSetting(DEFAULT_SETTINGS, "Server publicly visable", false, "Change me if you want your server to apear in server list");
            saveLocation = config.AddSetting(DEFAULT_SETTINGS, "Save location", "", "Server data save location");

            //advanced settings
            serverSize = config.AddSetting(ADVANCED_SETTINGS, "Server size", 10, "Number of players allowed in the server");
            config.Save();
        }


        public string getServerName()
        {
            return serverName.Value;
        }

        public int getServerPort()
        {
            return serverPort.Value;
        }

        public string getWorldName()
        {
            return worldName.Value;
        }

        public string getPassword()
        {
            return password.Value;
        }

        public bool isVisable()
        {
            return publiclyVisable.Value;
        }

        public int getSize()
        {
            return serverSize.Value;
        }

        public string getLocation()
        {
            return saveLocation.Value;
        }

        public void setConfigFile(ConfigFile config_file)
        {
            config = config_file;
        }
    }
}
