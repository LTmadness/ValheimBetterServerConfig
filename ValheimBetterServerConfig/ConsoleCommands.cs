using HarmonyLib;
using System;
using System.Collections.Generic;

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
                //print("unpermit [ip/userID] - remove user from permitted user list");
                print("permitted - list permitted users");
            }
            else
            {
                if (text.StartsWith("kick "))
                {
                    string user = text.Substring(5);
                    Kick(user);
                    return;
                }
                if (text.StartsWith("ban "))
                {
                    string user = text.Substring(4);
                    Ban(user);
                    return;
                }
                if (text.StartsWith("unban "))
                {
                    string user = text.Substring(6);
                    Unban(user);
                    return;
                }
                if (text.StartsWith("banned"))
                {
                    printBanned();
                    return;
                }
                if (text.StartsWith("permitted"))
                {
                    printPermitted();
                    return;
                }
                if(text.StartsWith("permit "))
                {
                    string user = text.Substring(7);
                    Permit(user);
                    return;
                }
                /*if(text.StartsWith("unpermit "))
                {
                    string user = text.Substring(7);
                    UnPermit(user);
                    return;
                }*/
            }
        }

        private static void print(string text)
        {
            System.Console.WriteLine(text);
        }

        private void Kick(string user)
        {
            if (user == "")
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
            if (user == "")
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
            if (user == "")
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
            if (user == "")
            {
                return;
            }
            print("Permitting user " + user);
            SyncedList permittedPlayers = (SyncedList)AccessTools.Field(typeof(ZNet), "m_permittedList").GetValue(zNet);
            permittedPlayers.Add(user);
            AccessTools.Field(typeof(ZNet), "m_permittedList").SetValue(zNet, permittedPlayers);
        }

        private void UnPermit(string user)
        {
            if (user == "")
            {
                return;
            }
            print("Removing user from permited user list" + user);
            SyncedList permittedPlayers = (SyncedList)AccessTools.Field(typeof(ZNet), "m_permittedList").GetValue(zNet);
            permittedPlayers.Remove(user);
            AccessTools.Field(typeof(ZNet), "m_permittedList").SetValue(zNet, permittedPlayers);
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
            print("Ban player steam ids:");
            SyncedList ids = (SyncedList) AccessTools.Field(typeof(ZNet), "m_bannedList").GetValue(zNet);
            List<string> idList = ids.GetList();
            foreach(string id in idList)
            {
                print(id);
            }
        }

        private void printPermitted()
        {
            print("Permited player steam ids:");
            SyncedList ids = (SyncedList)AccessTools.Field(typeof(ZNet), "m_permittedList").GetValue(zNet);
            List<string> idList = ids.GetList();
            foreach (string id in idList)
            {
                print(id);
            }
        }
    } 
}
