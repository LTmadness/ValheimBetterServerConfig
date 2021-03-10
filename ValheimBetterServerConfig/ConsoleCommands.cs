using HarmonyLib;
using System;
using System.Collections.Generic;
using BepInEx;
using UnityEngine;

namespace ValheimBetterServerConfig
{
    class ConsoleCommands
    {

        private static List<Command> commands = new List<Command>();
        
        private ConfigTool config;

        public ConsoleCommands(ConfigTool config)
        {
            this.config = config;
            registerCommands();
        }

        private void registerCommands()
        {
            commands.Add(new Command("help", "help - get list of all commands", Help));
            commands.Add(new Command("kick", "kick [name/steamID] - kick user", Kick));
            commands.Add(new Command("ban", "ban [name/ip/steamID] - ban user", Ban));
            commands.Add(new Command("unban", "unban [ip/steamID] - unban user", Unban));
            commands.Add(new Command("banned", "banned - list banned users", printBanned));
            commands.Add(new Command("permitted", "permitted - list permitted users", printPermitted));
            commands.Add(new Command("permit", "permit [ip/steamID] - add user to permitted user list", Permit));
            commands.Add(new Command("unpermit", "unpermit [ip/steamID] - remove user from permitted user list", UnPermit));
            commands.Add(new Command("addadmin", "addAdmin [steamID] - add user to admin list", AddAdmin));
            commands.Add(new Command("removeadmin", "removeAdmin [steamID] - remove user from admin list", RemoveAdmin));
            commands.Add(new Command("admins", "removeAdmin [steamID] - remove user from admin list", printAdmins));
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

        public void runCommand(string text)
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
                                print(command.Hint);
                                return;
                            }
                            print($"Something went wrong with args[0]: {args[0]}");
                        }
                    }
                }
                print("Invalid command to get all commands please use: help");
            }
        }

        private string rebuildString(string[] args)
        {
            args[0] = "";
            return String.Join(" ", args).Trim();
        }

        public void print(string text)
        {
            System.Console.WriteLine(text);
        }

        private bool UserSupplied(string[] args)
        {
            if (args.Length <= 1 || args[1].IsNullOrWhiteSpace())
            {
                print("No or incorrect user supplied");
                return false;
            }
            return true;
        }

        private bool Help(string[] args)
        {
            print("Available commands:");
            foreach (Command command in commands)
            {
                print(command.Hint);
            }
            return true;
        }

        private bool Kick(string[] args)
        {
            if (UserSupplied(args))
            {
                string user = rebuildString(args);

                ZNet zNet = ZNet.instance;
                ZNetPeer znetPeer = zNet.GetPeerByHostName(user);
                if (znetPeer == null)
                {
                    znetPeer = zNet.GetPeerByPlayerName(user);
                }
                if (znetPeer != null)
                {
                    print("Kicking " + znetPeer.m_playerName);
                    znetPeer.m_rpc.Invoke("Disconnect", Array.Empty<object>());
                    zNet.Disconnect(znetPeer);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        private bool Ban(string[] args)
        {
            if (UserSupplied(args))
            {
                string user = rebuildString(args);
                ZNet zNet = ZNet.instance;
                ZNetPeer peerByPlayerName = zNet.GetPeerByPlayerName(user);
                if (peerByPlayerName != null)
                {
                    user = peerByPlayerName.m_socket.GetHostName();
                }
                print("Banning user: " + user);
                SyncedList bannedPlayers = (SyncedList)AccessTools.Field(typeof(ZNet), "m_bannedList").GetValue(zNet);
                bannedPlayers.Add(user);
                AccessTools.Field(typeof(ZNet), "m_bannedList").SetValue(zNet, bannedPlayers);
                return true;
            }
            return false;
        }

        private bool Unban(string[] args)
        {
            if (UserSupplied(args))
            {
                string user = rebuildString(args);
                ZNet zNet = ZNet.instance;
                print("Unbanning user: " + user);
                SyncedList bannedPlayers = (SyncedList)AccessTools.Field(typeof(ZNet), "m_bannedList").GetValue(zNet);
                bannedPlayers.Remove(user);
                AccessTools.Field(typeof(ZNet), "m_bannedList").SetValue(zNet, bannedPlayers);
                return true;
            }
            return false;
        }

        private bool Permit(string[] args)
        {
            if (UserSupplied(args))
            {
                string user = rebuildString(args);
                ZNet zNet = ZNet.instance;
                ZNetPeer peerByPlayerName = zNet.GetPeerByPlayerName(user);
                if (peerByPlayerName != null)
                {
                    user = peerByPlayerName.m_socket.GetHostName();
                }
                print("Permitting user: " + user);
                SyncedList permittedPlayers = (SyncedList)AccessTools.Field(typeof(ZNet), "m_permittedList").GetValue(zNet);
                permittedPlayers.Add(user);
                AccessTools.Field(typeof(ZNet), "m_permittedList").SetValue(zNet, permittedPlayers);
                return true;
            }
            return false;
        }

        private bool UnPermit(string[] args)
        {
            if (UserSupplied(args))
            {
                string user = rebuildString(args);
                ZNet zNet = ZNet.instance;
                print("Removing user from permited user list: " + user);
                SyncedList permittedPlayers = (SyncedList)AccessTools.Field(typeof(ZNet), "m_permittedList").GetValue(zNet);
                permittedPlayers.Remove(user);
                AccessTools.Field(typeof(ZNet), "m_permittedList").SetValue(zNet, permittedPlayers);
                return true;
            }
            return false;
        }

        private bool AddAdmin(string[] args)
        {
            if (UserSupplied(args))
            {
                string user = rebuildString(args);
                ZNet zNet = ZNet.instance;
                ZNetPeer peerByPlayerName = zNet.GetPeerByPlayerName(user);
                if (peerByPlayerName != null)
                {
                    user = peerByPlayerName.m_socket.GetHostName();
                }
                print("Adding player to the admin list: " + user);
                SyncedList adminList = (SyncedList)AccessTools.Field(typeof(ZNet), "m_adminList").GetValue(zNet);
                adminList.Add(user);
                AccessTools.Field(typeof(ZNet), "m_adminList").SetValue(zNet, adminList);
                return true;
            }
            return false;
        }

        private bool RemoveAdmin(string[] args)
        {
            if (UserSupplied(args))
            {
                string user = rebuildString(args);
                ZNet zNet = ZNet.instance;
                ZNetPeer peerByPlayerName = zNet.GetPeerByPlayerName(user);
                if (peerByPlayerName != null)
                {
                    user = peerByPlayerName.m_socket.GetHostName();
                }
                print("Removing player from the admin list: " + user);
                SyncedList adminList = (SyncedList)AccessTools.Field(typeof(ZNet), "m_adminList").GetValue(zNet);
                adminList.Remove(user);
                AccessTools.Field(typeof(ZNet), "m_adminList").SetValue(zNet, adminList);
                return true;
            }
            return false;
        }

        private bool printBanned(string[] args)
        {
            print("Ban player steam IDs/IPs:");
            SyncedList ids = (SyncedList)AccessTools.Field(typeof(ZNet), "m_bannedList").GetValue(ZNet.instance);
            List<string> idList = ids.GetList();
            foreach (string id in idList)
            {
                print(id);
            }
            return true;
        }

        private bool printPermitted(string[] args)
        {
            print("Permited player steam IDs/IPs:");
            SyncedList ids = (SyncedList)AccessTools.Field(typeof(ZNet), "m_permittedList").GetValue(ZNet.instance);
            List<string> idList = ids.GetList();
            foreach (string id in idList)
            {
                print(id);
            }
            return true;
        }

        private bool printAdmins(string[] args)
        {
            print("Admin ids:");
            SyncedList ids = (SyncedList)AccessTools.Field(typeof(ZNet), "m_adminList").GetValue(ZNet.instance);
            List<string> idList = ids.GetList();
            foreach (string id in idList)
            {
                print(id);
            }
            return true;
        }

        private bool UpdateFromFile(string[] args)
        {
            ZNet zNet = ZNet.instance;
            AccessTools.Field(typeof(ZNet), "m_adminList").SetValue(zNet, new SyncedList(Utils.GetSaveDataPath() + "/adminlist.txt", "List admin players ID  ONE per line"));
            AccessTools.Field(typeof(ZNet), "m_bannedList").SetValue(zNet, new SyncedList(Utils.GetSaveDataPath() + " / bannedlist.txt", "List banned players ID  ONE per line"));
            AccessTools.Field(typeof(ZNet), "m_permittedList").SetValue(zNet, new SyncedList(Utils.GetSaveDataPath() + " / permittedlist.txt", "List permitted players ID ONE per line"));
            return true;
        }

        private bool Save(string[] args)
        {
            ZNet.instance.Save(false);
            return true;
        }

        private bool Difficulty(string[] args)
        {
            int num = 0;
            try
            {
                num = int.Parse(args[1]);
                if (num >= 1)
                {
                    Game.instance.SetForcePlayerDifficulty(num);
                    print("Setting players to " + num);
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        private bool Memory(string[] args)
        {
            long totalMemory = GC.GetTotalMemory(false);
            print("Total allocated memory: " + (totalMemory / 1048576L).ToString("0") + "mb");
            return true;
        }

        private bool Shutdown(string[] args)
        {
            ZNet.instance.Save(true);
            Application.Quit();
            System.Console.Out.Close();
            return true;
        }

        private bool Sleep(string[] args)
        {
            EnvMan.instance.SkipToMorning();
            return true;
        }

        private bool Say(string[] args)
        {
            if (args.Length > 1)
            {
                string message = rebuildString(args);
                string username = config.Username;
                ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ChatMessage", new object[] { new Vector3(), 1, username, message });
                print(username + " said " + message);
                return true;
            }
            return false;
        }

        private bool Yell(string[] args)
        {
            if (args.Length > 1)
            {
                string message = rebuildString(args);
                string username = config.Username;
                ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ChatMessage", new object[] { new Vector3(), 2, username, message });
                print(username + " yelled " + message);
                return true;
            }
            return false;
        }

        private bool Config(string[] args)
        {
            print("Your current settings:");
            foreach (string conf in config.getList())
            {
                print(conf);
            }
            return true;
        }

        private bool Online(string[] args)
        {
            List<ZNet.PlayerInfo> players = (List<ZNet.PlayerInfo>)AccessTools.Field(typeof(ZNet), "m_players").GetValue(ZNet.instance);
            if (players.Count > 0)
            {
                print("Players currently online:");
                int counter = 1;
                foreach (ZNet.PlayerInfo player in players)
                {
                    print(counter + ". " + player.m_name + " - " + player.m_host);
                    counter++;
                }
            }
            else
            {
                print("No players currently online");
            }
            return true;
        }
    }
}
