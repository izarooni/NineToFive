﻿using System.Numerics;
using NineToFive.Constants;
using NineToFive.Game.Entity;
using NineToFive.Game.Entity.Meta;
using NineToFive.Net;
using NineToFive.Packets;
using NineToFive.Resources;

namespace NineToFive.Event {
    public class UserSkillUseEvent : PacketEvent {
        private SkillRecord _playerskill;
        private int _skillId;
        private byte _skillLevel;
        private Vector2 _origin;

        public UserSkillUseEvent(Client client) : base(client) { }

        public override bool OnProcess(Packet p) {
            p.ReadInt(); // get_update_time
            _skillId = p.ReadInt();
            _skillLevel = p.ReadByte();

            // if ( is_antirepeat_buff_skill )
            //     _origin = new Vector2(p.ReadShort(), p.ReadShort());
            if (_skillId == (int) Skills.NightlordShadowStars) {
                p.ReadInt(); // Shadow Stars
            }

            Client.User.Skills.TryGetValue(_skillId, out _playerskill);
            return _playerskill?.Level == _skillLevel;
        }

        public override void OnHandle() {
            User user = Client.User;
            if (user.IsDebugging) user.SendMessage($"Skill: {_skillId}, Level: {_playerskill.Level}, Received: {_skillLevel}");

            if (WzCache.Skills.TryGetValue(_playerskill.Id, out var skill)) {
                _playerskill.Proc = true;

                if (user.IsDebugging) {
                    string cts = "CTS : ";
                    foreach (var pair in skill.CTS) {
                        cts += $"{pair.Key}={pair.Value[_playerskill.Level - 1]}, ";
                    }
                    user.SendMessage(cts);
                }
                
                if (skill.Id == (int) Skills.SuperGameMasterHide) {
                    if (user.IsHidden = !user.IsHidden) {
                        user.SendMessage("Now you don't.");
                        Client.Session.Write(CWvsPackets.GetTemporaryStatReset(skill));
                        return;
                    }

                    user.SendMessage("Now you see me.");
                }

                Client.Session.Write(CWvsPackets.GetTemporaryStatSet(skill, _playerskill));
            }

            user.CharacterStat.SendUpdate(0);
        }
    }
}