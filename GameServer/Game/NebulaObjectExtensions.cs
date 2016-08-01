// NebulaObjectExtensions.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 20, 2015 6:58:50 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using Nebula.Engine;
using Nebula.Game.Components;
using ServerClientCommon;
using Space.Game;
using System.Collections;

namespace Nebula.Game {
    public static class NebulaObjectExtensions {
        public static bool IsPlayer(this NebulaObject nebulaObject) {
            return (nebulaObject.Type == (byte)ItemType.Avatar);
        }
        public static bool IsBot(this NebulaObject nebulaObject) {
            return (nebulaObject.Type == (byte)ItemType.Bot);
        }
        public static PlayerSkills Skills(this NebulaObject nebulaObject) {
            return nebulaObject.GetComponent<PlayerSkills>();
        }
        public static DamagableObject Damagable(this NebulaObject nebulaObject) {
            return nebulaObject.GetComponent<DamagableObject>();
        }

        public static RaceableObject Raceable(this NebulaObject nebulaObject) {
            return nebulaObject.GetComponent<RaceableObject>();
        }

        public static MmoWorld mmoWorld(this NebulaObject nebulaObject) {
            return nebulaObject.world as MmoWorld;
        }

        public static CharacterObject Character(this NebulaObject nebulaObject) {
            return nebulaObject.GetComponent<CharacterObject>();
        }

        public static PlayerBonuses Bonuses(this NebulaObject nebulaObject) {
            return nebulaObject.GetComponent<PlayerBonuses>();
        }

        public static BaseWeapon Weapon(this NebulaObject nebulaObject) {
            return nebulaObject.GetComponent<BaseWeapon>();
        }

        public static PlayerShip PlayerShip(this NebulaObject nebulaObject) {
            return nebulaObject.GetComponent<PlayerShip>();
        }

        public static MmoMessageComponent MmoMessage(this NebulaObject nebulaObject) {
            return nebulaObject.GetComponent<MmoMessageComponent>();
        }

        public static MovableObject Movable(this NebulaObject nebulaObject) {
            return nebulaObject.GetComponent<MovableObject>();
        }

        public static PlayerTarget Target(this NebulaObject nebulaObject) {
            return nebulaObject.GetComponent<PlayerTarget>();
        }

        public static bool IAmBotAndNoPlayers(this NebulaObject nebulaObject) {
            if(nebulaObject.IAmBot()) {
                if(nebulaObject.mmoWorld().playerCountOnStartFrame == 0 ) {
                    return true;
                }
            }
            return false;
        }

        public static bool IAmBot(this NebulaObject nebulaObject) {
            return (nebulaObject.Type != (byte)ItemType.Avatar);
        }

        public static string Color(this string source, string color) {
            return source + " :" + color;
        }

        public static string ObjectString(this NebulaObject obj) {
            ItemType type = (ItemType)obj.Type;
            switch (type) {
                case ItemType.Avatar:
                    return "[player]";
                case ItemType.Bot: {
                        var botObject = obj.GetComponent<BotObject>();
                        if (botObject != null) {
                            return string.Format("[{0}:{1}]", ItemType.Bot, (BotItemSubType)botObject.botSubType);
                        } else {
                            return string.Format("[unknow bot]");
                        }
                    }
                default:
                    return "[" + type.ToString() + "]";
            }
        }

        public static void SetSkillUseState(this Hashtable hash, SkillUseState state ) {
            if(hash.ContainsKey((int)SPC.SkillUseState)) {
                hash.Remove((int)SPC.SkillUseState);
            }
            hash[(int)SPC.SkillUseState] = (byte)state;
        }

        public static void SetMastery(this Hashtable hash, bool val) {
            if(hash.ContainsKey((int)SPC.Mastery)) {
                hash.Remove((int)SPC.Mastery);
            }
            hash[(int)SPC.Mastery] = val;
        }
    }
}
