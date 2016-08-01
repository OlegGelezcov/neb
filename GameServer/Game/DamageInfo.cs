using Common;
using System;

namespace Space.Game
{
    public class DamageInfo
    {
        private string _damagerId;
        private byte _damagerType;
        private float _damage;
        private byte mWorkshop;
        private int mLevel;
        private byte mRace;
        private float mTime;


        public DamageInfo(string damagerId, byte damagerType, float damage, byte inWorkshop, int inLevel, byte race) {
            _damagerId = damagerId;
            _damagerType = damagerType;
            _damage = damage;
            mWorkshop = inWorkshop;
            mLevel = inLevel;
            mRace = race;
            mTime = Time.curtime();
        }

        public bool isAvatar {
            get {
                return DamagerType == ItemType.Avatar;
            }
        }

        public string DamagerId {
            get {
                return _damagerId;
            }
        }

        public ItemType DamagerType {
            get {
                return (ItemType)_damagerType;
            }
        }

        public float Damage {
            get
            {
                return _damage;
            }
        }

        public byte workshop {
            get {
                return mWorkshop;
            }
        }

        public int level {
            get {
                return mLevel;
            }
        }

        public byte race {
            get {
                return mRace;
            }
        }

        public void AddDamage(float damage) {
            _damage += damage;
            mTime = Time.curtime();
        }
    }
}
