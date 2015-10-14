using System;
using System.Collections;
using Common;
using Nebula.Engine;
using Nebula.Server.Components;
using Space.Game;

namespace Nebula.Game.Components {
    public class CharacterObject : NebulaBehaviour, IDatabaseObject {
        public byte workshop { get; private set; }
        public virtual int level { get; private set; }
        public int fraction { get; private set; }

        private BotCharacterComponentData mInitData;

        public void Init(BotCharacterComponentData data) {
            mInitData = data;
            SetWorkshop((byte)data.workshop);
            SetLevel( data.level);
            SetFraction(data.fraction);
        }

        public void SetWorkshop(byte inWorkshop) {
            workshop = inWorkshop;
        }

        public void SetWorkshop(Workshop inWorkshop) {
            workshop = (byte)inWorkshop;
        }

        public void SetLevel(int inLevel) {
            level = inLevel;
        }

        public virtual void SetFraction(FractionType inFraction) {
            fraction = (int)inFraction;
        } 

        public override void Update(float deltaTime) {
            if(nebulaObject.IAmBotAndNoPlayers()) {
                return;
            }

            props.SetProperty((byte)PS.Level, level);
            props.SetProperty((byte)PS.Workshop, workshop);
            props.SetProperty((int)PS.Fraction, fraction);
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.Character;
            }
        }

        public FractionRelation RelationTo(CharacterObject other) {
            return resource.fractionResolver.RelationFor((FractionType)fraction).RelationTo((FractionType)other.fraction);
        }

        public Hashtable GetDatabaseSave() {
            if(mInitData != null) {
                return mInitData.AsHash();
            }
            return new Hashtable();
        }
    }
}
