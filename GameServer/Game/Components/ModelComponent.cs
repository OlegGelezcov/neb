using Common;
using ExitGames.Logging;
using Nebula.Engine;
using Nebula.Server.Components;
using Space.Game;
using System;
using System.Collections;

namespace Nebula.Game.Components {
    public class ModelComponent : NebulaBehaviour, IDatabaseObject {

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public string modelId { get; private set; }

        private ModelComponentData mInitData;

        public override int behaviourId {
            get {
                return (int)ComponentID.Model;
            }
        }

        public void Init(ModelComponentData data) {
            mInitData = data;
            SetModelId(data.model);
        }

        public void SetModelId(string mId) {
            modelId = mId;
            props.SetProperty((byte)PS.Model, modelId);
        }

        public override void Start() {
            if(modelId == null ) {
                throw new Exception("model id must be setted before Start()");
            } else if(modelId == string.Empty) {
                log.InfoFormat("model is empty at Start() on object = {0} at world = {1} yellow", nebulaObject.Id, (nebulaObject.world as MmoWorld).Name);
            }
        }

        public override void Update(float deltaTime) {
            if (nebulaObject.IAmBotAndNoPlayers()) {
                return;
            }
            props.SetProperty((byte)PS.Model, modelId);
        }

        public Hashtable GetDatabaseSave() {
            if(mInitData != null ) {
                return mInitData.AsHash();
            }
            return new Hashtable();
        }

    }
}
