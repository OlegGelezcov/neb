// AudioCache.cs
// Nebula
//
// Created by Oleg Zheleztsov on Thursday, September 10, 2015 10:13:05 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
namespace Nebula.Resources {
    using UnityEngine;
    using System.Collections;
    using Common;

    public static class AudioCache {

        private static readonly ObjectSubCache<AudioClip, Race> sMenuClips = new ObjectSubCache<AudioClip, Race>();
        private static readonly ObjectSubCache<AudioClip, Race> sAmbientClips = new ObjectSubCache<AudioClip, Race>();
        private static readonly ObjectSubCache<AudioClip, UISoundType> sUIClips = new ObjectSubCache<AudioClip, UISoundType>();

        private static readonly ObjectSubCache<AudioClip, GameEffectSoundType> sHumansEffectClips = new ObjectSubCache<AudioClip, GameEffectSoundType>();
        private static readonly ObjectSubCache<AudioClip, GameEffectSoundType> sBorguzandsEffectClips = new ObjectSubCache<AudioClip, GameEffectSoundType>();
        private static readonly ObjectSubCache<AudioClip, GameEffectSoundType> sCriptizidsEffectClips = new ObjectSubCache<AudioClip, GameEffectSoundType>();
        private static readonly ObjectSubCache<AudioClip, GameEffectSoundType> sCommonEffects = new ObjectSubCache<AudioClip, GameEffectSoundType>();

        public static AudioClip AmbientRaceClip(Race race) {
            switch (race) {
                case Race.Humans:
                    return sAmbientClips.GetObject(race, "Audio/Humans/Ambient/ambient");
                case Race.Borguzands:
                    return sAmbientClips.GetObject(race, "Audio/Borguzands/Ambient/ambient");
                case Race.Criptizoids:
                    return sAmbientClips.GetObject(race, "Audio/Criptizids/Ambient/ambient");
                default:
                    Debug.Log("None race - not found ambient sound. Use for humans");
                    return sAmbientClips.GetObject(race, "Audio/Humans/Ambient/ambient");
            }
        }

        public static AudioClip MenuRaceClip(Race race) {
            switch (race) {
                case Race.Humans:
                    return sMenuClips.GetObject(race, "Audio/Humans/Menu/menu");
                case Race.Borguzands:
                    return sMenuClips.GetObject(race, "Audio/Borguzands/Menu/menu");
                case Race.Criptizoids:
                    return sMenuClips.GetObject(race, "Audio/Criptizids/Menu/menu");
                default:
                    Debug.Log("None race - nt found menu sound. Use for humans");
                    return sMenuClips.GetObject(race, "Audio/Humans/Menu/menu");
            }
        }

        public static AudioClip Effect(Race race, GameEffectSoundType effectType) {
            switch (effectType) {
                case GameEffectSoundType.WeaponStart:
                    return EffectClips(race).GetObject(effectType, WeaponEffectPath(race));
                case GameEffectSoundType.Explosion:
                    return EffectClips(race).GetObject(effectType, ExplosionEffectPath(race));
                case GameEffectSoundType.Warp:
                    return sCommonEffects.GetObject(GameEffectSoundType.Warp, "Audio/warp");
                case GameEffectSoundType.CollectAsteroid:
                    return sCommonEffects.GetObject(GameEffectSoundType.CollectAsteroid, "Audio/collect_asteroid");
                case GameEffectSoundType.Die:
                    return sCommonEffects.GetObject(GameEffectSoundType.Die, "Audio/die");
                case GameEffectSoundType.None:
                    return null;
                default:
                    return null;

            }
        }

        private static ObjectSubCache<AudioClip, GameEffectSoundType> EffectClips(Race race) {
            switch (race) {
                case Race.Humans:
                    return sHumansEffectClips;
                case Race.Borguzands:
                    return sBorguzandsEffectClips;
                case Race.Criptizoids:
                    return sCriptizidsEffectClips;
                default:
                    return sHumansEffectClips;
            }
        }

        private static string WeaponEffectPath(Race race) {
            switch (race) {
                case Race.Humans:
                    return "Audio/Humans/Weapon/Start/start";
                case Race.Borguzands:
                    return "Audio/Borguzands/Weapon/Start/start";
                case Race.Criptizoids:
                    return "Audio/Criptizids/Weapon/Start/start";
                default:
                    return "Audio/Humans/Weapon/Start/start";
            }
        }

        private static string ExplosionEffectPath(Race race) {
            switch (race) {
                case Race.Humans:
                    return "Audio/Humans/Weapon/Explosion/explosion";
                case Race.Criptizoids:
                    return "Audio/Criptizids/Weapon/Explosion/explosion";
                case Race.Borguzands:
                    return "Audio/Borguzands/Weapon/Explosion/explosion";
                default:
                    return "Audio/Humans/Weapon/Explosion/explosion";
            }
        }

        public static AudioClip GetUIClip(UISoundType type) {
            switch (type) {
                case UISoundType.ButtonClick:
                    return sUIClips.GetObject(type, "Audio/UI/button");
                case UISoundType.Notification:
                    return sUIClips.GetObject(type, "Audio/UI/notification");
                case UISoundType.Panel:
                    return sUIClips.GetObject(type, "Audio/UI/panel");
                default:
                    return null;
            }
        }
    }
}
