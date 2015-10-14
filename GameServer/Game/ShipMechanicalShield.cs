//namespace Space.Game
//{

//    using Common;
//    using GameMath;
//    using System;
//    using System.Collections;

//    public class ShipMechanicalShield
//    {
//        private float _fullDamageAbsorb;
//        private float _currentDamageAbsorbAllow;
//        private bool _enabled;
//        private MmoActor _owner;

//        public ShipMechanicalShield(MmoActor owner)
//        {
//            _owner = owner;
//        }

//        public void Initialize()
//        {
//            SetMaxDamageAbsorbAllow(1000.0f);
//            SetCurrentDamageAbsorbAllow(1000.0f);
//            SetEnabled(true);
//        }

//        public float FullDamageAbsorbAllow
//        {
//            get
//            {
//                return _fullDamageAbsorb;
//            }
//        }

//        public float CurrentDamageAbsorbAllow
//        {
//            get
//            {
//                return _currentDamageAbsorbAllow;
//            }
//        }

//        public bool Enabled
//        {
//            get { return _enabled; }
//        }


//        /// <summary>
//        /// Add damage to shield and return remainder damage after shield
//        /// </summary>
//        /// <param name="damage"></param>
//        /// <returns></returns>
//        public float AddDamage( float damage )
//        {
//            if (CurrentDamageAbsorbAllow >= damage)
//            {
//                float cur = CurrentDamageAbsorbAllow - damage;
//                SetCurrentDamageAbsorbAllow(cur);
//                return 0.0f;
//            }
//            else
//            {
//                SetCurrentDamageAbsorbAllow(0.0f);
//                return Mathf.Abs(_currentDamageAbsorbAllow - damage);
//            }
//        }

//        public void SetEnabled( bool enabled )
//        {
//            _enabled = enabled;
//            _owner.SetProperty((byte)GPS.MechanicalShieldState, (byte)PS.MechanicalShieldEnabled, _enabled);
//        }

//        public void SetCurrentDamageAbsorbAllow(float value)
//        {
//            _currentDamageAbsorbAllow = value;
//            _owner.SetProperty((byte)GPS.MechanicalShieldState, (byte)PS.MechanicalShieldCurrentDamageAbsorb, _currentDamageAbsorbAllow);
//        }

//        public void SetMaxDamageAbsorbAllow(float value )
//        {
//            _fullDamageAbsorb = value;
//            _owner.SetProperty((byte)GPS.MechanicalShieldState, (byte)PS.MechanicalShieldFullDamageAbsorb, _fullDamageAbsorb);
//        }
//    }
//}
