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
        
        public  ConfigTool config;

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
            commands.Add(new Command("help", "help - get list of all commands", Help));
            commands.Add(new Command("kick", "kick [name/steamID] - kick user", Kick));
            commands.Add(new Command("ban", "ban [name/ip/steamID] - ban user", Ban));
            commands.Add(new Command("unban", "unban [ip/steamID] - unban user", Unban));
            commands.Add(new Command("banned", "banned - list banned users", PrintBanned));
            commands.Add(new Command("permitted", "permitted - list permitted users", PrintPermitted));
            commands.Add(new Command("permit", "permit [ip/steamID] - add user to permitted user list", Permit));
            commands.Add(new Command("unpermit", "unpermit [ip/steamID] - remove user from permitted user list", UnPermit));
            commands.Add(new Command("addadmin", "addAdmin [steamID] - add user to admin list", AddAdmin));
            commands.Add(new Command("removeadmin", "removeAdmin [steamID] - remove user from admin list", RemoveAdmin));
            commands.Add(new Command("admins", "removeAdmin [steamID] - remove user from admin list", PrintAdmins));
            commands.Add(new Command("updateLists", "force updates banned, admin and permited list with data in coresponding files", UpdateFromFile));
            commands.Add(new Command("save", "save - save server", Save));
            commands.Add(new Command("difficulty", "difficulty [nr] - force difficulty", Difficulty));
            commands.Add(new Command("memory", "memory - show amount of memory used by server", Memory));
            commands.Add(new Command("shutdown", "shutdown - shutdown the server", Shutdown));
            commands.Add(new Command("sleep", "sleep - force night skip", Sleep));
            commands.Add(new Command("say", "say [message] - to say something as server", Say));
            commands.Add(new Command("yell", "yell [message] - to shout something as server", Yell));
            commands.Add(new Command("config", "config - shows all what is set on your settings", Config));
            commands.Add(new Command("online", "online - display list of players online with their steamIDs", Online));
        }

        public void RunCommand(string text)
        {
            if (!text.IsNullOrWhiteSpace())
            {
                text = text.Trim();
                string[] args = text.Split(' ');
                if (args.Length > 0)
                {
                    Command command = commands.Find(c => c.Key.Equals(args[0]));
                    if (command != null)
                    {
                        bool finished = command.Run((string[]) args.Clone());
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
                Print("Invalid command to get all commands please use: help");
            }
        }
    }
}
