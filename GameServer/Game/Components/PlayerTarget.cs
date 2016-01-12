using Common;
using ExitGames.Logging;
using Nebula.Engine;
using Nebula.Server.Components;
using Space.Game;
using System.Collections;
using System.Collections.Concurrent;

namespace Nebula.Game.Components {
    public class PlayerTarget : NebulaBehaviour, IInfoSource, IDatabaseObject
    {

        private static ILogger log = LogManager.GetCurrentClassLogger();

        public bool hasTarget { get; private set; }
        public string targetId { get; private set; }
        public byte targetType { get; private set; }
        public NebulaObject targetObject { get; private set; }

        private readonly ConcurrentDictionary<string, NebulaObject> mSubscribers = new ConcurrentDictionary<string, NebulaObject>();

        public bool inCombat { get; private set; } = false;
        private float inCombatTimer = -1f;

        private MmoActor mPlayer;
        private IDatabaseComponentData mInitData;

        public void Init(TargetComponentData data) {
            mInitData = data;
        }

        public bool noTarget {
            get {
                return (false == hasTarget);
            }
        }

        public bool targetIsEnemySubscriber {
            get {
                if(targetObject) {
                    if(mSubscribers.ContainsKey(targetObject.Id)) {
                        var targetCharacter = targetObject.Character();
                        if (targetCharacter) {
                            var meCharacter = nebulaObject.Character();
                            var relation = meCharacter.RelationTo(targetCharacter);
                            return (relation == FractionRelation.Enemy) || (relation == FractionRelation.Neutral);
                        }
                    }
                }
                return false;
            }
        }

        public override void Start() {
            hasTarget = false;
            targetId = string.Empty;
            targetType = (byte)ItemType.Avatar;
            UnsubscribeFromTarget();
            mPlayer = GetComponent<MmoActor>();
        }

        public NebulaObject anyEnemySubscriber {
            get {
                var meCharacter = GetComponent<CharacterObject>();

                foreach(var pSub in mSubscribers) {
                    var sub = pSub.Value;
                    if(sub && sub.Character() && sub.Target()) {
                        FractionRelation relation = sub.Character().RelationTo(meCharacter);
                        if(relation == FractionRelation.Enemy || relation == FractionRelation.Neutral ) {
                            if (sub.Target().inCombat) {
                                return sub;
                            }
                        }
                    }
                }
                return null;
            }
        }

        public int subscriberCount {
            get {
                return mSubscribers.Count;
            }
        }

        public bool noSubscribers {
            get {
                return subscriberCount == 0;
            }
        }

        public void OnTargetUnsubscribeMe(NebulaObject subscriber) {
            NebulaObject oldSubscriber;
            mSubscribers.TryRemove(subscriber.Id, out oldSubscriber);
            log.InfoFormat("target subscriber removed {0}:{1} yellow", (ItemType)subscriber.Type, subscriber.Id);
        }

        public void OnTargetSubscribeMe(NebulaObject subscriber) {
            
            if(mSubscribers.ContainsKey(subscriber.Id)) {
                NebulaObject oldSubscriber;
                mSubscribers.TryRemove(subscriber.Id, out oldSubscriber);
            }
            mSubscribers.TryAdd(subscriber.Id, subscriber);
            log.InfoFormat("target subscriber added {0}:{1} yellow", (ItemType)subscriber.Type, subscriber.Id);
        }

        public void OnHitMe(NebulaObject whoHit) {
            if(!hasTarget) {
                SetTarget(whoHit.Id, whoHit.Type);
            }
        }

        public override void Update(float deltaTime) {
            if(nebulaObject.IAmBotAndNoPlayers()) {
                return;
            }

            UpdateInCombat(deltaTime);
            SaveProperties();

            if(hasTarget) {
                if( (!targetObject) || (targetObject.invisible) ) {
                    SetTarget(string.Empty, (byte)ItemType.Avatar);
                }
                if(!(nebulaObject.world as MmoWorld).Contains(targetType, targetId)) {
                    SetTarget(string.Empty, (byte)ItemType.Avatar);
                }
            }
        }

        private void SaveProperties() {
            nebulaObject.properties.SetProperty((byte)PS.HasTarget, hasTarget);
            nebulaObject.properties.SetProperty((byte)PS.TargetId, targetId);
            nebulaObject.properties.SetProperty((byte)PS.TargetType, targetType);
            nebulaObject.properties.SetProperty((byte)PS.InCombat, inCombat);
        }

        public Hashtable GetInfo() {
            SaveProperties();
            return new Hashtable {
                { (byte)PS.HasTarget, hasTarget },
                { (byte)PS.TargetId, targetId },
                { (byte)PS.TargetType, targetType }
            };
        }

        public void SetTarget(NebulaObject nebObject) {
            if(nebObject.invisible) {
                Clear();
                return;
            }

            targetId = nebObject.Id;
            targetType = nebObject.Type;
            SubscribeToTarget(nebObject);
            hasTarget = true;
        }

