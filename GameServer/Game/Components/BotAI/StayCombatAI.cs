using Nebula.Server.Components;
using Nebula.Server;
using ExitGames.Logging;

namespace Nebula.Game.Components.BotAI {
    public class StayCombatAI : CombatBaseAI {

        private static ILogger log = LogManager.GetCurrentClassLogger();

        public void Init(StayAIComponentData data) {
            base.Init(data);

            SetAIType(new NoneAIType { battleMovingType = data.battleMovingType });
            log.InfoFormat("StayCombatAI.Init(): batlle type = {0}", data.battleMovingType);
        }

        public override void Start() {
            base.Start();
            Move(transform.position, transform.position, transform.position, transform.rotation, 0);
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);
        }

        protected override void OnDoIdle(float deltatTime) {
            
        }
    }
}
