﻿using System.Collections.Generic;
using NineToFive.Game.Entity.Meta;

namespace NineToFive.Game.Storage.Meta {
    public class ItemSlotBundleData {
        public ItemSlotBundleData(int templateId) {
            TemplateId = templateId;
        }

        public List<short> GetBuffValues() {
            var buffs = new List<short>();
            if (Pad > 0) {
                buffs.Add(Pad);
                BitMask |= SecondaryStat.PAD;
            }

            if (Mad > 0) {
                buffs.Add(Mad);
                BitMask |= SecondaryStat.MAD;
            }

            if (Pdd > 0) {
                buffs.Add(Pdd);
                BitMask |= SecondaryStat.PDD;
            }

            if (Mdd > 0) {
                buffs.Add(Mdd);
                BitMask |= SecondaryStat.MDD;
            }

            if (Acc > 0) {
                buffs.Add(Acc);
                BitMask |= SecondaryStat.ACC;
            }

            if (Eva > 0) {
                buffs.Add(Eva);
                BitMask |= SecondaryStat.EVA;
            }

            if (Speed > 0) {
                buffs.Add(Speed);
                BitMask |= SecondaryStat.Speed;
            }

            if (Jump > 0) {
                buffs.Add(Jump);
                BitMask |= SecondaryStat.Jump;
            }

            return buffs;
        }

        public int TemplateId { get; }
        public SecondaryStat BitMask { get; set; }

        public int SlotMax { get; set; }
        public int Price { get; set; }
        public double UnitPrice { get; set; }
        public int PickUpBlock { get; set; }
        public int TradeBlock { get; set; }
        public int ConsumeOnPickup { get; set; }
        public int NoCancelMouse { get; set; }
        public int ExpireOnLogout { get; set; }
        public int AccountSharable { get; set; }
        public int Quest { get; set; }
        public int Only { get; set; }

        public int EnchantCategory { get; set; }
        public int Success { get; set; }
        public int NotSale { get; set; }
        public int TimeLimited { get; set; }
        public int Morph { get; set; }
        public int MasterLevel { get; set; }
        public int Time { get; set; }

        #region Stats

        public short Hp { get; set; }
        public short Mp { get; set; }
        public short Pad { get; set; }
        public short Mad { get; set; }
        public short Prob { get; set; }
        public short Eva { get; set; }
        public string DefenseAtt { get; set; }
        public short Jump { get; set; }
        public short Acc { get; set; }
        public short Str { get; set; }
        public short Luk { get; set; }
        public short Int { get; set; }
        public short Dex { get; set; }
        public short Pdd { get; set; }
        public short Mdd { get; set; }
        public short Speed { get; set; }

        #endregion

        #region Increase

        public short IncPERIOD { get; set; }
        public short IncPAD { get; set; }
        public short IncMDD { get; set; }
        public short IncACC { get; set; }
        public short IncMHP { get; set; }
        public short Cursed { get; set; }
        public short IncINT { get; set; }
        public short IncDEX { get; set; }
        public short IncMAD { get; set; }
        public short IncEVA { get; set; }
        public short IncSTR { get; set; }
        public short IncLUK { get; set; }
        public short IncSpeed { get; set; }
        public short IncMMP { get; set; }
        public short IncJump { get; set; }
        public short Inc { get; set; }
        public short IncIUC { get; set; }
        public short IncCraft { get; set; }
        public short IncRandVol { get; set; }
        public short Expinc { get; set; }
        public short IncLEV { get; set; }
        public short IncFatigue { get; set; }
        public short IncMaxHP { get; set; }
        public short IncMaxMP { get; set; }
        public short IncReqLevel { get; set; }

        #endregion

        #region Require

        public short ReqLevel { get; set; }
        public short ReqCUC { get; set; }
        public short ReqRUC { get; set; }

        #endregion

        #region Rate

        public short PadRate { get; set; }
        public short MadRate { get; set; }
        public short PddRate { get; set; }
        public short MddRate { get; set; }
        public short AccRate { get; set; }
        public short EvaRate { get; set; }
        public short SpeedRate { get; set; }
        public short MhpRRate { get; set; }
        public short MmpRRate { get; set; }
        public short HpR { get; set; }
        public short MpR { get; set; }
        public short MmpR { get; set; }
        public short MhpR { get; set; }

        #endregion

        #region States

        public int Poison { get; set; }
        public int Darkness { get; set; }
        public int Weakness { get; set; }
        public int Seal { get; set; }
        public int Curse { get; set; }

        #endregion

        public int MaxLevel { get; set; }
        public int Exp { get; set; }
        public int MoveTo { get; set; }
        public int MaxDays { get; set; }
        public int QuestId { get; set; }

        public int RecoveryHP { get; set; }
        public int RecoveryMP { get; set; }
        public int ConsumeHP { get; set; }
        public int ConsumeMP { get; set; }
    }
}