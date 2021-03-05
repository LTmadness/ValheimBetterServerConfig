using HarmonyLib;
using System;
using System.Collections.Generic;
using BepInEx;
using UnityEngine;

namespace ValheimBetterServerConfig
{
    class ConsoleCommands
    {
        private static Dictionary<string, string> commands;

        private ZNet zNet;
        private ConfigTool config;

        public ConsoleCommands(ZNet zNet, ConfigTool config)
        {
            this.zNet = zNet;
            this.config = config;
        }

        private void registerCommands()
        {
            commands.Add("help", "Help");
            commands.Add("kick", "Kick");
            commands.Add("ban", "Ban");
            commands.Add("unban", "Unban");
            commands.Add("banned", "printBanned");
            commands.Add("permitted", "printPermitted");
            commands.Add("permit", "Permit");
            commands.Add("unpermit", "UnPermit");
            commands.Add("addadmin", "AddAdmin");
            commands.Add("removeadmin", "RemoveAdmin");
            commands.Add("admins", "printAdmins");
            commands.Add("save", "Save");
            commands.Add("difficulty", "Difficulty");
            commands.Add("memory", "Memory");
            commands.Add("shutdown", "Shutdown");
            commands.Add("sleep", "Sleep");
            commands.Add("say", "Say");
            commands.Add("yell", "Yell");
            commands.Add("config", "Config");
        }

        public void runCommand(string text)
        {
            string[] arg = text.Split(' ');
            string method = commands[arg[0].ToLower()];

            if (method != null)
            {
                AccessTools.Method(typeof(ConsoleCommands), method).Invoke(this, new object[] { arg });
            }
            else
            {
                print("Invalid command to get all commands please use: help");
            }
        }

        public static void Help(string[] args)
        {
            print("Available commands:");
            print("kick [name/ip/userID] - kick user");
            print("ban [name/ip/userID] - ban user");
            print("unban [ip/userID] - unban user");
            print("banned - list banned users");
            print("permit [ip/userID] - add user to permitted user list");
            print("unpermit [ip/userID] - remove user from permitted user list");
            print("permitted - list permitted users");
            print("addAdmin [userID] - add user to admin list");
            print("removeAdmin [userID] - remove user from admin list");
            print("admins - list of admin user ids");
            print("save - save server");
            print("shutdown - shutdown the server");
            print("difficulty [nr] - force difficulty");
            print("sleep - force night skip");
            print("say [message] - to say something as server");
            print("yell [message] - to shout something as server");
            print("config - shows all what is set on your settings");
        }

        public static void print(string text)
        {
            System.Console.WriteLine(text);
        }

        private void Kick(string[] args)
        {
            if (UserSupplied(args))
            {
                ZNetPeer znetPeer = zNet.GetPeerByHostName(args[1]);
                if (znetPeer == null)
                {
                    znetPeer = zNet.GetPeerByPlayerName(args[1]);
                }
                if (znetPeer != null)
                {
                    print("Kicking " + znetPeer.m_playerName);
                    SendDisconnect(znetPeer);
                }
            }
        }
        private void Ban(string[] args)
        {
            if (UserSupplied(args))
            {
                ZNetPeer peerByPlayerName = zNet.GetPeerByPlayerName(args[1]);
                if (peerByPlayerName != null)
                {
                    args[1] = peerByPlayerName.m_socket.GetHostName();
                }
                print("Banning user " + args[1]);
                SyncedList bannedPlayers = (SyncedList)AccessTools.Field(typeof(ZNet), "m_bannedList").GetValue(zNet);
                bannedPlayers.Add(args[1]);
                AccessTools.Field(typeof(ZNet), "m_bannedList").SetValue(zNet, bannedPlayers);
            }
        }

        private void Unban(string[] args)
        {
            if (UserSupplied(args))
            {
                print("Unbanning user " + args[1]);
                SyncedList bannedPlayers = (SyncedList)AccessTools.Field(typeof(ZNet), "m_bannedList").GetValue(zNet);
                bannedPlayers.Remove(args[1]);
                AccessTools.Field(typeof(ZNet), "m_bannedList").SetValue(zNet, bannedPlayers);
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
            if (args.Length <= 1 && args[1].IsNullOrWhiteSpace())
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
                num = int.Parse(args[1].Substring(11));
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
            print("Total allocated mem: " + (totalMemory / 1048576L).ToString("0") + "mb");
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
            return String.Concat(args, " ").Trim();
        }
    }
}
