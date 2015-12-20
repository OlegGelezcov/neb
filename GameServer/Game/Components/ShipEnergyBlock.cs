// ShipEnergyBlock.cs
// Nebula
//
// Created by Oleg Zheleztsov on Sunday, September 20, 2015 7:01:39 PM
// Copyright (c) 2015 KomarGames. All rights reserved.
//
using Common;
using ExitGames.Logging;
using GameMath;
using Nebula.Engine;
using Nebula.Server.Components;
using Space.Game;
using System;

namespace Nebula.Game.Components {
    //[REQUIRE_COMPONENT(typeof(AIState))]
    [REQUIRE_COMPONENT(typeof(PlayerBonuses))]
    [REQUIRE_COMPONENT(typeof(BaseShip))]
    public class ShipEnergyBlock : NebulaBehaviour
    {
        private static ILogger log = LogManager.GetCurrentClassLogger();

        private BaseShip mShip;
        private AIState mAI;
        private MmoActor mPlayer;
        private ShipBasedDamagableObject mDamagable;
        private PassiveBonusesComponent mPassiveBonuses;



        private float mCurrentEnergy;
        private float mEnergyCostPerShiftMoving;
        private float mMaxEnergyFromResource;
        private float mEnergyRestoration;
        private PlayerBonuses mBonuses;

        public void Init(EnergyComponentData data) { }

        //private Dictionary<string, float> energyBuffs;

        public override void Start() {
            mShip = RequireComponent<BaseShip>();
            mAI = GetComponent<AIState>();
            mPlayer = GetComponent<MmoActor>();
            mDamagable = GetComponent<ShipBasedDamagableObject>();
            mBonuses = GetComponent<PlayerBonuses>();

            //energyBuffs = new Dictionary<string, float>();
           
            mEnergyCostPerShiftMoving = nebulaObject.world.Resource().ServerInputs.GetValue<float>("accelerated_motion_energy_cost");
            mMaxEnergyFromResource = resource.ServerInputs.GetValue<float>("max_energy");
            mEnergyRestoration = resource.ServerInputs.GetValue<float>("energy_restoration");
            //log.InfoFormat("MAX EENRGY FROM RESOURCE: {0}, ENERGY RESTORATION: {1}", mMaxEnergyFromResource, mEnergyRestoration);
            mPassiveBonuses = GetComponent<PassiveBonusesComponent>();

            mCurrentEnergy = maximumEnergy;
        }

        public void Respwan() {
            mCurrentEnergy = maximumEnergy;
        }

        private float ApplyRestoreEnergyPassiveBonus(float inputRestoreEnergy) {
            if(nebulaObject.IsPlayer()) {
                if(mPassiveBonuses != null && mPassiveBonuses.restoreEnergyTier > 0 ) {
                    return inputRestoreEnergy * (1.0f + mPassiveBonuses.restoreEnergyBonus);
                }
            }
            return inputRestoreEnergy;
        }

        public override void Update(float deltaTime) {
            if (nebulaObject.IAmBotAndNoPlayers()) {
                return;
            }

            mCurrentEnergy += deltaTime * ApplyRestoreEnergyPassiveBonus ( mEnergyRestoration );

            if(mAI) {
                if(mAI.shiftState.keyPressed) {
                    float costedEnergy = mEnergyCostPerShiftMoving * deltaTime;
                    if( costedEnergy <= mCurrentEnergy) {

                        //handle god mod for player
                        if(mPlayer) {
                            if(mDamagable.god) {
                                costedEnergy = 0f;
                            }
                        }

                        mCurrentEnergy -= costedEnergy;
                        //log.InfoFormat("remove {0} energy, current = {1}", costedEnergy, mCurrentEnergy);

                        if(currentEnergy <= 0f) {
                            mAI.shiftState.OnKeyUp();
                        }
                    } else {
                        mAI.shiftState.OnKeyUp();
                    }
                }
            }

            nebulaObject.properties.SetProperty((byte)PS.CurrentEnergy, currentEnergy);
            nebulaObject.properties.SetProperty((byte)PS.MaxEnergy, maximumEnergy);

        }

        //public float totalEnergyBuff
        //{
        //    get {
        //        float total = 0.0f;
        //        foreach (var pair in energyBuffs) {
        //            total += pair.Value;
        //        }
        //        return total;
        //    }
        //}

        public float currentEnergy {
            get {
                mCurrentEnergy = Mathf.Clamp(mCurrentEnergy, 0f, maximumEnergy);
                return mCurrentEnergy;
            }
        }

        public void RestoreEnergy() {
            mCurrentEnergy = maximumEnergy;
        }


        public void RestoreEnergy(float energy) {
            mCurrentEnergy += energy;
            mCurrentEnergy = Mathf.Clamp(mCurrentEnergy, 0f, maximumEnergy);
        }

        public void RemoveEnergy(float en) {
            float modifiedEnergy = en * (1.0f + mBonuses.energyCostPcBonus) + mBonuses.energyCostCntBonus;
            mCurrentEnergy -= modifiedEnergy;
            mCurrentEnergy = Mathf.Clamp(mCurrentEnergy, 0f, maximumEnergy);
        }

        //public void SetEnergyBuf(string id, float buf) 
        //{
        //    if (energyBuffs.ContainsKey(id))
        //    {
        //        energyBuffs[id] = buf;
        //    }
        //    else 
        //    {
        //        energyBuffs.Add(id, buf);
        //    }
        //}

        //public void RemoveEnergyBuf(string id) {
        //    energyBuffs.Remove(id);
        //}

        public float maximumEnergy {
            get {
                float modelEnergy = mMaxEnergyFromResource;//100; //mShip.shipModel.BaseEnergyCount();
                modelEnergy = ApplyMaximumEnergyPassiveBonus(modelEnergy);

                float energyBonus = mBonuses.maxEnergyCntBonus;

                return Mathf.Clamp(modelEnergy * mShip.shipModel.energyBonus + energyBonus, 0.0f, Math.Max(0.0f, modelEnergy + energyBonus));
            }
        }

        private float ApplyMaximumEnergyPassiveBonus(float inputMaximumEnergy) {
            if(nebulaObject.IsPlayer()) {
                if(mPassiveBonuses != null && mPassiveBonuses.maxEnergyTier > 0 ) {
                    return inputMaximumEnergy * (1.0f + mPassiveBonuses.maxEnergyBonus);
                }
            }
            return inputMaximumEnergy;
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.Energy;
            }
        }
    }
}
