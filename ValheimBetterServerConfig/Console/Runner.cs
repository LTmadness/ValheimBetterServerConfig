using BepInEx;
using System.Collections.Generic;
using static ValheimBetterServerConfig.Console.Utils;
using static ValheimBetterServerConfig.Console.Methods;

namespace ValheimBetterServerConfig
{
    class Runner
    {
        private static Runner instance;

        public List<Command> commands = new List<Command>();

        public ConfigTool config;

        public static Runner Instance
        {
            get
            {
                return Runner.instance;
            }
        }

        public Runner(ConfigTool config)
        {
            instance = this;
            this.config = config;
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            commands.Add(new Command("help", "help - get list of all commands", Help, config.GetModCommands.Contains("help")));
            commands.Add(new Command("kick", "kick [name/steamID] - kick user", Kick, config.GetModCommands.Contains("kick")));
            commands.Add(new Command("ban", "ban [name/ip/steamID] - ban user", Ban, config.GetModCommands.Contains("ban")));
            commands.Add(new Command("unban", "unban [ip/steamID] - unban user", Unban, config.GetModCommands.Contains("unban")));
            commands.Add(new Command("banned", "banned - list banned users", PrintBanned, config.GetModCommands.Contains("banned")));
            commands.Add(new Command("permitted", "permitted - list permitted users", PrintPermitted, config.GetModCommands.Contains("permitted")));
            commands.Add(new Command("permit", "permit [ip/steamID] - add user to permitted user list", Permit, config.GetModCommands.Contains("permit")));
            commands.Add(new Command("unpermit", "unpermit [ip/steamID] - remove user from permitted user list", UnPermit, config.GetModCommands.Contains("unpermit")));
            commands.Add(new Command("addadmin", "addAdmin [name/steamID] - add user to admin list", AddAdmin, config.GetModCommands.Contains("addadmin")));
            commands.Add(new Command("removeadmin", "removeAdmin [name/steamID] - remove user from admin list", RemoveAdmin, config.GetModCommands.Contains("admins")));
            commands.Add(new Command("admins", "admins - list  of admin steam ids", PrintAdmins, config.GetModCommands.Contains("admins")));
            commands.Add(new Command("updateLists", "updateLists - force updates banned, admin, mod and permited lists with data in coresponding files", UpdateFromFile, config.GetModCommands.Contains("updateLists")));
            commands.Add(new Command("save", "save - save server", Save, config.GetModCommands.Contains("save")));
            commands.Add(new Command("difficulty", "difficulty [nr] - force difficulty", Difficulty, config.GetModCommands.Contains("difficulty")));
            commands.Add(new Command("memory", "memory - show amount of memory used by server", Memory, config.GetModCommands.Contains("memory")));
            commands.Add(new Command("shutdown", "shutdown - shutdown the server", Shutdown, config.GetModCommands.Contains("shutdown")));
            commands.Add(new Command("sleep", "sleep - force night skip", Sleep, config.GetModCommands.Contains("sleep")));
            commands.Add(new Command("say", "say [message] - to say something as server", Say, config.GetModCommands.Contains("say")));
            commands.Add(new Command("yell", "yell [message] - to shout something as server", Yell, config.GetModCommands.Contains("yell")));
            commands.Add(new Command("config", "config - shows all what is set on your settings", Config, config.GetModCommands.Contains("config")));
            commands.Add(new Command("online", "online - display list of players online with their steamIDs", Online, config.GetModCommands.Contains("online")));
            commands.Add(new Command("ip", "ip - show server ip", IpAddress, config.GetModCommands.Contains("ip")));
            commands.Add(new Command("tps", "tps - display server updates/sec", Tps, config.GetModCommands.Contains("tps")));
            commands.Add(new Command("version", "version - prints server version", PrintVersion, config.GetModCommands.Contains("version")));
            commands.Add(new Command("addmod", "addMod [name/steamID] - add user to moderator list", AddMod, config.GetModCommands.Contains("addmod")));
            commands.Add(new Command("removeMod", "removemod [name/steamID] - remove user from moderator list", RemoveMod, config.GetModCommands.Contains("removemod")));
            commands.Add(new Command("mods", "mods - print moderator player list in server console", PrintMods, config.GetModCommands.Contains("mods")));
            commands.Add(new Command("modCommands", "modCommands - list of commands available to moderators", PrintModCommands, config.GetModCommands.Contains("modcommands")));

            commands.Sort((x, y) => x.Key.CompareTo(y.Key));
        }

        public void RunCommand(string text, bool calledFromClient)
        {
            if (!text.IsNullOrWhiteSpace())
            {
                text = text.Trim();
                string[] args = text.Split(' ');
                if (args.Length > 0)
                {
                    Command command = commands.Find(c => c.Key.Equals(args[0].ToLower()));
                    if (!calledFromClient || command.ModsAllowed)
                    {
                        if (command != null)
                        {
                            bool finished = command.Run((string[])args.Clone());
                            if (finished)
                            {
                                return;
                            }
                            else
                            {
                                if (!args[0].IsNullOrWhiteSpace())
                                {
                                    Print(command.Hint);
                                    return;
                                }
                                Print($"Something went wrong with args[0]: {args[0]}");
                            }
                        }
                    }
                }
                Print("Invalid command to get all commands please use: help");
            }
        }
    }
}
