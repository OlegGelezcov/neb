using ExitGames.Logging;
using GameMath;
using Nebula.Game.Components;
using Nebula.Game.Utils;

namespace Nebula.Game.Pets {
    public class PetMovable : MovableObject {

        private PetObject m_Pet;

        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        public override void Start() {
            base.Start();
            m_Pet = GetComponent<PetObject>();
            //m_OwnerMovable = GetComponent<PetObject>().owner.GetComponent<ShipMovable>();
            //if(m_OwnerMovable) {
            //    s_Log.Info("owner movable cached...".Color(LogColor.red));
            //} else {
            //    s_Log.Info("owner movable not found...".Color(LogColor.red));
            //}
        }

        #region Override MovableObject
        public override float maximumSpeed {
            get {
                return normalSpeed;
            }
        }
        public override void Update(float deltaTime) {
            base.Update(deltaTime);

            //if(m_Pet && m_Pet.owner) {
            //    s_Log.Info("pet owner valid".Color(LogColor.red));
            //} else {
            //    s_Log.Info("pet owner invalid".Color(LogColor.red));
            //}
        }

        public override float normalSpeed {
            get {
                if(stopped) {
                    return 0f;
                }

                if(m_Pet != null && m_Pet.owner != null) {

                    
                    float oSpeed = m_Pet.owner.Movable().speed;
                    if(Mathf.Approximately(oSpeed, 0f)) {
                        return m_Pet.owner.Movable().maximumSpeed;
                    }else {
                        return oSpeed * 1.5f;
                    }
                } else {
                    return 0.1f;
                }
            }
        }

        public override float speed {
            get {

                return normalSpeed;
            }
        } 
        #endregion
    }
}
