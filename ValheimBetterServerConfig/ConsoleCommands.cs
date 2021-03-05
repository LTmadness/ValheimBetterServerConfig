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

            commands.Add("config", "Config");
            hint.Add("config", "config - shows all what is set on your settings");
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
                        AccessTools.Method(typeof(ConsoleCommands), method).Invoke(this, new object[] { arg });
                        return;
                    }
                }
                print("Invalid command to get all commands please use: help");
            }
        }

        public static void Help(string[] args)
        {
            print("Available commands:");
            foreach(KeyValuePair<string, string> entry in hint)
            {
                print(entry.Value);
            }
        }

        public static void print(string text)
        {
            System.Console.WriteLine(text);
        }

        private void Kick(string[] args)
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
                }
                else
                {
                    print("Incorrect player name: " + user);
                }
            }
            else
            {
                print(hint[args[0]]);
            }
        }

        private void Ban(string[] args)
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
            }
            else
            {
                print(hint[args[0]]);
            }
        }

        private void Unban(string[] args)
        {
            if (UserSupplied(args))
            {
                string user = rebuildString(args);
                print("Unbanning user " + user);
                SyncedList bannedPlayers = (SyncedList)AccessTools.Field(typeof(ZNet), "m_bannedList").GetValue(zNet);
                bannedPlayers.Remove(user);
                AccessTools.Field(typeof(ZNet), "m_bannedList").SetValue(zNet, bannedPlayers);
            }
            else
            {
                print(hint[args[0]]);
            }
        }

        private void Permit(string[] args)
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
            }
            else
            {
                print(hint[args[0]]);
            }
        }

        private void UnPermit(string[] args)
        {
            if (UserSupplied(args))
            {
                string user = rebuildString(args);
                print("Removing user from permited user list" + user);
                SyncedList permittedPlayers = (SyncedList)AccessTools.Field(typeof(ZNet), "m_permittedList").GetValue(zNet);
                permittedPlayers.Remove(user);
                AccessTools.Field(typeof(ZNet), "m_permittedList").SetValue(zNet, permittedPlayers);
            }
            else
            {
                print(hint[args[0]]);
            }
        }

        private void AddAdmin(string[] args)
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
            }
            else
            {
                print(hint[args[0]]);
            }
        }

        private void RemoveAdmin(string[] args)
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
            }
            else
            {
                print(hint[args[0]]);
            }
        }

        private void SendDisconnect(ZNetPeer peer)
        {
            if (peer.m_rpc != null)
            {
                print("Sent to " + peer.m_socket.GetEndPointString());
                peer.m_rpc.Invoke("Disconnect", Array.Empty<object>());
            }
        }

        private void printBanned(string[] args)
        {
            print("Ban player steam IDs/IPs:");
            SyncedList ids = (SyncedList)AccessTools.Field(typeof(ZNet), "m_bannedList").GetValue(zNet);
            List<string> idList = ids.GetList();
            foreach (string id in idList)
            {
                print(id);
            }
        }

        private void printPermitted(string[] args)
        {
            print("Permited player steam IDs/IPs:");
            SyncedList ids = (SyncedList)AccessTools.Field(typeof(ZNet), "m_permittedList").GetValue(zNet);
            List<string> idList = ids.GetList();
            foreach (string id in idList)
            {
                print(id);
            }
        }

        private void printAdmins(string[] args)
        {
            print("Admin ids:");
            SyncedList ids = (SyncedList)AccessTools.Field(typeof(ZNet), "m_adminList").GetValue(zNet);
            List<string> idList = ids.GetList();
            foreach (string id in idList)
            {
                print(id);
            }
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

        private void Save(string[] args)
        {
            zNet.Save(false);
        }

        private void Difficulty(string[] args)
        {
            int num = 0;
            try
            {
                num = int.Parse(args[1]);
            }
            catch
            {
                print("Number was incorrect");
                return;
            }

            if (num >= 1)
            {
                Game.instance.SetForcePlayerDifficulty(num);
                print("Setting players to " + num);
            }

        }

        private void Memory(string[] args)
        {
            long totalMemory = GC.GetTotalMemory(false);
            print("Total allocated memory: " + (totalMemory / 1048576L).ToString("0") + "mb");
        }

        private void Shutdown(string[] args)
        {
            zNet.Save(true);
            Application.Quit();
            System.Console.Out.Close();
        }

        private void Sleep(string[] args)
        {
            EnvMan.instance.SkipToMorning();
        }

        private void Say(string[] args)
        {
            string message = rebuildString(args);
            string username = config.Username;
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ChatMessage", new object[] { new Vector3(), 1, username, message });
            print(username + ": " + message);
        }

        private void Yell(string[] args)
        {
            string message = rebuildString(args);
            string username = config.Username;
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ChatMessage", new object[] { new Vector3(), 2, username, message });
            print(username + ": " + message);
        }

        private void Config(string[] args)
        {
            print("Your current settings:");
            foreach (string conf in config.getList())
            {
                print(conf);
            }
        }

        private string rebuildString(string[] args)
        {
            args[0] = "";
            return String.Join(" ", args).Trim();
        }
    }
}
