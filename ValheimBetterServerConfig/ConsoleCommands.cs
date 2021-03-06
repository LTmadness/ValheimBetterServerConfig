using HarmonyLib;
using System;
using System.Collections.Generic;
using BepInEx;
using UnityEngine;

namespace ValheimBetterServerConfig
{
    class ConsoleCommands
    {

        private static Dictionary<string, string> commands = new Dictionary<string, string>();
        private static Dictionary<string, string> hint = new Dictionary<string, string>();
        
        private ZNet zNet;
        private ConfigTool config;

        public ConsoleCommands(ZNet zNet, ConfigTool config)
        {
            this.zNet = zNet;
            this.config = config;
            registerCommands();
        }

        private void registerCommands()
        {
            commands.Add("help", "Help");
            hint.Add("help", "help - get list of all commands");

            commands.Add("kick", "Kick");
            hint.Add("kick", "kick [name/ip/userID] - kick user");

            commands.Add("ban", "Ban");
            hint.Add("ban", "ban [name/ip/userID] - ban user");

            commands.Add("unban", "Unban");
            hint.Add("unban", "unban [ip/userID] - unban user");

            commands.Add("banned", "printBanned");
            hint.Add("banned", "banned - list banned users");

            commands.Add("permitted", "printPermitted");
            hint.Add("permitted", "permitted - list permitted users");

            commands.Add("permit", "Permit");
            hint.Add("permit", "permit [ip/userID] - add user to permitted user list");

            commands.Add("unpermit", "UnPermit");
            hint.Add("unpermit", "unpermit [ip/userID] - remove user from permitted user list");

            commands.Add("addadmin", "AddAdmin");
            hint.Add("addadmin", "addAdmin [userID] - add user to admin list");

            commands.Add("removeadmin", "RemoveAdmin");
            hint.Add("removeadmin", "removeAdmin [userID] - remove user from admin list");

            commands.Add("admins", "printAdmins");
            hint.Add("admins", "admins - list of admin user ids");

            commands.Add("save", "Save");
            hint.Add("save", "save - save server");

            commands.Add("difficulty", "Difficulty");
            hint.Add("difficulty", "difficulty [nr] - force difficulty");

            commands.Add("memory", "Memory");
            hint.Add("memory", "memory - show amount of memory used by server");

            commands.Add("shutdown", "Shutdown");
            hint.Add("shutdown", "shutdown - shutdown the server");

            commands.Add("sleep", "Sleep");
            hint.Add("sleep", "sleep - force night skip");

            commands.Add("say", "Say");
            hint.Add("say", "say [message] - to say something as server");

            commands.Add("yell", "Yell");
            hint.Add("yell", "yell [message] - to shout something as server");

            //commands.Add("whisper", "Whisper");
            //hint.Add("whisper", "whisper [name] [message] - whisper something to a single player");

            commands.Add("config", "Config");
            hint.Add("config", "config - shows all what is set on your settings");

            commands.Add("online", "Online");
            hint.Add("online", "online - display list of players online");
        }

