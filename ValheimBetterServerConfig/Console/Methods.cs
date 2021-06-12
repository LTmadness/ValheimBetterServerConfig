using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using static ValheimBetterServerConfig.Console.Utils;
using static Utils;
using static ValheimBetterServerConfig.ValheimBetterServerConfig;
using ValheimBetterServerConfig.Logger;

namespace ValheimBetterServerConfig.Console
{
    class Methods
    {
        public static bool Help(string[] args)
        {
            ArgumentSkipped(args);
            Runner runner = Runner.Instance;
            int page = int.Parse(args[1]);
            int totalPages = (int) Math.Ceiling((double) (runner.commands.Count / runner.config.HelpPageSize));
            if (totalPages <= page)
            {
                List<Command> pageCommands = runner.commands.GetRange((page * runner.config.HelpPageSize) - 1, runner.config.HelpPageSize);
                Print($"Available commands page {page}");
                foreach (Command command in pageCommands)
                {
                    Print(command.Hint);
                }

            } else
            {
                Print($"Please select page from range: 1 - {totalPages}");
                return false;
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
                    znetPeer = Helper.GetPeerByPlayerName(user);
                }
                if (znetPeer != null)
                {
                    Print($"Kicking {znetPeer.m_playerName}");
                    znetPeer.m_rpc.Invoke("Disconnect", Array.Empty<object>());
                    AccessTools.Method(typeof(ZNet), "ClearPlayerData").Invoke(zNet, new object[] { znetPeer });
                    List<ZNetPeer> peers = (List<ZNetPeer>)AccessTools.Field(typeof(ZNet), "m_peers").GetValue(zNet);
                    peers.Remove(znetPeer);
                    AccessTools.Field(typeof(ZNet), "m_peers").SetValue(zNet, peers);
                    znetPeer.Dispose();
                    return true;
                }
                else
                {
                    Console.Utils.Print($"User \"{user}\" not found");
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
                ZNetPeer peerByPlayerName = Helper.GetPeerByPlayerName(user);
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
                ZNetPeer peerByPlayerName = Helper.GetPeerByPlayerName(user);
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
                ZNetPeer peerByPlayerName = Helper.GetPeerByPlayerName(user);
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
                ZNetPeer peerByPlayerName = Helper.GetPeerByPlayerName(user);
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
            AccessTools.Field(typeof(ZNet), "m_adminList")
                       .SetValue(zNet, new SyncedList(GetSaveDataPath() + "/adminlist.txt", "List admin players ID  ONE per line"));
            AccessTools.Field(typeof(ZNet), "m_bannedList")
                       .SetValue(zNet, new SyncedList(GetSaveDataPath() + "/bannedlist.txt", "List banned players ID  ONE per line"));
            AccessTools.Field(typeof(ZNet), "m_permittedList")
                       .SetValue(zNet, new SyncedList(GetSaveDataPath() + "/permittedlist.txt", "List permitted players ID ONE per line"));
            Patches.config.modList = new SyncedList(GetSaveDataPath() + "/permittedlist.txt", "List permitted players ID ONE per line");
            Print("Lists refreshed");
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
                Print($"{username} said {message}", LoggerType.Chat);
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
                Print($"{username} yelled {message}", LoggerType.Chat);
                return true;
            }
            return false;
        }

        public static bool Config(string[] args)
        {
            ArgumentSkipped(args);
            Print("Your current settings:");
            foreach (string conf in Runner.Instance.config.GetList())
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

        public static bool Tps(string[] args)
        {
            ArgumentSkipped(args);
            Print($"Server tps: {(1 / Time.deltaTime):F2}");
            return true;
        }

        public static bool PrintVersion(string[] args)
        {
            ArgumentSkipped(args);
            Print($"Valheim version: {gameVersion}");
            Print($"Better Server Config version: {VERSION}");
            return true;
        }

        public static bool AddMod(string[] args)
        {
            if (UserSupplied(args))
            {
                string user = RebuildString(args);
                ZNetPeer peerByPlayerName = Helper.GetPeerByPlayerName(user);
                if (peerByPlayerName != null)
                {
                    user = peerByPlayerName.m_socket.GetHostName();
                }
                Print($"Adding player to the mod list: {user}");
                Runner.Instance.config.modList.Add(user);
                return true;
            }
            return false;
        }

        public static bool RemoveMod(string[] args)
        {
            if (UserSupplied(args))
            {
                string user = RebuildString(args);
                ZNetPeer peerByPlayerName = Helper.GetPeerByPlayerName(user);
                if (peerByPlayerName != null)
                {
                    user = peerByPlayerName.m_socket.GetHostName();
                }
                Print($"Remove player to the mod list: {user}");
                Runner.Instance.config.modList.Remove(user);
                return true;
            }
            return false;
        }

        public static bool PrintMods(string[] args)
        {
            ArgumentSkipped(args);
            Print("Mod ids:");
            List<string> idList = Runner.Instance.config.modList.GetList();
            foreach (string id in idList)
            {
                Print(id);
            }
            return true;
        }

        public static bool PrintModCommands(string[] args)
        {
            ArgumentSkipped(args);
            Print("Commands allowed to moderators:");
            foreach (string allowed in Patches.config.GetModCommands)
            {
                Command command = Runner.Instance.commands.Find(c => c.Key.Equals(allowed));
                if (command != null)
                {
                    Print(command.Hint);
                }
            }
            return true;
        }
    }
}
