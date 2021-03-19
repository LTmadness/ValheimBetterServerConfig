using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using static ValheimBetterServerConfig.Console.Utils;
using static Utils;
using Steamworks;

namespace ValheimBetterServerConfig.Console
{
    class Methods
    {
        public static bool Help(string[] args)
        {
            ArgumentSkipped(args);
            Print("Available commands:");
            foreach (Command command in Runner.Instance.commands)
            {
                Print(command.Hint);
            }
            return true;
        }

        public static bool Kick(string[] args)
        {
            if (UserSupplied(args))
            {
                string user = RebuildString(args);

                ZNet zNet = ZNet.instance;
                ZNetPeer znetPeer = zNet.GetPeerByHostName(user);
                if (znetPeer == null)
                {
                    znetPeer = zNet.GetPeerByPlayerName(user);
                }
                if (znetPeer != null)
                {
                    Print($"Kicking {znetPeer.m_playerName}");
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

        public static bool Ban(string[] args)
        {
            if (UserSupplied(args))
            {
                string user = RebuildString(args);
                ZNet zNet = ZNet.instance;
                ZNetPeer peerByPlayerName = zNet.GetPeerByPlayerName(user);
                if (peerByPlayerName != null)
                {
                    user = peerByPlayerName.m_socket.GetHostName();
                }
                Print($"Banning user: {user}");
                SyncedList bannedPlayers = (SyncedList)AccessTools.Field(typeof(ZNet), "m_bannedList").GetValue(zNet);
                bannedPlayers.Add(user);
                AccessTools.Field(typeof(ZNet), "m_bannedList").SetValue(zNet, bannedPlayers);
                return true;
            }
            return false;
        }

        public static bool Unban(string[] args)
        {
            if (UserSupplied(args))
            {
                string user = RebuildString(args);
                ZNet zNet = ZNet.instance;
                Print($"Unbanning user: {user}");
                SyncedList bannedPlayers = (SyncedList)AccessTools.Field(typeof(ZNet), "m_bannedList").GetValue(zNet);
                bannedPlayers.Remove(user);
                AccessTools.Field(typeof(ZNet), "m_bannedList").SetValue(zNet, bannedPlayers);
                return true;
            }
            return false;
        }

        public static bool Permit(string[] args)
        {
            if (UserSupplied(args))
            {
                string user = RebuildString(args);
                ZNet zNet = ZNet.instance;
                ZNetPeer peerByPlayerName = zNet.GetPeerByPlayerName(user);
                if (peerByPlayerName != null)
                {
                    user = peerByPlayerName.m_socket.GetHostName();
                }
                Print($"Permitting user: {user}");
                SyncedList permittedPlayers = (SyncedList)AccessTools.Field(typeof(ZNet), "m_permittedList").GetValue(zNet);
                permittedPlayers.Add(user);
                AccessTools.Field(typeof(ZNet), "m_permittedList").SetValue(zNet, permittedPlayers);
                return true;
            }
            return false;
        }

        public static bool UnPermit(string[] args)
        {
            if (UserSupplied(args))
            {
                string user = RebuildString(args);
                ZNet zNet = ZNet.instance;
                Print($"Removing user from permited user list: {user}");
                SyncedList permittedPlayers = (SyncedList)AccessTools.Field(typeof(ZNet), "m_permittedList").GetValue(zNet);
                permittedPlayers.Remove(user);
                AccessTools.Field(typeof(ZNet), "m_permittedList").SetValue(zNet, permittedPlayers);
                return true;
            }
            return false;
        }

        public static bool AddAdmin(string[] args)
        {
            if (UserSupplied(args))
            {
                string user = RebuildString(args);
                ZNet zNet = ZNet.instance;
                ZNetPeer peerByPlayerName = zNet.GetPeerByPlayerName(user);
                if (peerByPlayerName != null)
                {
                    user = peerByPlayerName.m_socket.GetHostName();
                }
                Print($"Adding player to the admin list: {user}");
                SyncedList adminList = (SyncedList)AccessTools.Field(typeof(ZNet), "m_adminList").GetValue(zNet);
                adminList.Add(user);
                AccessTools.Field(typeof(ZNet), "m_adminList").SetValue(zNet, adminList);
                return true;
            }
            return false;
        }

        public static bool RemoveAdmin(string[] args)
        {
            if (UserSupplied(args))
            {
                string user = RebuildString(args);
                ZNet zNet = ZNet.instance;
                ZNetPeer peerByPlayerName = zNet.GetPeerByPlayerName(user);
                if (peerByPlayerName != null)
                {
                    user = peerByPlayerName.m_socket.GetHostName();
                }
                Print($"Removing player from the admin list: {user}");
                SyncedList adminList = (SyncedList)AccessTools.Field(typeof(ZNet), "m_adminList").GetValue(zNet);
                adminList.Remove(user);
                AccessTools.Field(typeof(ZNet), "m_adminList").SetValue(zNet, adminList);
                return true;
            }
            return false;
        }

        public static bool PrintBanned(string[] args)
        {
            ArgumentSkipped(args);
            Print("Ban player steam IDs/IPs:");
            SyncedList ids = (SyncedList)AccessTools.Field(typeof(ZNet), "m_bannedList").GetValue(ZNet.instance);
            List<string> idList = ids.GetList();
            foreach (string id in idList)
            {
                Print(id);
            }
            return true;
        }

        public static bool PrintPermitted(string[] args)
        {
            ArgumentSkipped(args);
            Print("Permited player steam IDs/IPs:");
            SyncedList ids = (SyncedList)AccessTools.Field(typeof(ZNet), "m_permittedList").GetValue(ZNet.instance);
            List<string> idList = ids.GetList();
            foreach (string id in idList)
            {
                Print(id);
            }
            return true;
        }

        public static bool PrintAdmins(string[] args)
        {
            ArgumentSkipped(args);
            Print("Admin ids:");
            SyncedList ids = (SyncedList)AccessTools.Field(typeof(ZNet), "m_adminList").GetValue(ZNet.instance);
            List<string> idList = ids.GetList();
            foreach (string id in idList)
            {
                Print(id);
            }
            return true;
        }

        public static bool UpdateFromFile(string[] args)
        {
            ArgumentSkipped(args);
            ZNet zNet = ZNet.instance;
            AccessTools.Field(typeof(ZNet), "m_adminList").SetValue(zNet, new SyncedList(GetSaveDataPath() + "/adminlist.txt", "List admin players ID  ONE per line"));
            AccessTools.Field(typeof(ZNet), "m_bannedList").SetValue(zNet, new SyncedList(GetSaveDataPath() + " / bannedlist.txt", "List banned players ID  ONE per line"));
            AccessTools.Field(typeof(ZNet), "m_permittedList").SetValue(zNet, new SyncedList(GetSaveDataPath() + " / permittedlist.txt", "List permitted players ID ONE per line"));
            return true;
        }

        public static bool Save(string[] args)
        {
            ArgumentSkipped(args);
            ZNet.instance.Save(false);
            return true;
        }

        public static bool Difficulty(string[] args)
        {
            try
            {
                int num = int.Parse(args[1]);
                if (num >= 1)
                {
                    Game.instance.SetForcePlayerDifficulty(num);
                    Print($"Setting players to {num}");
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        public static bool Memory(string[] args)
        {
            ArgumentSkipped(args);
            long totalMemory = GC.GetTotalMemory(false);
            Print($"Total allocated memory: {totalMemory / 1048576L:0} mb");
            return true;
        }

        public static bool Shutdown(string[] args)
        {
            ArgumentSkipped(args);
            ZNet.instance.Save(true);
            Application.Quit();
            System.Console.Out.Close();
            return true;
        }

        public static bool Sleep(string[] args)
        {
            ArgumentSkipped(args);
            EnvMan.instance.SkipToMorning();
            return true;
        }

        public static bool Say(string[] args)
        {
            if (args.Length > 1)
            {
                string message = RebuildString(args);
                string username = Runner.Instance.config.Username;
                ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ChatMessage", new object[] { new Vector3(), 1, username, message });
                Print($"{username} said {message}");
                return true;
            }
            return false;
        }

        public static bool Yell(string[] args)
        {
            if (args.Length > 1)
            {
                string message = RebuildString(args);
                string username = Runner.Instance.config.Username;
                ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ChatMessage", new object[] { new Vector3(), 2, username, message });
                Print($"{username} yelled {message}");
                return true;
            }
            return false;
        }

        public static bool Config(string[] args)
        {
            ArgumentSkipped(args);
            Print("Your current settings:");
            foreach (string conf in Runner.Instance.config.getList())
            {
                Print(conf);
            }
            return true;
        }

        public static bool Online(string[] args)
        {
            ArgumentSkipped(args);
            List<ZNet.PlayerInfo> players = (List<ZNet.PlayerInfo>)AccessTools.Field(typeof(ZNet), "m_players").GetValue(ZNet.instance);
            if (players.Count > 0)
            {
                Print("Players currently online:");
                int counter = 1;
                foreach (ZNet.PlayerInfo player in players)
                {
                    Print($"{counter}. {player.m_name} - {player.m_host}");
                    counter++;
                }
            }
            else
            {
                Print("No players currently online");
            }
            return true;
        }

        public static bool IpAddress(string[] args)
        {
            ArgumentSkipped(args);
            Print($"Sever IP: {AccessTools.Method(typeof(ZNet), "GetPublicIP").Invoke(ZNet.instance, new object[] { })}");
            return true;
        }
    }
}
