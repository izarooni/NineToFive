﻿using System;
using System.Linq;
using System.Net;
using log4net;
using NineToFive.Net;
using NineToFive.Net.Interoperations;
using NineToFive.Net.Interoperations.Event;
using NineToFive.Packets;
using NineToFive.SendOps;

namespace NineToFive.Event {
    public class SelectCharEvent : PacketEvent {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SelectCharEvent));

        private uint _playerId;
        private string[] _localMacAddress;          // CLogin::GetLocalMacAddress
        private string[] _localMacAddressWithHddSn; // CLogin::GetLocalMacAddressWithHDDSerialNo
        private string _secondaryPassword;
        private byte[] _remoteAddress;

        public SelectCharEvent(Client client) : base(client) { }

        public override void OnError(Exception e) {
            base.OnError(e);
            Client.Session.Write(GetSelectCharFailed(6));
        }

        public override bool OnProcess(Packet p) {
            p.Position = 0;
            short op = p.ReadShort();
            if (op == (int) ReceiveOperations.Login_OnSelectCharInitSPWPacket) {
                p.ReadByte(); // COutPacket::Encode1(&iPacket, 1);
                _playerId = p.ReadUInt();
                _localMacAddress = p.ReadString().Split(", ");
                _localMacAddressWithHddSn = p.ReadString().Split("_");
                _secondaryPassword = p.ReadString();

                using Packet w = new Packet();
                w.WriteByte((byte) Interoperation.ClientInitializeSPWRequest);
                w.WriteString(Client.Username);
                w.WriteString(_secondaryPassword);
                Interoperability.GetPacketResponse(w.ToArray(), ServerConstants.InterCentralPort, ServerConstants.CentralServer);
            } else {
                if (op == (int) ReceiveOperations.Login_OnSelectCharSPWPacket) {
                    _secondaryPassword = p.ReadString();
                    if (!_secondaryPassword.Equals(Client.SecondaryPassword, StringComparison.Ordinal)) {
                        Client.Session.Write(GetSelectCharFailed(4));
                        return false;
                    }
                }

                _playerId = p.ReadUInt();
                _localMacAddress = p.ReadString().Split(", ");
                _localMacAddressWithHddSn = p.ReadString().Split("_");
            }

            // check if selected playerId exists within the account
            if (Client.Users.FirstOrDefault(u => u.CharacterStat.Id == _playerId) == null) {
                Client.Session.Write(GetSelectCharFailed(5));
                return false;
            }

            {
                // hehe variable scopes
                // send a migration request to the central server to obtain the remote server's IP address
                using Packet w = new Packet();
                w.WriteByte((byte) Interoperation.MigrateClientRequest);
                w.WriteByte(Client.World.Id);
                w.WriteByte(Client.Channel.Id);
                using Packet r = new Packet(Interoperability.GetPacketResponse(w.ToArray(), ServerConstants.InterCentralPort, ServerConstants.CentralServer));
                if (r.ReadBool()) {
                    _remoteAddress = r.ReadBytes(4);
                } else {
                    Client.Session.Write(GetSelectCharFailed(6));
                    Client.Session.Write(CWvsPackets.GetBroadcastMessage(null, false, 1, "Server is unavailable.", null));
                    return false;
                }
            }

            return true;
        }

        public override void OnHandle() {
            Log.Info($"Migrating Client {Client.Id} to {new IPAddress(_remoteAddress)}:{Client.Channel.Port}");

            Client.LoginStatus = 2;
            ClientAuthRequest.RequestClientUpdate(Client);
            Client.Session.Write(GetSelectChar(Client, _playerId, _remoteAddress));
        }

        private static byte[] GetSelectChar(Client client, uint characterId, byte[] address) {
            using Packet p = new Packet();
            p.WriteShort((short) CLogin.OnSelectCharacterResult);
            p.WriteByte();
            p.WriteByte();

            p.WriteBytes(address);                     // uChatIp
            p.WriteShort((short) client.Channel.Port); // uChatPort
            p.WriteUInt(characterId);                  // dwCharacterId
            p.WriteByte(69);                           // bAuthenCode
            p.WriteInt(1337);                          // ulArgument
            return p.ToArray();
        }

        /// <summary>
        /// <para>All possible values for the parameter <paramref name="a"/></para>
        /// <code>6,8,9 for    "Trouble logging in? Try logging in again from maplestory.nexon.net."</code>
        /// <code>2,3 for    "This is an ID that has been deleted or blocked from connection."</code>
        /// <code>4 for    "This is an incorrect password."</code>
        /// <code>5 for    "This is not a registered ID."</code>
        /// <code>7 for    "This is an ID that is already logged in, or the server in under inspection."</code>
        /// <code>10 for    "Could not be processed due to too many connection requests to the server."</code>
        /// <code>11 for    "Only those who are 20 years old or older can use this."</code>
        /// <code>13 for    "Unable to log-on as a master at IP."</code>
        /// <code>14 for    open_web_site(open_web_site(http://www.nexon.net)</code>
        /// <code>15 for    open_web_site(open_web_site(http://www.nexon.net)</code>
        /// <code>16,21 for    "Please verify your account via email in order to play the game."</code>
        /// <code>17 for    "You have either selected the wrong gateway, or you have yet to change your personal information."</code>
        /// <code>25 for    "You're logging in from outside of the service region."</code>
        /// <para>when parameter <paramref name="a"/> is given the value 12, parameter <paramref name="b"/> may have the following values</para>
        /// <code>1 for   "You have entered an incorrect LOGIN ID."</code>
        /// <code>2 for   "You have entered an incorrect form of ID, or your account info hasn't been changed yet."</code>
        /// <code>3 for   "This account has not been verified."</code>
        /// <code>11,13    success if : a != 34</code>
        /// <code>19 for   "An error occurred. Either your IP address is unable to connect or you have used up your available game time."</code>
        /// <code>25 for   "This account has not been verified."</code>
        /// <code>27 for   "PC방 프리미엄 적용 대상이 아닙니다. 넥슨 PC방 고객센터로 문의 비답니다."</code>
        /// <code>28 for   "비정상 접속으로 인해 임시 치단되었습니디."</code>
        /// <code>deafult for   "Trouble logging in? Try logging in again from maplestory.nexon.net."</code>
        /// </summary>
        /// <param name="a">Represents a message popup image in the directory: <code>UI.wz/Login.img/Notice/text</code></param>
        /// <param name="b">Represents a message popup image in the directory: <code>UI.wz/Login.img/Notice/text</code></param>
        private static byte[] GetSelectCharFailed(byte a, byte b = 0) {
            using Packet p = new Packet();
            p.WriteShort((short) CLogin.OnSelectCharacterResult);
            p.WriteByte(a);
            p.WriteByte(b);
            return p.ToArray();
        }
    }
}