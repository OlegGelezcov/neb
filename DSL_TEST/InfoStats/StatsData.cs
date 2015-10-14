using Common;
using Nebula.Engine;
using Nebula.Game.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSL_TEST.InfoStats {
    public class StatsData {
        public readonly int level;
        public readonly Workshop workshop;

        private float mDamage;
        private float mSpeed;
        private float mHealth;
        private float mOptimalDistance;
        private float mCriticalChance;
        private float mCriticalDamage;

        //private float mHold;

        private int mCounter;

        public StatsData(Workshop workshop, int level) {
            this.workshop = workshop;
            this.level = level;
            Clear();
        }

        public void Aggregate(NebulaObject nebulaObject) {
            mDamage += nebulaObject.GetComponent<ShipWeapon>().GetDamage(false);
            mSpeed += nebulaObject.GetComponent<ShipMovable>().maximumSpeed;
            mHealth += nebulaObject.GetComponent<ShipBasedDamagableObject>().maximumHealth;
            //mHold += nebulaObject.GetComponent<BotShip>().shipModel.cargo;
            mOptimalDistance += nebulaObject.GetComponent<ShipWeapon>().optimalDistance;
            mCriticalChance += nebulaObject.GetComponent<ShipWeapon>().criticalChance;
            mCriticalDamage += nebulaObject.GetComponent<ShipWeapon>().GetDamage(true);
            mCounter++;
        }

        private void Clear() {
            mDamage = 0f;
            mSpeed = 0f;
            mHealth = 0f;
            mOptimalDistance = 0f;
            mCriticalChance = 0;
            mCriticalDamage = 0;
            mCounter = 0;
        }

        public float averageDamage {
            get {
                if(mCounter == 0) { return 0; }
                return (mDamage / mCounter);
            }
        }

        public float averageSpeed {
            get {
                if (mCounter == 0) { return 0; }
                return (mSpeed / mCounter);
            }
        }

        public float averageHealth {
            get {
                if (mCounter == 0) { return 0; }
                return (mHealth / mCounter);
            }
        }

        public float averageOptimalDistance {
            get {
                if (mCounter == 0) { return 0; }
                return (mOptimalDistance / mCounter);
            }
        }

        public float averageCriticalChance {
            get {
                if(mCounter == 0) { return 0; }
                return (mCriticalChance / mCounter);
            }
        }

        public float averageCriticalDamage {
            get {
                if(mCounter == 0) { return 0; }
                return (mCriticalDamage / mCounter);
            }
        }

        public int damagePercent(float maxDamage) {
            
            return (int)Math.Round(((averageDamage / maxDamage) * 100));
        }

        public int speedPercent(float maxSpeed) {
            return (int)Math.Round(((averageSpeed / maxSpeed) * 100)); 
        }

        public int healthPercent(float maximumHealth) {
            return (int)Math.Round(((averageHealth / maximumHealth) * 100));
        }

        //public int holdPercent(float maximumHold) {
        //    return (int)Math.Round(((averageHold / maximumHold) * 100));
        //}

        public int optimalDistance(float maxOptimalDistance) {
            return (int)Math.Round((averageOptimalDistance / maxOptimalDistance) * 100);
        }

        public  int criticalChance(float maxCriticalChance) {
            return (int)Math.Round((averageCriticalChance / maxCriticalChance) * 100);
        }

        public int criticalDamage(float maxCriticalDamage) {
            return (int)Math.Round((averageCriticalDamage / maxCriticalDamage) * 100);
        }
    }
}
