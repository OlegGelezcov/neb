using ExitGames.Logging;
using Nebula.Engine;
using Nebula.Game.Components;
using Nebula.Game.Utils;

namespace Nebula.Game.Pets {
    [REQUIRE_COMPONENT(typeof(PetObject))]
    public class PetCharacter : CharacterObject {
        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();
        private PetObject m_PetObject;

        public override int level {
            get {
                if(m_PetObject && m_PetObject.info != null) {
                    return resource.Leveling.LevelForExp(m_PetObject.info.exp);
                }
                return 0;
            }
        }

        public override void SetLevel(int inLevel) {
            s_Log.InfoFormat("SetLevel() don't work on PetCharacter".Color(LogColor.orange));
        }

        public override void Start() {
            base.Start();
            m_PetObject = GetComponent<PetObject>();
        }

        public override void Update(float deltaTime) {
            base.Update(deltaTime);
        }

    }
}
