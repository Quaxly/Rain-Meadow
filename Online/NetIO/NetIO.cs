using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace RainMeadow
{
    public abstract class NetIO
    {
        public enum SendType : byte
        {
            Reliable,
            Unreliable,
        }

        public virtual void SendSessionData(OnlinePlayer toPlayer)
        {
            try
            {
                OnlineManager.serializer.WriteData(toPlayer);
                SendP2P(toPlayer, new SessionPacket(OnlineManager.serializer.buffer, (ushort)OnlineManager.serializer.Position), SendType.Unreliable);
                OnlineManager.serializer.EndWrite();
            }
            catch (Exception e)
            {
                RainMeadow.Error(e);
                OnlineManager.serializer.EndWrite();
                throw;
            }

        }

        public virtual void SendCustomData(OnlinePlayer toPlayer, CustomPacket customPacket, SendType sendType)
        {
            try
            {
                SendP2P(toPlayer, customPacket, sendType);
            }
            catch (Exception e)
            {
                RainMeadow.Error(e);
                throw;
            }

        }

        public virtual void ForgetPlayer(OnlinePlayer player) {}
        public virtual void ForgetEverything() {}

        // If using a domain requires you to start a conversation, then any packet sent before before starting a conversation is ignored.
        // otherwise, the parameter "start_conversation" is ignored.
        public abstract void SendP2P(OnlinePlayer player, Packet packet, SendType sendType, bool start_conversation = false);

        // public void SendP2P(OnlinePlayer player, Packet packet, SendType sendType)
        // {
        //     var localPlayerId = player.id as LocalMatchmakingManager.LocalPlayerId;
        //     MemoryStream memory = new MemoryStream(128);
        //     BinaryWriter writer = new BinaryWriter(memory);

        //     Packet.Encode(packet, writer, player);

        //     byte[] bytes = memory.GetBuffer();

        //     UdpPeer.Send(localPlayerId.endPoint, bytes, (int)memory.Position,
        //         sendType switch
        //         {
        //             SendType.Reliable => UdpPeer.PacketType.Reliable,
        //             SendType.Unreliable => UdpPeer.PacketType.Unreliable,
        //             _ => UdpPeer.PacketType.Unreliable,
        //         });
        // }

        public virtual void Update()
        {
               RecieveData();
        }

        public abstract void RecieveData();

        // public void ReceiveDataLocal()
        // {
        //     UdpPeer.Update();

        //     while (UdpPeer.IsPacketAvailable())
        //     {
        //         try
        //         {
        //             //RainMeadow.Debug("To read: " + UdpPeer.debugClient.Available);
        //             if (!UdpPeer.Read(out BinaryReader netReader, out IPEndPoint remoteEndpoint))
        //                 continue;
        //             if (netReader.BaseStream.Position == ((MemoryStream)netReader.BaseStream).Length) continue; // nothing to read somehow?
        //             var player = (MatchmakingManager.instance as LocalMatchmakingManager).GetPlayerLocal(remoteEndpoint.Port);
        //             if (player == null)
        //             {
        //                 RainMeadow.Debug("Player not found! Instantiating new at: " + remoteEndpoint.Port);
        //                 player = new OnlinePlayer(new LocalMatchmakingManager.LocalPlayerId(remoteEndpoint.Port, remoteEndpoint, remoteEndpoint.Port == UdpPeer.STARTING_PORT));
        //             }

        //             Packet.Decode(netReader, player);
        //         }
        //         catch (Exception e)
        //         {
        //             RainMeadow.Error(e);
        //             OnlineManager.serializer.EndRead();
        //         }
        //     }
        // }

        // public void ReceiveDataSteam()
        // {
        //     lock (OnlineManager.serializer)
        //     {
        //         int n;
        //         IntPtr[] messages = new IntPtr[32];
        //         do // process in batches
        //         {
        //             n = SteamNetworkingMessages.ReceiveMessagesOnChannel(0, messages, messages.Length);
        //             for (int i = 0; i < n; i++)
        //             {
        //                 var message = SteamNetworkingMessage_t.FromIntPtr(messages[i]);
        //                 try
        //                 {
        //                     if (OnlineManager.lobby != null)
        //                     {

        //                         var fromPlayer = (MatchmakingManager.instance as SteamMatchmakingManager).GetPlayerSteam(message.m_identityPeer.GetSteamID().m_SteamID);
        //                         if (fromPlayer == null)
        //                         {
        //                             RainMeadow.Error("player not found: " + message.m_identityPeer + " " + message.m_identityPeer.GetSteamID());
        //                             continue;
        //                         }
        //                         //RainMeadow.Debug($"Receiving message from {fromPlayer}");
        //                         Marshal.Copy(message.m_pData, OnlineManager.serializer.buffer, 0, message.m_cbSize);
        //                         OnlineManager.serializer.ReadData(fromPlayer, message.m_cbSize);
        //                     }
        //                 }
        //                 catch (Exception e)
        //                 {
        //                     RainMeadow.Error("Error reading packet from player : " + message.m_identityPeer.GetSteamID());
        //                     RainMeadow.Error(e);
        //                     OnlineManager.serializer.EndRead();
        //                     //throw;
        //                 }
        //                 finally
        //                 {
        //                     SteamNetworkingMessage_t.Release(messages[i]);
        //                 }
        //             }
        //         }
        //         while (n > 0);
        //     }
        // }

 
    }
}
