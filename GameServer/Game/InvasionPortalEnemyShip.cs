namespace Space.Game
{
    /*
    public class InvasionPortalEnemyShip:  IShip 
    {
        private InvasionPortalEnemy _owner;

        private float _delta;
        private Vector3 _position;
        private Quaternion _rotation;
        private float _maxHealth;
        private float _health;
        private int _model;

        private float baseSpeed;
        private float _rotationSpeed;

        private NpcShipWeapon _weapon;
        //private PlayerBonuses _bonuses;

        private float _destroyTime;



        private bool _isBoss;



        private Vector3 targetPos;

        public enum InvStateName { Idle, Fire, Dead, Pursuit }
        private BaseFSM<InvStateName> fsm;
        private int level;
        private Workshop workshop;

        public InvasionPortalEnemyShip(InvasionPortalEnemy owner, float[] position, float[] rotation, bool isBoss, Hashtable evtInfo, Workshop workshop, int level) {
            try
            {
                _owner = owner;

                this.level = level;
                this.workshop = workshop;

                this.fsm = new BaseFSM<InvStateName>();
                this.fsm.AddState(new FSMState<InvStateName>(InvStateName.Idle, Stub, Idle, Stub));
                this.fsm.AddState(new FSMState<InvStateName>(InvStateName.Fire, Stub, Fire, Stub));
                this.fsm.AddState(new FSMState<InvStateName>(InvStateName.Dead, StartDead, Dead, Stub));
                this.fsm.AddState(new FSMState<InvStateName>(InvStateName.Pursuit, Stub, Pursuit, Stub));
                this.fsm.ForceState(InvStateName.Idle);

                _position = position.toVec3(); _rotation = Quaternion.FromEuler(rotation.toVec3());
                GenerateTargetPos();

                _isBoss = isBoss;
                if (false == isBoss)
                {
                    SetMaxHealth((float)evtInfo["ship_health"]);
                    SetHealth(_maxHealth);
                    SetSpeed((float)evtInfo["ship_speed"]);
                    SetModel(Utils.IntRange(3, 14));
                }
                else
                {
                    //generate boss
                    SetMaxHealth((float)evtInfo["ship_health"] * 2.0f);
                    SetHealth(_maxHealth);
                    SetSpeed((float)evtInfo["ship_speed"]);
                    SetModel(15);
                }

                SetAngleSpeed(0.3f);

                DropManager dropManager = DropManager.Get(workshop, Res.Get);
                ObjectColor color = Res.Get.ColorRes.GenColor(Resources.ColoredObjectType.Weapon).color;
                WeaponDropper weaponDropper = dropManager.GetWeaponDropper(level, string.Empty, Difficulty.none, color);

                var weaponObject = weaponDropper.Drop() as WeaponObject;


                _weapon = new NpcShipWeapon(owner);
                //float rangeMin = 0;
                //float rangeMax = _owner.Portal.Event._eventRadius / 3.0f;

                _weapon.Initialize(weaponObject);
                //_bonuses = new PlayerBonuses();
            }
            catch(Exception ex)
            {
                CL.Out(LogFilter.NPC, ex.Message);
                CL.Out(LogFilter.NPC, ex.StackTrace);
            }
        }

        public float MaxSpeed
        {
            get
            {
                return this.baseSpeed * this._owner.Bonuses.MaxSpeedBonusValue(Time.time);
            }
        }


        private float GenerateDamage(bool isBoss) {
            return isBoss ? 10.0f : 5.0f;
        }

        private void Stub(){

            
        }

        private void CheckTargetPos() {
            if (Vector3.Distance(_position, targetPos) < 100.0f) {
                GenerateTargetPos();
            }
        }

        private void GenerateTargetPos() {
            var vec = Utils.RandomVector3(4 * Utils.RadomValue * _owner.Portal.Event.Radius);
            targetPos = _owner.Portal.Event.Center + vec;
        }

        private void Idle() {

            CheckTargetPos();
            Vector3 direction = (targetPos - _position).normalized;
            Vector3 move = direction * this.MaxSpeed * _delta;
            _position += move;
            _rotation = Quaternion.Slerp(_rotation, Quaternion.LookRotation(direction), _rotationSpeed * _delta);
            _owner.Move(_position.toArray(), _rotation.EulerArray, this.Speed());

            FindTarget();
        }

        private void Pursuit()
        {
            ICombatActor target;
            if (_owner.Target.TargetIsICombatActor(out target))
            {
                Vector3 targetPosition = target.Avatar.PositionVec3;
                Vector3 direction = (targetPosition - _position).normalized;
                _position += direction * this.MaxSpeed * _delta;
                _rotation = Quaternion.Slerp(_rotation, Quaternion.Look(direction), _rotationSpeed * _delta);
                _owner.Move(_position.toArray(), _rotation.EulerArray, this.Speed());
                CompleteFire();
                CheckTarget();
            }
            else
            {
                _owner.Target.ResetTarget();
                if(false == this.fsm.IsState(InvStateName.Idle))
                {
                    this.fsm.GotoState(InvStateName.Idle);
                }
            }
        }


        private bool CompleteFire()
        {
            ICombatActor target;
            if (_owner.Target.TargetIsICombatActor(out target))
            {
                if (_weapon.Ready)
                {
                    _weapon.SetLastFireTime(Time.time);
                    SendFire();
                }
                return true;
            }
            else {
                return false;
            }
            
        }
        private void Fire() {
            if (CompleteFire()) {
                CheckTarget();
            }
            else
            {
                _owner.Target.ResetTarget();
                this.fsm.GotoState(InvStateName.Idle);
            }
        }

        private void StartDead() {
            if(_owner.Portal != null )
                _owner.Portal.OnEnemyDestroyed(_owner);

            _destroyTime = Time.time;
            this._owner.OnDeadOccured();
        }
        private void Dead() {
            
            if (Time.time > _destroyTime + 600.0f) {
                _destroyTime = Time.time + 1000000;
                _owner.Destroy();
            }
        }

        public void SetMaxHealth(float maxHealth) {
            _maxHealth = maxHealth;
            _owner.Avatar.SetProperty(GroupProps.SHIP_BASE_STATE, Props.SHIP_BASE_STATE_MAX_HEALTH, _maxHealth);
        }
        public void SetHealth(float health) {
            _health = health;
            if (_health <= 0.0f) {
                _health = 0.0f;
                this.fsm.GotoState(InvStateName.Dead);
                _owner.Avatar.SetProperty(GroupProps.SHIP_BASE_STATE, Props.SHIP_BASE_STATE_DESTROYED, Destroyed);
            }
            _owner.Avatar.SetProperty(GroupProps.SHIP_BASE_STATE, Props.SHIP_BASE_STATE_HEALTH, _health);
        }

        
        public void SetModel(int model) {
            _model = model;
            _owner.Avatar.SetProperty(GroupProps.SHIP_BASE_STATE, Props.SHIP_BASE_STATE_MODEL, _model);
        }

        private void SetSpeed(float speed) 
        {
            this.baseSpeed = speed;
            _owner.Avatar.SetProperty(GroupProps.SHIP_BASE_STATE, Props.SHIP_BASE_STATE_MAX_LINEAR_SPEED, this.MaxSpeed);
            _owner.Avatar.SetProperty(GroupProps.SHIP_BASE_STATE, Props.SHIP_BASE_STATE_CURRENT_LINEAR_SPEED, this.MaxSpeed);
        }

        private void SetAngleSpeed(float angleSpeed)
        {
            _rotationSpeed = angleSpeed;
            _owner.Avatar.SetProperty(GroupProps.SHIP_BASE_STATE, Props.SHIP_BASE_STATE_ANGLE_SPEED, _rotationSpeed);
        }



        public void Update(float delta) 
        {
            //ConsoleLogging.Get.Print("pos: [{0:F1}, {1:F1}, {2:F1}]", _position.X, _position.Y, _position.Z);
            _delta = delta;

            this.fsm.Update();

            if (_health <= 0.0f) 
            {
                if(false == this.fsm.IsState(InvStateName.Dead))
                {
                    this.fsm.GotoState(InvStateName.Dead);
                }
            }
            _owner.Avatar.SetProperty(GroupProps.SHIP_BASE_STATE, Props.SHIP_BASE_STATE_DESTROYED, Destroyed);

            if (_owner.Portal == null || (_owner.Portal.Avatar == null || _owner.Portal.Avatar.Disposed)) {
                _owner.Destroy();
            }
            //ConsoleLogging.Get.Print("postion: {0}", _position);
        }

        public bool Destroyed {
            get {
                return this.fsm.IsState(InvStateName.Dead);
            }
        }


        public IShipWeapon GetWeapon
        {
            get { return _weapon; }
        }

        public float Health {
            get {
                return _health;
            }
        }


        public Vector3 Position {
            get
            {
                return _position;
            }
        }

        public Quaternion Rotation {
            get {
                return _rotation;
            }
        }

        private void FindTarget()
        {
            try
            {

                Item result;
                if (((MmoWorld)_owner.World).ItemCache.TryGetItem((byte)ItemType.Avatar, _owner.Portal.Event.Center, _owner.Portal.Event.Radius, (it) =>
                {
                    MmoItem mit = it as MmoItem;
                    return (mit.Disposed == false && mit.Owner.ShipDestroyed == false) ? true : false;
                }, out result)) {
                    //_owner.GetTarget.SetTarget(result.Id, result.Type, true);
                    //ConsoleLogging.Get.Print("target with id: {0} founded", result.Id);
                    _owner.Target.SetTarget(result);
                    //ConsoleLogging.Get.Print("target founded");
                    CheckTarget();
                    return;
                }
                //ConsoleLogging.Get.Print("find target not founded");
            }
            catch (Exception ex)
            {
                //ConsoleLogging.Get.Print(LogFilter.ALL, "exception: {0}".f( ex.Message));
                //ConsoleLogging.Get.Print(LogFilter.ALL, ex.StackTrace);
            }
        }

        private void CheckTarget()
        {
            //ConsoleLogging.Get.Print("CheckTarget() called");
            ICombatActor target;
            if ( _owner.Target.TargetIsICombatActor(out target))
            {
                //ConsoleLogging.Get.Print("ok, target is combat");
                float distance2Me = Vector3.Distance(target.Avatar.PositionVec3, _position);
                float distance2Center = Vector3.Distance(target.Avatar.PositionVec3, _owner.Portal.Event.Center);
                float END_PURSUIT_DIST = _weapon.Distance * 1.5f;
                float START_PURSUIT_DIST = _weapon.Distance;

                switch (this.fsm.CurrentState.Name) { 
                    case InvStateName.Fire:
                        
                        if (distance2Me > START_PURSUIT_DIST && distance2Me <= END_PURSUIT_DIST ) 
                        {
                            this.fsm.GotoState(InvStateName.Pursuit);
                        }
                        else if (distance2Me > END_PURSUIT_DIST)
                        {
                            this.fsm.GotoState(InvStateName.Idle);
                        }
                        break;
                    case InvStateName.Pursuit:
                        if (distance2Me <= _weapon.Distance) 
                        {
                            this.fsm.GotoState(InvStateName.Fire);
                        }
                        else if (distance2Me > END_PURSUIT_DIST)
                        {
                            this.fsm.GotoState(InvStateName.Idle);
                        }
                        break;
                    case InvStateName.Idle:
                        //ConsoleLogging.Get.Print("now idle state");
                        //ConsoleLogging.Get.Print("dist2me: {0}, optimal distance for weapon: {1}", distance2Me, _weapon.OptimalDistance);
                        if (distance2Me <= _weapon.Distance)
                        {
                            this.fsm.GotoState(InvStateName.Fire);
                        }
                        else if( distance2Me  >= START_PURSUIT_DIST && distance2Me < END_PURSUIT_DIST  )
                        {
                            this.fsm.GotoState(InvStateName.Pursuit);
                        }
                        break;
                }
            }
            else 
            {
                if(false == this.fsm.IsState(InvStateName.Idle))
                {
                    this.fsm.GotoState(InvStateName.Idle);
                }
            }
        }

        private float DistanceTo(Item item) {
            return Vector3.Distance(item.PositionArr.toVec3(), _position);
        }

        private void SendFire()
        {
            CombatGameObject.SendFire(this._owner);
        }



        public float GetShipEnergy()
        {
            return 1000;
        }

        public void SetShipEnergy(float energy)
        {
            
        }

        public bool IsBoss {
            get {
                return _isBoss;
            }
        }


        public float Damage
        {
            get { return _weapon.Damage; }
        }


        public float Speed()
        {
            return this.MaxSpeed;
        }


        public void Touch()
        {
            //throw new NotImplementedException();
        }

        public float MaxHealth
        {
            get
            {
                return this._maxHealth;
            }
        }



        public PlayerShipEnergyBlock Energy
        {
            get { return this._owner.EnergyBlock; }
        }

        public float ResistDamagePercent
        {
            get
            {
                return 0.0f;
            }
        }

        public float CurrentHealth
        {
            get
            {
                return this._health;
            }
        }


        public float TotalEnergy
        {
            get { return 100; }
        }

        public float TotalEnergyRestoration
        {
            get { return 10; }
        }
    }
     */
}
