using System;
using Common;
using ExitGames.Logging;
using Nebula.Engine;
using Space.Game;
using Nebula.Server.Components;

namespace Nebula.Game.Components.Activators {

    public abstract class ActivatorObject : NebulaBehaviour {

        private float m_Cooldown;
        private float m_Radius;
        private float m_CooldownTimer;
        private bool m_Interactable;
        protected ActivatorType activatorType { get; private set; }

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public virtual void Init(ActivatorComponentData data) {
            m_Cooldown = data.cooldown;
            m_Radius = data.radius;
            m_CooldownTimer = cooldown;
            activatorType = data.activatorType;
            SetInteractable(true);
        }

        public override int behaviourId {
            get {
                return (int)ComponentID.Activator;
            }
        }

        public override void Update(float deltaTime) {
            nebulaObject.properties.SetProperty((byte)PS.LightCooldown, cooldown);
            nebulaObject.properties.SetProperty((byte)PS.Radius, radius);
            nebulaObject.properties.SetProperty((byte)PS.Interactable, interactable);
            nebulaObject.properties.SetProperty((byte)PS.TypeName, (int)activatorType);

            if(!interactable) {
                if (m_CooldownTimer > 0f) {
                    m_CooldownTimer -= deltaTime;
                }

                if(m_CooldownTimer <= 0f ) {
                    InteractOn();
                }
            }
        }

        protected virtual void InteractOff() {
            SetCooldownTimer(cooldown);
            SetInteractable(false);
            GetComponent<MmoMessageComponent>().SendActivatorEvent(interactable);
        }

        protected virtual void InteractOn() {
            SetInteractable(true);
            GetComponent<MmoMessageComponent>().SendActivatorEvent(interactable);
        }

        protected bool IsDistanceValid(NebulaObject obj) {
            return transform.DistanceTo(obj.transform) < radius;
        }

        /// <summary>
        /// Activate action this activator
        /// </summary>
        /// <param name="source">What object is activator source</param>
        /// <param name="errorCode">Error code or Ok</param>
        /// <returns>Status</returns>
        public abstract void OnActivate(NebulaObject source, out RPCErrorCode errorCode);


        protected void SetCooldownTimer(float interval ) {
            m_CooldownTimer = interval;
        }

        protected void SetInteractable(bool val) {
            m_Interactable = val;
        }


        protected virtual bool interactable {
            get {
                return m_Interactable;
            }
        }

        protected float cooldown {
            get {
                return m_Cooldown;
            }
        }

        public float radius {
            get {
                return m_Radius;
            }
        }


    }
}
