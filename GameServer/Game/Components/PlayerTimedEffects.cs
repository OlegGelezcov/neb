using Common;
using ExitGames.Logging;
using Nebula.Engine;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System;
using System.Collections;
using Nebula.Database;
using Space.Game;
using Nebula.Game.Utils;

namespace Nebula.Game.Components {
    public class PlayerTimedEffects : NebulaBehaviour, IInfoSource {

        public const float UPDATE_INTERVAL = 5f;

        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<TimedEffectType, TimedEffect> m_Effects = new ConcurrentDictionary<TimedEffectType, TimedEffect>();
        private readonly List<TimedEffectType> m_InvalidEffects = new List<TimedEffectType>();
        private float m_Timer = UPDATE_INTERVAL;

        public override int behaviourId {
            get {
                return (int)ComponentID.PlayerTimedEffects;
            }
        }

        public void Load() {
            s_Log.InfoFormat("Load: timed effects".Color(LogColor.orange));
            bool isNew = false;
            Hashtable save = TimedEffectsDatabase.instance.LoadTimedEffects(GetComponent<PlayerCharacterObject>().characterId, nebulaObject.resource as Res, out isNew);
            if(save != null ) {
                foreach(DictionaryEntry entry in save ) {
                    switch((TimedEffectType)(int)entry.Key) {
                        case TimedEffectType.exp: {
                                Hashtable expEffectSave = entry.Value as Hashtable;
                                if(expEffectSave != null ) {
                                    ExpTimedEffect effect = new ExpTimedEffect();
                                    effect.ParseInfo(expEffectSave);
                                    m_Effects.TryAdd(effect.type, effect);
                                    s_Log.InfoFormat("loaded: " + effect.ToString().Color(LogColor.orange));
                                }
                            }

                            break;
                    }
                }
            }


            CheckEffects();
        }

        public override void Awake() {
            base.Awake();
        }

        public override void Start() {
            base.Start();
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);
            m_Timer -= deltaTime;
            if(m_Timer < 0f ) {
                m_Timer = UPDATE_INTERVAL;
                CheckEffects();
            }
        }

        private void CheckEffects() {
            m_InvalidEffects.Clear();

            foreach(var kvp in m_Effects ) {
                if(kvp.Value.CheckExpire(nebulaObject)) {
                    m_InvalidEffects.Add(kvp.Key);
                }
            }

            TimedEffect effect = null;
            foreach(TimedEffectType type in m_InvalidEffects) {
                if( !m_Effects.TryRemove(type, out effect) ) {
                    s_Log.InfoFormat("unable to remove = {0}", type);
                } else {
                    s_Log.InfoFormat("removed invalid effect: {0}".Color(LogColor.orange), type);
                }
            }
        }

        public bool AddTimedEffect(TimedEffect effect ) {
            bool removedSuccessfully = true;
            if(m_Effects.ContainsKey(effect.type)) {
                TimedEffect oldEffect = null;
                if(m_Effects.TryRemove(effect.type, out oldEffect)) {
                    removedSuccessfully = true;
                } else {
                    removedSuccessfully = false;
                }
            }

            bool addedSuccessfully = true;

            if(removedSuccessfully) {
                addedSuccessfully = m_Effects.TryAdd(effect.type, effect);
            }

            return removedSuccessfully && addedSuccessfully;
        }

        public Hashtable GetInfo() {
            Hashtable save = new Hashtable();
            foreach(var kvp in m_Effects) {
                save.Add((int)kvp.Key, kvp.Value.GetInfo());
            }
            return save;
        }
    }
}
