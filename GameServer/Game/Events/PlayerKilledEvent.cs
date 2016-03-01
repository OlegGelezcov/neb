using Nebula.Engine;

namespace Nebula.Game.Events {
    public class PlayerKilledEvent : BaseEvent {
        public PlayerKilledEvent(NebulaObject source)
            : base(Common.EventType.PlayerKilled, source) { }
    }
}
