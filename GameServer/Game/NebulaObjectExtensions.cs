﻿// NebulaObjectExtensions.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 20, 2015 6:58:50 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using Nebula.Engine;
using Nebula.Game.Components;
using Space.Game;

namespace Nebula.Game {
    public static class NebulaObjectExtensions {
        public static bool IsPlayer(this NebulaObject nebulaObject) {
            return (nebulaObject.Type == (byte)ItemType.Avatar);
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
            if(nebulaObject.Type != (byte)ItemType.Avatar) {
                if(nebulaObject.mmoWorld().playerCountOnStartFrame == 0 ) {
                    return true;
                }
            }
            return false;
        }
    }
}