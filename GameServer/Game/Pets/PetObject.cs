using Common;
using ExitGames.Logging;
using GameMath;
using Nebula.Engine;
using Nebula.Game.Components;
using Nebula.Game.Utils;
using Nebula.Pets;
using Space.Server;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nebula.Game.Pets {
    public class PetObject : NebulaBehaviour {

        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        private const float kSkillUpdateInterval = 4;

        private const float UPDATE_OFFSET_COOLDOWN = 30;
        private const float MIN_DISTANCE = 1f;
        private const float MAX_DISTANCE = 20f;
        private const float ROTATION_SPEED = 0.5f;
        public const float OFFSET_RADIUS = 10;

        private NebulaObject m_Owner;
        private PetInfo m_Info;

        private MmoMessageComponent m_Message;
        private Vector3 m_Offset;
        private float m_OffsetChangeTimer = UPDATE_OFFSET_COOLDOWN;
        private MovableObject m_Movable;
        private readonly List<PetSkill> m_Skills = new List<PetSkill>();
        private float m_SkillUpdateTimer = kSkillUpdateInterval;

        public class PetObjectInitData {
            private NebulaObject m_Owner;
            private PetInfo m_Info;

            public PetObjectInitData(NebulaObject owner, PetInfo info) {
                m_Owner = owner;
                m_Info = info;
            }

            public NebulaObject owner {
                get {
                    return m_Owner;
                }
            }

            public PetInfo info {
                get {
                    return m_Info;
                }
            }
        }

        public void Init(PetObjectInitData data) {
            m_Owner = data.owner;
            m_Info = data.info;
            UpdateOffset();
            CreateSkills();
        }

        public override void Start() {
            base.Start();
            m_Movable = GetComponent<MovableObject>();
            m_Message = GetComponent<MmoMessageComponent>();
        }



        public override int behaviourId {
            get {
                return (int)ComponentID.PetObject;
            }
        }

        
        public NebulaObject owner {
            get {
                return m_Owner;
            }
        }

        public override void Update(float deltaTime) {

            base.Update(deltaTime);

            if(nebulaObject.subZone != m_Owner.subZone) {
                nebulaObject.SetSubZone(m_Owner.subZone);
            }

            m_OffsetChangeTimer -= deltaTime;
            if(m_OffsetChangeTimer <= 0f ) {
                m_OffsetChangeTimer = UPDATE_OFFSET_COOLDOWN;
                UpdateOffset();
            }

            var direction = targetPoint - transform.position;
            float distance = direction.magnitude;
            if (distance <= MIN_DISTANCE) {
                //UpdateOffset();
                //var nDir = direction.normalized;
                //var oPos = transform.position;
                //var oRot = transform.rotation;
                //var nPos = transform.position + nDir * m_Movable.speed * deltaTime;
                //var nRot = ComputeRotation(nDir, ROTATION_SPEED, deltaTime);
                //Move(oPos, oRot, nPos, nRot.eulerAngles, m_Movable.speed);
            } else {

                var nDir = direction.normalized;
                var oPos = transform.position;
                var oRot = transform.rotation;

                float modSpeed = distance / 1; //m_Movable.speed * (distance - MIN_DISTANCE) / (MAX_DISTANCE - MIN_DISTANCE);

                float moving = modSpeed * deltaTime;

                if(moving > distance ) {
                    moving = distance * 0.9f;
                }

                if(distance > MAX_DISTANCE ) {
                    moving += (distance - MAX_DISTANCE);
                    if (moving > distance) {
                        moving = distance * 0.9f;
                    }
                }

                if(Mathf.Approximately(moving, 0f)) {
                    moving = 0f;
                }

                float spFinal = 0;
                if (deltaTime > 0) {
                    spFinal = moving / deltaTime;
                }

                var nPos = transform.position + nDir * moving;


                var nRot =  ComputeRotation(nDir, ROTATION_SPEED, deltaTime).eulerAngles;
                //if(owner) {
                //    nRot = owner.transform.rotation;
                //}
                Move(oPos, oRot, nPos, nRot, spFinal);
            }

            UpdateSkills(deltaTime);
        }

        private void Move(Vector3 oldPos, Vector3 oldRot, Vector3 newPos, Vector3 newRot, float speed) {
            (nebulaObject as Item).Move(newPos.ToVector(), newRot.ToVector());
            m_Message.PublishMove(oldPos.ToArray(), oldRot.ToArray(), transform.position.ToArray(), transform.rotation.ToArray(), speed);
        }

        private Quat ComputeRotation(Vector3 direction, float rotationSpeed, float deltaTime) {
            var newRot = Quat.Slerp(Quat.Euler(transform.rotation), Quat.LookRotation(direction), Mathf.Clamp( rotationSpeed * deltaTime, 0, 1.3f));
            return newRot;
        }

        private void  UpdateOffset() {
            m_Offset = Rand.UnitVector3() * OFFSET_RADIUS;
        }

        private void CreateSkills() {
            m_Skills.Clear();
            if(m_Info.skills != null ) {
                PetSkillFactory factory = new PetSkillFactory();
                foreach(int skillId in m_Info.skills ) {
                    var skill = factory.Create(skillId, nebulaObject.resource);
                    if(skill != null ) {
                        m_Skills.Add(skill);
                    }
                }
            }

            s_Log.InfoFormat("{0} skills created".Color(LogColor.orange), m_Skills.Count);
        }

        private void UpdateSkills(float deltaTime) {
            m_SkillUpdateTimer -= deltaTime;
            if(m_SkillUpdateTimer <= 0f ) {
                m_SkillUpdateTimer = kSkillUpdateInterval;

                foreach(var skill in m_Skills ) {
                    if(skill.Use(this)) {
                        s_Log.InfoFormat("pet skill = {0} used success", skill.id);
                    }
                }
            }
        }

        private Vector3 targetPoint {
            get {
                return m_Owner.transform.position + m_Offset;
            }
        }

        public void Death() {
            s_Log.InfoFormat("Called Death() on Pet [red]");

            if(owner) {
                var petManager = owner.GetComponent<PetManager>();
                if(petManager) {
                    s_Log.InfoFormat("Remove pet from pet manager [red]");
                    petManager.RemovePet(nebulaObject.Id);
                }
            }
        }

        public void SendPetSkill(Hashtable properties) {
            if(m_Message) {
                m_Message.SendPetSkillUsed(properties);
            }
        }
    }
}
