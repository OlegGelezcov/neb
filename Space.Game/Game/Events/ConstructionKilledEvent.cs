using Nebula.Engine;

namespace Nebula.Game.Events {
    public class ConstructionKilledEvent : BaseEvent {
        public ConstructionKilledEvent(NebulaObject source)
            : base(Common.EventType.ConstructionKilled, source) { }
    }
}
