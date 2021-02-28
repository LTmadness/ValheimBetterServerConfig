using HarmonyLib;
using System;
using System.Collections.Generic;
using BepInEx;

namespace ValheimBetterServerConfig
{
    class ConsoleCommands
    {
        private static ConsoleCommands m_instance;
        private ZNet zNet;
        public ConsoleCommands instance 
        {
            get
            {
                return ConsoleCommands.m_instance;
            }
         }

        public void setZNet(ZNet zNet)
        {
            this.zNet = zNet;
        }

        public ZNet getZNet()
        {
            return zNet;
        }

        public void runCommand(string text)
        {
            if (text.StartsWith("help"))
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
            }
            else
            {
                if (text.ToLower().StartsWith("kick "))
                {
                    string user = text.Substring(5);
                    Kick(user);
                    return;
                }

                if (text.ToLower().StartsWith("ban "))
                {
                    string user = text.Substring(4);
                    Ban(user);
                    return;
                }

                if (text.ToLower().StartsWith("unban "))
                {
                    string user = text.Substring(6);
                    Unban(user);
                    return;
                }

                if (text.ToLower().StartsWith("banned"))
                {
                    printBanned();
                    return;
                }

                if (text.ToLower().StartsWith("permitted"))
                {
                    printPermitted();
                    return;
                }

                if (text.ToLower().StartsWith("permit "))
                {
                    string user = text.Substring(7);
                    Permit(user);
                    return;
                }

                if (text.ToLower().StartsWith("unpermit "))
                {
                    string user = text.Substring(9);
                    UnPermit(user);
                    return;
                }

                if (text.ToLower().StartsWith("addadmin "))
                {
                    string user = text.Substring(9);
                    AddAdmin(user);
                    return;
                }

                if (text.ToLower().StartsWith("removeadmin "))
                {
                    string user = text.Substring(12);
                    RemoveAdmin(user);
                    return;
                }

                if (text.ToLower().StartsWith("admins"))
                {
                    printAdmins();
                    return;
                }

                if (text.ToLower().StartsWith("save"))
                {
                    zNet.Save(false);
                    return;
                }

                if (text.ToLower().StartsWith("difficulty "))
                {
                    int num = 0;
                    try
                    {
                        num = int.Parse(text.Substring(11));
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
                        return;
                    }
                }

                if (text.ToLower().StartsWith("memory"))
                {
                    long totalMemory = GC.GetTotalMemory(false);
                    print("Total allocated mem: " + (totalMemory / 1048576L).ToString("0") + "mb");
                    return;
                }

                if (text.ToLower().StartsWith("shutdown"))
                {
                    zNet.Shutdown();
                    return;
                }

            }
        }

        public static void print(string text)
        {
            System.Console.WriteLine(text);
        }

        private void Kick(string user)
        {
            if (user.IsNullOrWhiteSpace())
            {
                print("No user found");
                return;
            }

            ZNetPeer znetPeer = zNet.GetPeerByHostName(user);
            if (znetPeer == null)
            {
                znetPeer = zNet.GetPeerByPlayerName(user);
            }
            if (znetPeer != null)
            {
                Kick(znetPeer);
            }
        }
        private void Ban(string user)
        {
            if (user.IsNullOrWhiteSpace())
            {
                return;
            }
            ZNetPeer peerByPlayerName = zNet.GetPeerByPlayerName(user);
            if (peerByPlayerName != null)
            {
                user = peerByPlayerName.m_socket.GetHostName();
            }
            print( "Banning user " + user);
            SyncedList bannedPlayers = (SyncedList) AccessTools.Field(typeof(ZNet), "m_bannedList").GetValue(zNet);
            bannedPlayers.Add(user);
            AccessTools.Field(typeof(ZNet), "m_bannedList").SetValue(zNet, bannedPlayers);
        }

        private void Unban(string user)
        {
            if (user.IsNullOrWhiteSpace())
            {
                return;
            }
            print( "Unbanning user " + user);
            SyncedList bannedPlayers = (SyncedList) AccessTools.Field(typeof(ZNet), "m_bannedList").GetValue(zNet);
            bannedPlayers.Remove(user);
            AccessTools.Field(typeof(ZNet), "m_bannedList").SetValue(zNet, bannedPlayers);
        }

        private void Kick(ZNetPeer peer)
        {
            if (!zNet.IsServer())
            {
                return;
            }
            if (peer != null)
            {
                ZLog.Log("Kicking " + peer.m_playerName);
                SendDisconnect(peer);
            }
        }

        private void Permit(string user)
        {
            if (user.IsNullOrWhiteSpace())
            {
                return;
            }
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

        private void UnPermit(string user)
        {
            if (user.IsNullOrWhiteSpace())
            {
                return;
            }
            print("Removing user from permited user list" + user);
            SyncedList permittedPlayers = (SyncedList)AccessTools.Field(typeof(ZNet), "m_permittedList").GetValue(zNet);
            permittedPlayers.Remove(user);
            AccessTools.Field(typeof(ZNet), "m_permittedList").SetValue(zNet, permittedPlayers);
        }

        private void AddAdmin(string user)
        {
            if (user.IsNullOrWhiteSpace())
            {
                return;
            }
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

        private void RemoveAdmin(string user)
        {
            if (user.IsNullOrWhiteSpace())
            {
                return;
            }
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

        private void SendDisconnect(ZNetPeer peer)
        {
            if (peer.m_rpc != null)
            {
                ZLog.Log("Sent to " + peer.m_socket.GetEndPointString());
                peer.m_rpc.Invoke("Disconnect", Array.Empty<object>());
            }
        }

        private void printBanned()
        {
            print("Ban player steam IDs/IPs:");
            SyncedList ids = (SyncedList) AccessTools.Field(typeof(ZNet), "m_bannedList").GetValue(zNet);
            List<string> idList = ids.GetList();
            foreach(string id in idList)
            {
                print(id);
            }
        }

        private void printPermitted()
        {
            print("Permited player steam IDs/IPs:");
            SyncedList ids = (SyncedList)AccessTools.Field(typeof(ZNet), "m_permittedList").GetValue(zNet);
            List<string> idList = ids.GetList();
            foreach (string id in idList)
            {
                print(id);
            }
        }

        private void printAdmins()
        {
            print("Admin ids:");
            SyncedList ids = (SyncedList)AccessTools.Field(typeof(ZNet), "m_adminList").GetValue(zNet);
            List<string> idList = ids.GetList();
            foreach (string id in idList)
            {
                print(id);
            }
        }
    } 
}
