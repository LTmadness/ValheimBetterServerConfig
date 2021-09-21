using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using ValheimBetterServerConfig.Logger;

namespace ValheimBetterServerConfig
{
    class ConfigTool
    {
        private const string GENERAL_SETTINGS = "General settings";
        private const string ADVANCED_SETTINGS = "Advanced settings";
        private const string CONSOLE_SETTINGS = "Console settings";
        private const string LOGGER_SETTINGS = "Logger settings";

        public static ConfigFile config;

        public SyncedList modList;

        //Default settings
        private ConfigEntry<string> serverName;
        private ConfigEntry<string> worldName;
        private ConfigEntry<string> password;
        private ConfigEntry<string> saveLocation;

        private ConfigEntry<bool> visable;

        private ConfigEntry<int> serverPort;

        private string name;

        //Advanced settings
        private ConfigEntry<string> serverNameColor;
        private ConfigEntry<string> steamMapName;
        private ConfigEntry<string> serverUsername;
        private ConfigEntry<string> gameDescription;

        private ConfigEntry<bool> serverNameItalic;
        private ConfigEntry<bool> serverNameBold;
        private ConfigEntry<bool> announceSave;

        private ConfigEntry<int> numberOfBackups;
        private ConfigEntry<int> serverSize;

        //Console settings
        private ConfigEntry<bool> showChatYell;
        private ConfigEntry<bool> showChatAll;

        private ConfigEntry<int> helpPageSize;

        private List<string> modCommandsList;

        //Logger settings
        private ConfigEntry<LoggerLevel> loggerLevel;

        

        public ConfigTool(ConfigFile config)
        {
            ConfigTool.config = config;
            LoadConfig();
        }

        public void LoadConfig()
        {
            //default settings
            serverName = Helper.GetValidServerName(config.Bind<string>(GENERAL_SETTINGS, "Server Name", "My Server Name", "Server name, please change, if deleted will say \"Server Name\", if you want tou can use multiple colors example: " + Environment.NewLine +
                                                                                                                             "<color=RED>Server</color><color=BLUE>Name</color>, or you can do same <i>italc</i> or  <b>bold</b>," + Environment.NewLine +
                                                                                                                             " it doesn't work in steam server browser, also you can find more color names here: https://www.w3schools.com/colors/colors_names.asp"));
            serverPort = config.Bind<int>(GENERAL_SETTINGS, "Server Port", 2456);
            worldName = config.Bind<string>(GENERAL_SETTINGS, "World Name", "ServerWorld");
            password = config.Bind<string>(GENERAL_SETTINGS, "Server password", "changeMe", "server password, please change");
            saveLocation = config.Bind<string>(GENERAL_SETTINGS, "Save location", "", "Server data save location");
            visable = config.Bind<bool>(GENERAL_SETTINGS, "Publicly visable:", true, "Will server be publicly visable in server list");

            //advanced settings
            serverSize = config.Bind<int>(ADVANCED_SETTINGS, "Server size", 10, "Number of players allowed in the server, minimum 1, only works on steam browser so far");
            serverNameColor = config.Bind<string>(ADVANCED_SETTINGS, "Server name color", "", "You can choose your server name color, doesn't work in steam server browser eg.: red");
            serverNameItalic = config.Bind<bool>(ADVANCED_SETTINGS, "Server name italic", false, "Should your server name be writen in italics, doesn't work on steam server browser");
            serverNameBold = config.Bind<bool>(ADVANCED_SETTINGS, "Server name bold", false, "Should your server name be writen in bold, doesn't work on steam server browser");
            steamMapName = config.Bind<string>(ADVANCED_SETTINGS, "Steam Map name", "", "If empty world name will be used");
            numberOfBackups = config.Bind<int>(ADVANCED_SETTINGS, "Number of backups", 5, "Number of backups you wanna keep");
            serverUsername = config.Bind<string>(ADVANCED_SETTINGS, "Server username", "Server", "Used as sender name when using say/yell commands in server console");
            announceSave = config.Bind<bool>(ADVANCED_SETTINGS, "Announce saves", false, "Should server announce in game chat when the world save is happening");
            gameDescription = config.Bind<string>(ADVANCED_SETTINGS, "Game Description", "Valheim", "Text that will be shown as Game in Steam Browser, changing this does not affect under which game server will apear");

            //Console settings
            showChatYell = config.Bind<bool>(CONSOLE_SETTINGS, "Show shout chat", true, "Show what eveyone shouts (/s) in console");
            showChatAll = config.Bind<bool>(CONSOLE_SETTINGS, "Show chat", true, "Show all chat in console, overwrites show chat shout option");
            modCommandsList = config.Bind<string>(CONSOLE_SETTINGS, "Commands Allowed for Mods", "kick,say,save,sleep" , "List of commands allowed to use by mods").Value.ToLower().Split(',').ToList();
            helpPageSize = config.Bind<int>(CONSOLE_SETTINGS, "Help Page Size", 10, "The size opf page when using help");

            //Logger settings
            loggerLevel = config.Bind<LoggerLevel>(LOGGER_SETTINGS, "Logging level", LoggerLevel.Info, "Lowest level of logs to be recored to file (Info, Debug, Error)");

            config.Save();

            if (!serverNameColor.Value.IsNullOrWhiteSpace() && !Helper.HasColor(serverName.Value))
            {
                name = Helper.SetColor(serverName.Value, serverNameColor.Value);
            }
            else
            {
                name = serverName.Value;
            }

            if (serverNameItalic.Value)
            {
                name = "<i>" + name + "</i>";
            }

            if (serverNameBold.Value)
            {
                name = "<b>" + name + "</b>";
            }
        }

        public void PostInisilisation()
        {
            modList = new SyncedList(Utils.GetSaveDataPath() + "/modlist.txt", "List moderator players ID  ONE per line");
        }

        public string ServerName { get => name; }
        public int ServerPort { get => serverPort.Value; }
        public string WorldName { get => worldName.Value; }
        public string Password { get => password.Value; }
        public int Size { get => serverSize.Value; }
        public string Location { get => saveLocation.Value; }
        public string SteamMapName { get => steamMapName.Value.IsNullOrWhiteSpace() ? worldName.Value : steamMapName.Value; }
        public int NumberOfBackups { get => numberOfBackups.Value; }
        public bool Visable { get => visable.Value; }
        public string Username { get => serverUsername.Value; }
        public string GameDescription { get => gameDescription.Value; }
        public bool AnnounceSave { get => announceSave.Value; }
        public bool ShowChatYell { get => showChatYell.Value || showChatAll.Value; }
        public bool ShowChat { get => showChatAll.Value; }
        public List<string> GetModCommands { get => modCommandsList; }
        public int HelpPageSize { get => helpPageSize.Value; }
        public LoggerLevel LoggerLevel { get => loggerLevel.Value; }

        public List<string> GetList()
        {
            return new List<string>
            {
                $"Server Name: {ServerName}",
                $"Server Port: {ServerPort}",
                $"World Name: {WorldName}",
                $"Server Password: {Password}",
                $"Is Server Visable: {Visable}",
                $"Server Size: {Size}",
                $"Server Save Location: {Location}",
                $"Steam Map Name: {SteamMapName}",
                $"Number Of Backups: {NumberOfBackups}",
                $"Server Username: {Username}",
                $"Show Shouts In Console: {ShowChatYell}",
                $"Show All Chat In Console: {ShowChat}",
                $"Announce Saves: {AnnounceSave}",
                $"Game Description: {GameDescription}",
                $"Help Page Size: {HelpPageSize}",
                $"Logger level: {LoggerLevel}"
            };
        }
    }
}