        //set target object to null and send message to target about unsubscribe
        private void UnsubscribeFromTarget() {
            if(targetObject) {
                targetObject.SendMessage(ComponentMessages.OnTargetUnsubscribeMe, nebulaObject);
            }
            targetObject = null;
        }

        //unsubscribe from old and subscribe to new target with sending messages to target
        private void SubscribeToTarget(NebulaObject newTargetObject) {
            UnsubscribeFromTarget();
            targetObject = newTargetObject;
            if(targetObject) {
                targetObject.SendMessage(ComponentMessages.OnTargetSubscribeMe, nebulaObject);
            }
        }

        public void SetTarget(string tId, byte tType) {
            if(string.IsNullOrEmpty(tId)) {
                Clear();               
                SendMmoEvent();
                return;
            }

            if(tId != targetId || (!hasTarget)) {
                NebulaObject newTargetObject = null;
                if(!nebulaObject.world.TryGetObject(tType, tId, out newTargetObject)) {
                    Clear();
                    SendMmoEvent();
                    return;
                }

                if(newTargetObject.invisible) {
                    Clear();
                    SendMmoEvent();
                    return;
                }

                targetId = tId;
                targetType = tType;
                SubscribeToTarget(newTargetObject);
                hasTarget = true;

                if(mPlayer) {
                    log.InfoFormat("player set target {0}, type = {1}, has = {2}", targetId, (ItemType)targetType, hasTarget);
                }
            }

            SendMmoEvent();
        }

        private void SendMmoEvent() {
            if (GetComponent<MmoActor>()) {
                GetComponent<MmoMessageComponent>().SendTargetUpdate();
            }
        }

        public void Clear() {
            targetId = string.Empty;
            targetType = (byte)ItemType.Avatar;
            hasTarget = false;
            UnsubscribeFromTarget();
            SaveProperties();
            if (GetComponent<MmoActor>()) {
                GetComponent<MmoMessageComponent>().SendTargetUpdate();
            }
        }


        public bool IsTarget(string tId, byte tType) {
            return (tId == targetId) && (tType == targetType) && hasTarget;
        }



        public override int behaviourId {
            get {
                return (int)ComponentID.Target;
            }
        }

        private void UpdateInCombat(float deltaTime) {
            if(inCombatTimer > 0 ) {
                inCombatTimer -= deltaTime;
                if(inCombatTimer <= 0f ) {
                    //log.InfoFormat("{0} exit combat green", (ItemType)nebulaObject.Type);
                    if (inCombat) {
                        inCombat = false;
                        //if(nebulaObject.Type == (byte)ItemType.Bot) {
                        //    log.InfoFormat("bot exit from combat state yellow");
                        //    nebulaObject.SendMessage("ExitCombat");
                        //}
                    }

                }
            }
        }

        public void InCombat() {
            //log.InfoFormat("{0} enter combat green", (ItemType)nebulaObject.Type);
            inCombatTimer = 10;
            inCombat = true;
        }

        private void ClearSubscribers() {
            mSubscribers.Clear();
        }

        public void Death() {
            //log.InfoFormat("Reset inCombat on Death()");
            inCombatTimer = -1;
            inCombat = false;
            Clear();
            ClearSubscribers();
        }

        public float MoveDamageToSubscriber(float inputDamage) {

            float movedDamage = inputDamage;
            

            if (nebulaObject.IsPlayer()) {
                var meRaceable = nebulaObject.Raceable();
                foreach (var subscriber in mSubscribers) {
                    if (subscriber.Value) {
                        if (subscriber.Value.IsPlayer()) {
                            if (subscriber.Value.Raceable().race == meRaceable.race) {
                                if (subscriber.Value.Skills().MoveDamageFromAlly(inputDamage, ref movedDamage)) {
                                    return movedDamage;
                                }
                            }
                        }
                    }
                }
            }

            return inputDamage;
        }

        public void OnInvisibilityChanged(bool value) {
            if(value) {
                if(mSubscribers.Count > 0 ) {
                    ConcurrentBag<NebulaObject> currentSubscribers = new ConcurrentBag<NebulaObject>();
                    foreach(var pSubscriber in mSubscribers) {
                        currentSubscribers.Add(pSubscriber.Value);
                    }

                    foreach(var csubscriber in currentSubscribers) {
                        csubscriber.Target().Clear();
                    }
                }

            }
        }

        public Hashtable GetDatabaseSave() {
            if(mInitData != null ) {
                return mInitData.AsHash();
            }
            return new Hashtable();
        }

        public bool IsTarget(string id) {
            if(hasTarget) {
                if(targetId == id ) {
                    return true;
                }
            }
            return false;
        }

        public bool IsNotTarget(string id) {
            return (false == IsTarget(id));
        }
    }
}
