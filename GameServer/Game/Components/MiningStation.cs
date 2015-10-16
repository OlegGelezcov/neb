using System;
using System.Collections;
using Common;
using Nebula.Engine;
using Nebula.Server.Components;
using ServerClientCommon;
using Space.Game;

namespace Nebula.Game.Components {


    public class MiningStation : NebulaBehaviour, IInfoSource {

        public const float FULL_TIME = 24 * 60 * 60;
        public const float RECEIVE_DAMAGE_NOTIFICATION_INTERVAL = 2 * 60;

        //element id from planet
        private string mNebulaElementID;

        //current count of items getted
        private int mCurrentCount;

        //Max count of items allowed
        private int mMaxCount;

        //Interval to get single element
        private float mTimeToGetSingleElement;

        //planet id of station
        private string mPlanetID;

        //player who setup this station
        private string ownerPlayerID;

        private string mCharacterID;

        private float mTimer = 0f;

        private float mFullTimer = FULL_TIME;

        private int mCurrentTotalCount;

        private int mTotalCount;

        private bool mDestroyed = false;
        private float mLastReceiveDamageNotificationSended = 0;

        public void MakeEmpty() {
            mCurrentCount = 0;
            ResetFullTimer();
        }

        public int currentCount {
            get {
                return mCurrentCount;
            }
        }

        public string nebulaElementID {
            get {
                return mNebulaElementID;
            }
        }

        public bool isFull {
            get {
                return (mCurrentCount >= mMaxCount);
            }
        }

        public string ownedPlayer {
            get {
                return ownerPlayerID;
            }
        }

        public bool hasSpace {
            get {
                return (false == isFull);
            }
        }

        public bool isWornOut {
            get {
                return (mCurrentTotalCount >= mTotalCount);
            }
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.MiningStation;
            }
        }

        public void SendInfoToPlayer(MmoActor player) {
            player.nebulaObject.MmoMessage().ReceiveMiningStation(GetInfo());
        }

        public Hashtable GetInfo() {
            return new Hashtable {
                { (int)SPC.NebulaElementId, mNebulaElementID },
                { (int)SPC.Count, mCurrentCount },
                { (int)SPC.MaxCount, mMaxCount },
                { (int)SPC.Source, mPlanetID },
                { (int)SPC.MiningStationOwnedPlayer, ownerPlayerID },
                { (int)SPC.WarnCount, mCurrentTotalCount },
                { (int)SPC.MaxWarnCount, mTotalCount },
                { (int)SPC.ItemId , nebulaObject.Id },
                { (int)SPC.ItemType, nebulaObject.Type }
            };
        }

        public void Init(MiningStationComponentData data) {
            mNebulaElementID = data.nebulaElementID;
            mMaxCount = data.maxCount;
            mTimeToGetSingleElement = data.timeToGetSingleElement;
            mPlanetID = data.sourceID;
            ownerPlayerID = data.ownedPlayerID;
            mCurrentCount = 0;
            mTimer = mTimeToGetSingleElement;
            mTotalCount = data.totalCount;
            mCurrentTotalCount = 0;
            props.SetProperty((byte)PS.DataId, ownerPlayerID);
            mCharacterID = data.characterID;
        }

        public override void Start() {
            
        }

        public override void Update(float deltaTime) {

            if (hasSpace && (!isWornOut)) {
                mTimer -= deltaTime;
                if (mTimer <= 0f) {
                    mTimer = mTimeToGetSingleElement;
                    mCurrentCount++;
                    mCurrentTotalCount++;
                    ResetFullTimer();
                }
            } 

            if(isFull || isWornAndNotEmpty) {
                mFullTimer -= deltaTime;
                if(mFullTimer <= 0f ) {
                    DestroyStation();
                }
            }

            if(isWornAndEmpty) {
                DestroyStation();
            } 


        }

        private bool isWornAndEmpty {
            get {
                return isWornOut && mCurrentCount == 0;
            }
        }

        private bool isWornAndNotEmpty {
            get {
                return (isWornOut && (mCurrentCount > 0));
            }
        }

        private void ResetFullTimer() {
            mFullTimer = FULL_TIME;
        }

        private bool hasPlanet {
            get {
                return (false == string.IsNullOrEmpty(mPlanetID));
            }
        }

        public void Death() {
            DestroyStation();
        }

        //called when receive damage,
        //at new damage every interval send notification via notification service when my mining station were attacked
        public void OnNewDamage(DamageInfo damager) {
            if ((Time.curtime() - mLastReceiveDamageNotificationSended) >= RECEIVE_DAMAGE_NOTIFICATION_INTERVAL) {
                mLastReceiveDamageNotificationSended = Time.curtime();

                if (false == string.IsNullOrEmpty(mCharacterID)) {
                    GameApplication.Instance.updater.CallS2SMethod(NebulaCommon.ServerType.SelectCharacter, "MiningStationUnderAttackNotification",
                        new object[] { mCharacterID, nebulaObject.mmoWorld().Zone.Id });
                }
            }
        }

        private void DestroyStation() {
            if (!mDestroyed) {
                mDestroyed = true;

                if (hasPlanet) {
                    var item = nebulaObject.mmoWorld().GetItem((it) => {
                        if (it.Id == mPlanetID && it.Type == (byte)ItemType.Bot) {
                            return true;
                        }
                        return false;
                    });
                    if (item != null) {
                        var planetComponent = item.GetComponent<PlanetObject>();
                        if (planetComponent) {
                            planetComponent.FreeStationSlot(nebulaObject.Id);
                        }
                    }
                    mPlanetID = string.Empty;
                }

                (nebulaObject as GameObject).Destroy();
            }
        }

    }
}
