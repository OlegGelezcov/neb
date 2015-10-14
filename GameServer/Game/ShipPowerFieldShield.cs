

//namespace Space.Game
//{

//    using Common;
//    using GameMath;
//    using System;
//    using System.Collections;

//    public class ShipPowerFieldShield
//    {
//        private MmoActor _owner;
//        private float _fieldPower;
//        private float _lastFieldPowerChangedTime;
//        private float _fieldRecoverSpeed;
//        private float _fullDamage;
//        private bool _enabled;


//        public ShipPowerFieldShield(MmoActor owner)
//        {
//            _owner = owner;
//            _fieldPower = 1.0f;
//            _lastFieldPowerChangedTime = Time.curtime();
//            _fieldRecoverSpeed = 1.0f / 60.0f;
//            _fullDamage = 2000.0f;
//            _enabled = true;
//        }

//        public void Initialize()
//        {
//            SetPower(1.0f);
//            _lastFieldPowerChangedTime = Time.curtime();
//            SetRecoverSpeed(1.0f / 60.0f);
//            SetMaxDamageAbsorb(2000.0f);
//            SetEnabled(true);
//        }

//        public void Touch() {
//            UpdatePower();
//        }


//        public void UpdatePower()
//        {
//            //Console.WriteLine("time: {0}  last time: {1}", Time.time, _lastFieldPowerChangedTime);
//            float deltaTime = Time.curtime() - _lastFieldPowerChangedTime;
//            //Console.WriteLine("delta time: {0}", deltaTime);
//            SetPower ( Mathf.Clamp(_fieldPower + _fieldRecoverSpeed * deltaTime, 0.0f, 1.0f) );
//            _lastFieldPowerChangedTime = Time.curtime();
//        }

//        public float Power
//        {
//            get
//            {
//                UpdatePower();
//                return _fieldPower;
//            }
//        }

//        public float PowerInPoints
//        {
//            get
//            {
//                return Power * _fullDamage;
//            }
//        }

//        public bool Enabled
//        {
//            get
//            {
//                return _enabled;
//            }
//        }

//        public float MaxPoints {
//            get {
//                return _fullDamage;
//            }
//        }

//        public float AddDamage( float damage )
//        {
//            //number of points which absorb
//            float damageToDefense = ( _fieldPower ) * _fullDamage;

//            if (damageToDefense >= damage)
//            {
//                damageToDefense -= damage;
//                SetPower(Mathf.Clamp(damageToDefense / _fullDamage, 0.0f, 100.0f));
//                _lastFieldPowerChangedTime = Time.curtime();
//                return 0.0f;
//            }
//            else
//            {
//                SetPower(0.0f);
//                _lastFieldPowerChangedTime = Time.curtime();
//                return Mathf.Abs(damage - damageToDefense);
//            }
//        }

//        public void SetMaxDamageAbsorb(float absorb )
//        {
//            _fullDamage = absorb;
//            SetOwnerProperty((byte)GPS.PowerFieldShieldState, (byte)PS.PowerFieldShieldFullDamageAbsorb, _fullDamage);
//        }

//        public void SetRecoverSpeed(float recoverSpeed )
//        {
//            _fieldRecoverSpeed = recoverSpeed;
//            SetOwnerProperty((byte)GPS.PowerFieldShieldState, (byte)PS.PowerFieldShieldRecoverSpeed, _fieldRecoverSpeed);
//        }

//        public void SetPower(float power )
//        {
//            _fieldPower = power;
//            SetOwnerProperty((byte)GPS.PowerFieldShieldState, (byte)PS.PowerFieldShieldCurrentPowerPercent, _fieldPower);
//        }

//        public void SetEnabled(bool enabled)
//        {
//            _enabled = enabled;
//            SetOwnerProperty((byte)GPS.PowerFieldShieldState, (byte)PS.PowerFieldShieldEnabled, _enabled);
//        }

//        private void SetOwnerProperty(byte group, byte prop, object value) {
//            if (_owner != null) {
//                _owner.SetProperty(group, prop, value);
//            }
//        }
//    }
     
//}