        public void runCommand(string text)
        {
            text = text.Trim();
            if (!text.IsNullOrWhiteSpace())
            {
                string[] arg = text.Split(' ');
                if (arg.Length > 0)
                {
                    if (commands.ContainsKey(arg[0].ToLower()))
                    {
                        string method = commands[arg[0].ToLower()];
                        bool finished = (bool)AccessTools.Method(typeof(ConsoleCommands), method).Invoke(this, new object[] { arg });
                        if (finished)
                        {
                            return;
                        }
                        else
                        {
                            print(hint[arg[0]]);
                            return;
                        }
                    }
                }
                print("Invalid command to get all commands please use: " + hint["help"]);
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
            if (args.Length < 1 && args[1].IsNullOrWhiteSpace())
            {
                print("No or incorrect user supplied");
                return false;
            }
            return true;
        }

        private void SendDisconnect(ZNetPeer peer)
        {
            if (peer.m_rpc != null)
            {
                print("Sent to " + peer.m_socket.GetEndPointString());
                peer.m_rpc.Invoke("Disconnect", Array.Empty<object>());
            }
        }

        private bool Help(string[] args)
        {
            print("Available commands:");
            foreach (KeyValuePair<string, string> entry in hint)
            {
                print(entry.Value);
            }
            return true;
        }

        private bool Kick(string[] args)
        {
            if (UserSupplied(args))
            {
                string user = rebuildString(args);
                ZNetPeer znetPeer = zNet.GetPeerByHostName(user);
                if (znetPeer == null)
                {
                    znetPeer = zNet.GetPeerByPlayerName(user);
                }
                if (znetPeer != null)
                {
                    print("Kicking " + znetPeer.m_playerName);
                    SendDisconnect(znetPeer);
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
                ZNetPeer peerByPlayerName = zNet.GetPeerByPlayerName(user);
                if (peerByPlayerName != null)
                {
                    user = peerByPlayerName.m_socket.GetHostName();
                }
                print("Banning user " + user);
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
                print("Unbanning user " + user);
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
                ZNetPeer peerByPlayerName = zNet.GetPeerByPlayerName(user);
                if (peerByPlayerName != null)
                {
                    user = peerByPlayerName.m_socket.GetHostName();
                }
                print("Permitting user " + user);
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
                print("Removing user from permited user list" + user);
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
                ZNetPeer peerByPlayerName = zNet.GetPeerByPlayerName(user);
                if (peerByPlayerName != null)
                {
                    user = peerByPlayerName.m_socket.GetHostName();
                }
                print("Adding player to the admin list:" + user);
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
                ZNetPeer peerByPlayerName = zNet.GetPeerByPlayerName(user);
                if (peerByPlayerName != null)
                {
                    user = peerByPlayerName.m_socket.GetHostName();
                }
                print("Removing player from the admin list:" + user);
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
            SyncedList ids = (SyncedList)AccessTools.Field(typeof(ZNet), "m_bannedList").GetValue(zNet);
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
            SyncedList ids = (SyncedList)AccessTools.Field(typeof(ZNet), "m_permittedList").GetValue(zNet);
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
            SyncedList ids = (SyncedList)AccessTools.Field(typeof(ZNet), "m_adminList").GetValue(zNet);
            List<string> idList = ids.GetList();
            foreach (string id in idList)
            {
                print(id);
            }
            return true;
        }

        private bool Save(string[] args)
        {
            zNet.Save(false);
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
            zNet.Save(true);
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

 /*       private bool Whisper(string[] args)
        {
            int userNumber = 1;
            if (args.Length > 2)
            {
                Vector3 pos = new Vector3();
                Vector3 refPos = pos;
                string username = "";

                List<ZNetPeer> players = (List<ZNetPeer>)AccessTools.Field(typeof(ZRoutedRpc), "m_peers").GetValue(ZRoutedRpc.instance);

                if (players != null && players.Count <= 0)
                {
                    print("No players online");
                    return true;
                }

                while (pos.Equals(refPos) && userNumber < args.Length)
                {
                    if(userNumber == 1)
                    {
                        username += args[userNumber].ToLower();
                    }
                    else
                    {
                        username += " " + args[userNumber].ToLower();
                    }
                    
                    foreach (ZNetPeer player in players)
                    {
                        if (player.m_playerName.ToLower().Equals(username))
                        {
                            pos = player.GetRefPos();
                        }
                    }
                    userNumber++;
                }

                if (!pos.Equals(refPos))
                {
                    for (int i = 0; i < userNumber; i++)
                    {
                        args[i] = "";
                    }

                    string message = rebuildString(args);

                    ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "Say", new object[] { pos, 0, config.Username, message });
                    print(username + " whispered " + message);
                    return true;
                }
                else
                {
                    print("No user found");
                }
            }
            return false;
        }*/

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
            List<ZNet.PlayerInfo> players = (List<ZNet.PlayerInfo>)AccessTools.Field(typeof(ZNet), "m_players").GetValue(zNet);
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
