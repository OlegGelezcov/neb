using Common;
using ExitGames.Logging;
using Nebula.Engine;
using Nebula.Server.Components;
using Space.Game;
using System.Collections;
using System.Collections.Concurrent;
using System;
using ServerClientCommon;
using Nebula.Drop;
using GameMath;
using Nebula.Game.Bonuses;

namespace Nebula.Game.Components {
    public class PlayerTarget : NebulaBehaviour, IInfoSource, IDatabaseObject
    {

        private static ILogger log = LogManager.GetCurrentClassLogger();

        public bool hasTarget { get; private set; }
        public string targetId { get; private set; }
        public byte targetType { get; private set; }
        public NebulaObject targetObject { get; private set; }



        private readonly ConcurrentDictionary<string, NebulaObject> mSubscribers = new ConcurrentDictionary<string, NebulaObject>();
        private readonly PlayerMarkedItem m_MarkedItem = new PlayerMarkedItem();

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

        public void SetMarkedItem(string objId, byte objType) {
            var old = m_MarkedItem.SetMark(objId, objType);
            Hashtable hash = m_MarkedItem.GetInfo();
            hash.Add((int)SPC.Source, nebulaObject.Id);
            hash.Add((int)SPC.PrevId, old.id);
            hash.Add((int)SPC.PrevType, old.type);
            SendPlayerMark(hash);
        }

        public void ClearMarkedItem() {
            var old = m_MarkedItem.Clear();
            Hashtable hash = m_MarkedItem.GetInfo();
            hash.Add((int)SPC.Source, nebulaObject.Id);
            hash.Add((int)SPC.PrevId, old.id);
            hash.Add((int)SPC.PrevType, old.type);
            SendPlayerMark(hash);
        }

        private void SendPlayerMark(Hashtable hash) {
            var world = nebulaObject.mmoWorld();

            var character = nebulaObject.GetComponent<PlayerCharacterObject>();
            if (character != null && character.group != null) {

                if (character.group.members != null) {

                    foreach (var pkvMember in character.group.members) {

                        var member = pkvMember.Value;

                        if (member.gameRefID != nebulaObject.Id) {

                            NebulaObject groupMemberObj;
                            if (world.TryGetObject((byte)ItemType.Avatar, member.gameRefID, out groupMemberObj)) {

                                var memberMmo = groupMemberObj.GetComponent<MmoMessageComponent>();

                                if (memberMmo != null) {

                                    memberMmo.ReceivePlayerMark(hash);
                                }
                            }
                        }
                    }
                }
            }

            var myMmo = nebulaObject.GetComponent<MmoMessageComponent>();
            if (myMmo != null) {
                myMmo.ReceivePlayerMark(hash);
            }
        }

        public override Hashtable DumpHash() {
            var hash = base.DumpHash();
            hash["has_target?"] = hasTarget.ToString();
            hash["target_id"] = (targetId != null) ? targetId : "";
            hash["target_type"] = ((ItemType)targetType).ToString();
            hash["in_combat?"] = inCombat.ToString();
            hash["combat_timer"] = inCombatTimer.ToString();
            hash["no_target?"] = noTarget.ToString();
            hash["subscribers_count"] = mSubscribers.Count.ToString();
            hash["is_target_subscribed_to_me??"] = targetIsEnemySubscriber.ToString();
            hash["exists_enemy_subscriber?"] = ((bool)anyEnemySubscriber).ToString();
            hash["no_subscribers?"] = noSubscribers.ToString();
            return hash;
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
                    if(sub && sub.Character() ) {
                        FractionRelation relation = sub.Character().RelationTo(meCharacter);
                        if(relation == FractionRelation.Enemy || relation == FractionRelation.Neutral ) {
                            return sub;
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
            //log.InfoFormat("target subscriber removed {0}:{1} yellow", (ItemType)subscriber.Type, subscriber.Id);
        }

        public void OnTargetSubscribeMe(NebulaObject subscriber) {
            
            if(mSubscribers.ContainsKey(subscriber.Id)) {
                NebulaObject oldSubscriber;
                mSubscribers.TryRemove(subscriber.Id, out oldSubscriber);
            }
            mSubscribers.TryAdd(subscriber.Id, subscriber);
            //log.InfoFormat("target subscriber added {0}:{1} yellow", (ItemType)subscriber.Type, subscriber.Id);
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

            /*
            if(nebulaObject.IsPlayer()) {
                if(targetObject != null ) {
                    float ang = transform.angleWithDirection(targetObject.transform.position - transform.position) * Mathf.RAD2DEG;
                    log.InfoFormat("angle between us is: {0} degrees", ((int)ang));
                }
            }*/
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
                    //log.InfoFormat("player set target {0}, type = {1}, has = {2}", targetId, (ItemType)targetType, hasTarget);
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

        public void MoveDamageToSubscriber(InputDamage inputDamage) {

            float movedDamage = inputDamage.totalDamage;
            

            if (nebulaObject.IsPlayer()) {
                var meRaceable = nebulaObject.Raceable();
                foreach (var subscriber in mSubscribers) {
                    if (subscriber.Value) {
                        if (subscriber.Value.IsPlayer()) {
                            if (subscriber.Value.Raceable().race == meRaceable.race) {
                                if (subscriber.Value.Skills().MoveDamageFromAlly(inputDamage.totalDamage, ref movedDamage)) {
                                    inputDamage.ClearAllDamages();
                                    inputDamage.SetBaseDamage(movedDamage);
                     
                                }
                            }
                        }
                    }
                }
            }

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

        public void OnBuffSetted(Buff buff, NebulaObject source ) {
            if(source != null && noTarget && (nebulaObject.getItemType() != ItemType.Avatar)) {

                if (BuffUtils.IsDebuff(buff.buffType)) {

                    if (nebulaObject.IsBot()) {
                        var weap = GetComponent<BaseWeapon>();
                        var character = GetComponent<CharacterObject>();
                        var sourceCharacter = source.GetComponent<CharacterObject>();

                        if(weap != null && character != null && sourceCharacter != null) {

                            var relation = character.RelationTo(sourceCharacter);

                            if(relation == FractionRelation.Enemy || relation == FractionRelation.Neutral ) {
                                SetTarget(source);
                            }
                        }
                    }
                }
            }
        }
    }

    public class PlayerMarkedItem : IInfoSource {
        private string m_Id;
        private byte m_Type;

        public PlayerMarkedItem(string id, byte type) {
            m_Id = id;
            m_Type = type;
        }
        public PlayerMarkedItem() {
            Clear();
        }

        public PlayerMarkedItem SetMark(string id, byte type) {
            PlayerMarkedItem old = new PlayerMarkedItem(this.id, this.type);
            m_Id = id;
            m_Type = type;
            return old;
        }

        public PlayerMarkedItem Clear() {
            PlayerMarkedItem old = new PlayerMarkedItem(this.id, this.type);
            m_Id = string.Empty;
            m_Type = (byte)ItemType.None;
            return old;
        }

        public Hashtable GetInfo() {
            return new Hashtable {
                {(int)SPC.Id, id },
                {(int)SPC.Type, type}
            };
        }

        public string id {
            get {
                return m_Id;
            }
        }

        public byte type {
            get {
                return m_Type;
            }
        }
    }
}
