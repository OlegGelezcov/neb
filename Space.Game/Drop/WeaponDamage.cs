using Common;
using GameMath;

namespace Nebula.Drop {

    public class WeaponDamage {
        private float m_RocketDamage;
        private float m_LaserDamage;
        private float m_AcidDamage;
        private WeaponBaseType m_BaseType;

        public WeaponDamage() {
            SetRocketDamage(0);
            SetLaserDamage(0);
            SetAcidDamage(0);
            SetBaseType(WeaponBaseType.None);
        }

        public WeaponDamage(WeaponBaseType baseType, float rockDmg, float lasDmg, float acidDmg ) {
            SetRocketDamage(rockDmg);
            SetLaserDamage(lasDmg);
            SetAcidDamage(acidDmg);
            SetBaseType(baseType);
        }

        public void ClearAllDamages() {
            SetRocketDamage(0);
            SetLaserDamage(0);
            SetAcidDamage(0);
        }

        public WeaponDamage(WeaponBaseType bt) :
            this(bt, 0, 0, 0) { } 

        public void SetFromDamage(WeaponDamage other) {
            SetRocketDamage(other.rocketDamage);
            SetLaserDamage(other.laserDamage);
            SetAcidDamage(other.acidDamage);
            SetBaseType(other.baseType);
        }

        public void SetRocketDamage(float dmg) {
            m_RocketDamage = dmg;
        }
        public void SetLaserDamage(float dmg) {
            m_LaserDamage = dmg;
        }
        public void SetAcidDamage(float dmg) {
            m_AcidDamage = dmg;
        }
        public void SetBaseType(WeaponBaseType bt) {
            m_BaseType = bt;
        }

        public void SetBaseTypeDamage(float dmg) {
            SetDamage(baseType, dmg);
        }

        public void SetDamage(WeaponBaseType bt, float dmg) {
            switch(bt) {
                case WeaponBaseType.Rocket: {
                        SetRocketDamage(dmg);
                    }
                    break;
                case WeaponBaseType.Laser: {
                        SetLaserDamage(dmg);
                    }
                    break;
                case WeaponBaseType.Acid: {
                        SetAcidDamage(dmg);
                    }
                    break;
                case WeaponBaseType.None: {
                        SetRocketDamage(dmg);
                    }
                    break;
            }
        }

        public void Mult(float val ) {
            m_RocketDamage *= val;
            m_LaserDamage *= val;
            m_AcidDamage *= val;
        }

        public void ClampLess(float val) {
            m_RocketDamage = Mathf.ClampLess(m_RocketDamage, val);
            m_LaserDamage = Mathf.ClampLess(m_LaserDamage, val);
            m_AcidDamage = Mathf.ClampLess(m_AcidDamage, val);
        }

        public void Add(float val, WeaponBaseType bt) {
            switch(bt) {
                case WeaponBaseType.Rocket: {
                        m_RocketDamage += val;
                    }
                    break;
                case WeaponBaseType.Laser: {
                        m_LaserDamage += val;
                    }
                    break;
                case WeaponBaseType.Acid: {
                        m_AcidDamage += val;
                    }
                    break;
                case WeaponBaseType.None: {
                        m_RocketDamage += val;
                    }
                    break;
            }
        }

        public void AddToBase(float val) {
            Add(val, baseType);
        }

        public WeaponBaseType baseType {
            get {
                return m_BaseType;
            }
        }

        public float totalDamage {
            get {
                return rocketDamage + laserDamage + acidDamage;
            }
        }

        public float rocketDamage {
            get {
                return m_RocketDamage;
            }
        }
        public float laserDamage {
            get {
                return m_LaserDamage;
            }
        }
        public float acidDamage {
            get {
                return m_AcidDamage;
            }
        }
    }
}
