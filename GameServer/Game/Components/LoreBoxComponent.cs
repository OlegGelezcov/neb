using Common;
using Nebula.Engine;
using Nebula.Server.Components;

namespace Nebula.Game.Components {
    public class LoreBoxComponent : NebulaBehaviour {

        private LoreBoxComponentData m_Data;

        public void Init(LoreBoxComponentData data) {
            m_Data = data;
            if(m_Data != null && props != null ) {
                props.SetProperty((byte)PS.DataId, m_Data.loreRecordId);
            }
        }

        public override void Start() {
            base.Start();
            if (m_Data != null && props != null) {
                props.SetProperty((byte)PS.DataId, m_Data.loreRecordId);
            }
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.LoreBox;
            }
        }

        public string recordId {
            get {
                if(m_Data != null ) {
                    return m_Data.loreRecordId;
                }
                return string.Empty;
            }
        }
    }
}
