using Common;
using Nebula.Drop;
using Nebula.Engine;

namespace Nebula.Game.Components {

    public class DamageParams {
        public bool reflected { get; set; } = false;
    }

    public struct InputDamage {
        private NebulaObject m_Damager;
        private WeaponDamage m_Damage;

        private Workshop m_Workshop;
        private int m_Level;
        private Race m_Race;
        private DamageParams m_DamageParams;


        public InputDamage(NebulaObject source, WeaponDamage damage, DamageParams damageParams = null) {
            m_Damager = source;
            m_Damage = damage;
            if(m_Damager) {
                var sourceCharacter = source.Character();
                if(sourceCharacter) {
                    m_Workshop = (Workshop)sourceCharacter.workshop;
                    m_Level = sourceCharacter.level;    
                } else {
                    m_Workshop = Workshop.Arlen;
                    m_Level = 1;
                }
                var sourceRaceable = source.Raceable();
                if(sourceRaceable) {
                    m_Race = (Race)sourceRaceable.race;
                } else {
                    m_Race = Race.None;
                }
            }else {
                m_Workshop = Workshop.Arlen;
                m_Level = 1;
                m_Race = Race.None;
            }
            m_DamageParams = damageParams;
        }

        public void SetDamage(WeaponDamage dmg) {
            m_Damage = dmg;
        }

        public NebulaObject source {
            get {
                return m_Damager;
            }
        }

        public WeaponDamage damage {
            get {
                return m_Damage;
            }
        }

        public bool hasDamager {
            get {
                return (true == source);
            }
        }

        public Workshop workshop {
            get {
                return m_Workshop;
            }
        }

        public int level {
            get {
                return m_Level;
            }
        }

        public Race race {
            get {
                return m_Race;
            }
        }

        public string sourceId {
            get {
                if(source) {
                    return source.Id;
                }
                return string.Empty;
            }
        }

        public byte sourceType {
            get {
                if(source) {
                    return source.Type;
                }
                return (byte)ItemType.Bot;
            }
        }

        public bool reflected {
            get {
                if(m_DamageParams != null ) {
                    return m_DamageParams.reflected;
                }
                return false;
            }
        }
    }
}
