using Common;
using Nebula.Server.Components;
using System.Collections.Generic;

namespace Nebula.Game.Components {
    public class RaceableModelComponent : ModelComponent {

        private readonly Dictionary<Race, string> mModels = new Dictionary<Race, string>();
        private RaceableObject mRaceable;
        private MmoMessageComponent mMessage;
        private byte mCurrentRace;
        
             
        public void Init(RaceableModelComponentData data) {
            mModels.Add(Race.Humans, data.humanModel);
            mModels.Add(Race.Borguzands, data.borguzandModel);
            mModels.Add(Race.Criptizoids, data.criptizidModel);
            mModels.Add(Race.None, data.model);
            base.Init(data);
        }

        public override void Start() {
            base.Start();
            mRaceable = GetComponent<RaceableObject>();
            mMessage = GetComponent<MmoMessageComponent>();
            mCurrentRace = mRaceable.race;
        }

        public override void Update(float deltaTime) {
            if(mRaceable.race != mCurrentRace ) {
                mCurrentRace = mRaceable.race;
                SetModelId(mModels[(Race)mCurrentRace]);
                mMessage.SendModelChanged(modelId);
            }
            base.Update(deltaTime);
        }

    }
}
