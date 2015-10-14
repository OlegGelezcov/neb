using System;
using System.Collections;
using GameMath;
using Nebula.Server.Components;

namespace Nebula.Game.Components {
    public class SimpleMovable : MovableObject, IDatabaseObject {

        private float mSpeed;

        private PlayerBonuses mBonuses;
        private IDatabaseComponentData mInitData;

        public override void Start() {
            base.Start();
            mBonuses = GetComponent<PlayerBonuses>();
        }

        public void Init(SimpleMovableComponentData data) {
            mInitData = data;
            SetSpeed(data.speed);
        }

        public override float normalSpeed {
            get {
                if(stopped) {
                    return 0;
                }
                return mSpeed;
            }
        }

        public override float maximumSpeed {
            get {
                return mSpeed;
            }
        }

        public override float speed {
            get {
                if(stopped) {
                    return 0;
                }
                float result =  normalSpeed;
                if(mBonuses) {
                    result = Mathf.ClampLess(result * (1.0f + mBonuses.speedPcBonus) + mBonuses.speedCntBonus, 0f);
                }
                return result;
            }
        }

        public void SetSpeed(float inSpeed) {
            mSpeed = inSpeed;
        }

        public Hashtable GetDatabaseSave() {
            if(mInitData != null ) {
                return mInitData.AsHash();
            }
            return new Hashtable();
        }
    }
}
