﻿using BepInEx;
using BepInEx.Configuration;

namespace ValheimBetterServerConfig
{
    class ConfigTool
    {

        private const string DEFAULT_SETTINGS = "Default settings";
        private const string ADVANCED_SETTINGS = "Default settings";

        private static ConfigTool m_instance;
        public static ConfigFile config;
        public static Helper helper = new Helper();

        //Default settings
        private string name;
        private ConfigEntry<string> serverName;
        private ConfigEntry<int> serverPort;
        private ConfigEntry<string> worldName;
        private ConfigEntry<string> password;
        private ConfigEntry<string> saveLocation;

        //Advanced settings
        private ConfigEntry<string> serverNameColor;
        //private ConfigEntry<bool> publiclyVisable;
        private ConfigEntry<int> serverSize;
        //private ConfigEntry<bool> playerVisable;
        private ConfigEntry<bool> serverNameItalic;
        private ConfigEntry<bool> serverNameBold;
        private ConfigEntry<string> steamMapName;

        public ConfigTool instance
        {
            get
            {
                return ConfigTool.m_instance;
            }
        }

        public void loadConfig()
        {
            //default settings
            serverName = helper.getValidServerName(config.Bind<string>(DEFAULT_SETTINGS, "Server Name", "My Server Name", "Server name, please change, if deleted will say \"Server Name\", if you want tou can use multiple colors example: \n " +
                                                                                                                             "<color=RED>Server</color><color=BLUE>Name</color>, or you can do same <i>italc</i> or  <b>bold</b>,\n" +
                                                                                                                             " it doesn't work in steam server browser, also you can find more color names here: https://www.w3schools.com/colors/colors_names.asp"));
            serverPort = config.Bind<int>(DEFAULT_SETTINGS, "Server Port", 2456);
            worldName = config.Bind<string>(DEFAULT_SETTINGS, "World Name", "ServerWorld", "World name, please change");
            password = config.Bind<string>(DEFAULT_SETTINGS, "Server password", "changeMe", "server password, please change");
            saveLocation = config.Bind<string>(DEFAULT_SETTINGS, "Save location", "", "Server data save location");

            //advanced settings
            //publiclyVisable = config.Bind<bool>(ADVANCED_SETTINGS, "Server publicly visable", true, "Change me if you want your server to apear in server list");
            serverSize = config.Bind<int>(ADVANCED_SETTINGS, "Server size", 10, "Number of players allowed in the server, minimum 1, only works on steam browser so far");
            //playerVisable = config.Bind<bool>(ADVANCED_SETTINGS, "Players Visable", false, "Default for new players joining the server, if set to true player by default will be visable on the map");
            serverNameColor = config.Bind<string>(ADVANCED_SETTINGS, "Server name color", "", "You can choose your server name color, doesn't work in steam server browser eg.: red");
            serverNameItalic = config.Bind<bool>(ADVANCED_SETTINGS, "Server name italic", false, "Should your server name be writen in italics, doesn't work on steam server browser");
            serverNameBold = config.Bind<bool>(ADVANCED_SETTINGS, "Server name bold", false, "Should your server name be writen in bold, doesn't work on steam server browser");
            steamMapName = config.Bind<string>(ADVANCED_SETTINGS, "Steam Map name", "", "If empty world name will be used");

            if(!serverNameColor.Value.IsNullOrWhiteSpace() && !helper.hasColor(serverName.Value))
            {
                name = helper.setColor(serverName.Value, serverNameColor.Value);
            }
            else
            {
                name = serverName.Value;
            }

            if(serverNameItalic.Value)
            {
                name = "<i>" + name + "</i>";
            }

            if(serverNameBold.Value)
            {
                name = "<b>" + name + "</b>";
            }

            config.Save();
        }


        public string getServerName()
        {
            return name;
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

        /*public bool isVisable()
        {
            return publiclyVisable.Value;
        }*/

        public int getSize()
        {
            return serverSize.Value;
        }

        public string getLocation()
        {
            return saveLocation.Value;
        }

        /*public bool getPlayerVisable()
        {
            return playerVisable.Value;
        }*/

        public string getSteamMapName()
        {
            return steamMapName.Value;
        }

        public void setConfigFile(ConfigFile config_file)
        {
            config = config_file;
        }
    }
}
