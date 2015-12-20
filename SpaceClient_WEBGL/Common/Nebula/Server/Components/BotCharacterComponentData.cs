﻿using Common;
using System;
using ExitGames.Client.Photon;
using ServerClientCommon;
#if UP
using Nebula.Client.UP;
#else
using System.Xml.Linq;
#endif

namespace Nebula.Server.Components {
    public class BotCharacterComponentData : MultiComponentData, IDatabaseComponentData {

        public Workshop workshop { get; private set; }
        public int level { get; private set; }
        public FractionType fraction { get; private set; }

#if UP
        public BotCharacterComponentData(UPXElement e) {
            workshop = (Workshop)Enum.Parse(typeof(Workshop), e.GetString("workshop"));
            level = e.GetInt("level");
            fraction = (FractionType)Enum.Parse(typeof(FractionType), e.GetString("fraction"));
        }
#else
        public BotCharacterComponentData(XElement e) {
            workshop = (Workshop)Enum.Parse(typeof(Workshop), e.GetString("workshop"));
            level = e.GetInt("level");
            fraction = (FractionType)Enum.Parse(typeof(FractionType), e.GetString("fraction"));
        }
#endif

        public BotCharacterComponentData(Workshop workshop, int level, FractionType fraction) {
            this.workshop = workshop;
            this.level = level;
            this.fraction = fraction;
        }

        public BotCharacterComponentData(Hashtable hash) {
            workshop = (Workshop)(byte)hash.GetValue<int>((int)SPC.Workshop, (int)Workshop.Arlen);
            level = hash.GetValue<int>((int)SPC.Level, 0);
            fraction = (FractionType)hash.GetValue<int>((int)SPC.Fraction, (int)FractionType.Friend);
        }

        public override ComponentID componentID {
            get {
                return ComponentID.Character;
            }
        }

        public override ComponentSubType subType {
            get {
                return ComponentSubType.character_bot;
            }
        }

        public Hashtable AsHash() {
            return new Hashtable {
                { (int)SPC.Workshop, (int)workshop },
                { (int)SPC.Level, level },
                { (int)SPC.Fraction, (int)fraction }
            };
        }
    }
}
