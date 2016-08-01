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
        private MmoMessageComponent m_Mmo;




        private float m_CurrentEnergy;
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
            m_Mmo = GetComponent<MmoMessageComponent>();

            //energyBuffs = new Dictionary<string, float>();
           
            mEnergyCostPerShiftMoving = nebulaObject.world.Resource().ServerInputs.GetValue<float>("accelerated_motion_energy_cost");
            mMaxEnergyFromResource = resource.ServerInputs.GetValue<float>("max_energy");
            mEnergyRestoration = resource.ServerInputs.GetValue<float>("energy_restoration");
            //log.InfoFormat("MAX EENRGY FROM RESOURCE: {0}, ENERGY RESTORATION: {1}", mMaxEnergyFromResource, mEnergyRestoration);
            mPassiveBonuses = GetComponent<PassiveBonusesComponent>();

            m_CurrentEnergy = maximumEnergy;
        }

        public void Respwan() {
            m_CurrentEnergy = maximumEnergy;
            SendUpdateCurrentEnergy();
        }

        public float currentEnergy {
            get {
                m_CurrentEnergy = Mathf.Clamp(m_CurrentEnergy, 0f, maximumEnergy);
                return m_CurrentEnergy;
            }
        }

        private float m_EnergyFromLastSend = 0f;

        private void SendUpdateCurrentEnergy() {
            float energy = currentEnergy;
            SetCurrentEnergyProperty(energy);
            if (!Mathf.Approximately(m_EnergyFromLastSend, energy)) {
                m_EnergyFromLastSend = energy;
                if (m_Mmo != null) {
                    m_Mmo.SendPropertyUpdate(new System.Collections.Hashtable { { (byte)PS.CurrentEnergy, energy} });
                }
            }
        }



        private float ApplyRestoreEnergyPassiveBonus(float inputRestoreEnergy) {
            if(nebulaObject.IsPlayer()) {
                if(mPassiveBonuses != null && mPassiveBonuses.restoreEnergyTier > 0 ) {
                    return inputRestoreEnergy * (1.0f + mPassiveBonuses.restoreEnergyBonus);
                }
            }
            return inputRestoreEnergy;
        }

        private void SetCurrentEnergyProperty(float val ) {
            nebulaObject.properties.SetProperty((byte)PS.CurrentEnergy, val);
        }
        public override void Update(float deltaTime) {
            if (nebulaObject.IAmBotAndNoPlayers()) {
                return;
            }

            float bonMult = 1.0f;
            float bonAdd = 0f;

            if(mBonuses) {
                bonMult = (1.0f + mBonuses.energyRegenPcBonus);
                bonAdd = mBonuses.energyRegenCntBonus;
            }
            m_CurrentEnergy += deltaTime * ApplyRestoreEnergyPassiveBonus ( mEnergyRestoration * bonMult + bonAdd );

            if(mAI) {
                if(mAI.shiftState.keyPressed) {
                    float costedEnergy = mEnergyCostPerShiftMoving * deltaTime;
                    if( costedEnergy <= m_CurrentEnergy) {

                        //handle god mod for player
                        if(mPlayer) {
                            if(mDamagable.god) {
                                costedEnergy = 0f;
                            }
                        }

                        m_CurrentEnergy -= costedEnergy;
                        //log.InfoFormat("remove {0} energy, current = {1}", costedEnergy, mCurrentEnergy);

                        if(currentEnergy <= 0f) {
                            mAI.shiftState.OnKeyUp();
                        }
                    } else {
                        mAI.shiftState.OnKeyUp();
                    }
                }
            }

            SendUpdateCurrentEnergy();
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

        //public float currentEnergy {
        //    get {
        //        mCurrentEnergy = Mathf.Clamp(mCurrentEnergy, 0f, maximumEnergy);
        //        return mCurrentEnergy;
        //    }
        //}

        public void RestoreEnergy() {
            m_CurrentEnergy = maximumEnergy;
        }


        public void RestoreEnergy(float energy) {
            m_CurrentEnergy += energy;
            m_CurrentEnergy = Mathf.Clamp(m_CurrentEnergy, 0f, maximumEnergy);
            SendUpdateCurrentEnergy();
        }

        public void RemoveEnergy(float en) {
            float modifiedEnergy = en * (1.0f + mBonuses.energyCostPcBonus) + mBonuses.energyCostCntBonus;
            m_CurrentEnergy -= modifiedEnergy;
            m_CurrentEnergy = Mathf.Clamp(m_CurrentEnergy, 0f, maximumEnergy);
            SendUpdateCurrentEnergy();
        }

        public void SetCurrentEnergy(float en) {
            m_CurrentEnergy = Mathf.Clamp(en, 0f, maximumEnergy);
            SendUpdateCurrentEnergy();
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

                float energyBonus = 0f;
                if (mBonuses != null) {
                    energyBonus = mBonuses.maxEnergyCntBonus;
                }

                float shipModelBonus = 0;
                if(mShip != null && mShip.shipModel != null ) {
                    shipModelBonus = mShip.shipModel.energyBonus;
                }

                float result = modelEnergy * (1.0f + shipModelBonus)  + energyBonus;
                if(result < 0f ) {
                    result = 0;
                }
                return result;
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
