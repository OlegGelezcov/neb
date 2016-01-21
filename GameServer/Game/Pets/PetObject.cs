using Common;
using ExitGames.Logging;
using GameMath;
using Nebula.Engine;
using Nebula.Game.Components;
using Nebula.Game.Pets.PassiveBonuses;
using Nebula.Game.Pets.Skills;
using Nebula.Game.Utils;
using Nebula.Pets;
using ServerClientCommon;
using Space.Server;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nebula.Game.Pets {
    public class PetObject : NebulaBehaviour {

        private static readonly ILogger s_Log = LogManager.GetCurrentClassLogger();

        private const float kSkillUpdateInterval = 4;
        private const float kPassiveBonusUpdateInterval = 4;
        private const int kCollectChestBonus = 11;



        private NebulaObject m_Owner;
        private PetInfo m_Info;

        private MmoMessageComponent m_Message;

        
        private MovableObject m_Movable;
        private readonly List<PetSkill> m_Skills = new List<PetSkill>();
        private PassivePetBonus m_PassiveBonus = null;

        private float m_SkillUpdateTimer = kSkillUpdateInterval;
        private float m_PassiveBonusUpdateTimer = kPassiveBonusUpdateInterval;
        private PetBaseState m_State;
        private CharacterObject m_Character;



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
            SetOwner(data.owner);
            SetInfo(data.info);
            CreateSkills();
            CreatePassiveBonus();
        }

        public override void Start() {
            base.Start();
            m_Movable = GetComponent<MovableObject>();
            m_Message = GetComponent<MmoMessageComponent>();
            m_State = new PetIdleState(this);
            m_Character = GetComponent<CharacterObject>();
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

        public PetInfo info {
            get {
                return m_Info;
            }
        }

        public void SetState(PetBaseState state) {
            m_State = state;
        }

        public override void Update(float deltaTime) {

            base.Update(deltaTime);

            if(m_State != null ) {
                m_State.Update(deltaTime);
            }

            if(nebulaObject.subZone != m_Owner.subZone) {
                nebulaObject.SetSubZone(m_Owner.subZone);
                nebulaObject.MmoMessage().SendSubZoneUpdate(EventReceiver.ItemSubscriber, nebulaObject.subZone);
            }

            UpdateSkills(deltaTime);
            UpdatePassiveBonus(deltaTime);
        }

        public void Move(Vector3 oldPos, Vector3 oldRot, Vector3 newPos, Vector3 newRot, float speed) {
            (nebulaObject as Item).Move(newPos.ToVector(), newRot.ToVector());
            m_Message.PublishMove(oldPos.ToArray(), oldRot.ToArray(), transform.position.ToArray(), transform.rotation.ToArray(), speed);
        }

        public Quat ComputeRotation(Vector3 direction, float rotationSpeed, float deltaTime) {
            var newRot = Quat.Slerp(Quat.Euler(transform.rotation), Quat.LookRotation(direction), Mathf.Clamp( rotationSpeed * deltaTime, 0, 1.3f));
            return newRot;
        }


        private void CreateSkills() {
            m_Skills.Clear();
            if(m_Info.skills != null ) {
                PetSkillFactory factory = new PetSkillFactory();
                foreach(PetActiveSkill s in m_Info.skills ) {
                    if (s.activated) {
                        var skill = factory.Create(s.id, nebulaObject.resource, nebulaObject);
                        if (skill != null) {
                            //s_Log.InfoFormat("create activated pet skill = {0}".Color(LogColor.orange), skill.id);
                            m_Skills.Add(skill);
                        }
                    }
                }
            }

            //s_Log.InfoFormat("{0} skills created".Color(LogColor.orange), m_Skills.Count);
        }

        private void CreatePassiveBonus() {
            PassiveBonusFactory factory = new PassiveBonusFactory();
            var bonus = resource.petPassiveBonuses[m_Info.passiveSkill];
            m_PassiveBonus = factory.Create(bonus, nebulaObject);
            //s_Log.InfoFormat("passive bonus {0} created".Color(LogColor.orange), m_PassiveBonus.id);
        }

        private void UpdateSkills(float deltaTime) {
            m_SkillUpdateTimer -= deltaTime;
            if(m_SkillUpdateTimer <= 0f ) {
                m_SkillUpdateTimer = kSkillUpdateInterval;

                foreach(var skill in m_Skills ) {
                    if (skill.data.auto) {
                        if (skill.Use()) {
                            //s_Log.InfoFormat("pet skill = {0} used success".Color(LogColor.orange), skill.id);
                        }
                    }
                }
            }
        }

        private void UpdatePassiveBonus(float deltaTime) {
            m_PassiveBonusUpdateTimer -= deltaTime;
            if(m_PassiveBonusUpdateTimer <= 0f ) {
                m_PassiveBonusUpdateTimer = kPassiveBonusUpdateInterval;
                if(m_PassiveBonus != null ) {
                    if(m_PassiveBonus.Check() ) {
                        //s_Log.InfoFormat("passive bonus = {0} successfully checked".Color(LogColor.orange), m_PassiveBonus.id);
                    }
                }
            }
        }

        public bool HasValidActiveSkill(int id) {
            foreach(var skill in m_Skills) {
                if(skill.id == id && skill.ConditionsValid()) {
                    return true;
                }
            }
            return false;
        }

        public bool UseExplicit(int id, NebulaObject target) {
            foreach(var skill in m_Skills) {
                if(skill.id == id && (false == skill.data.auto) ) {
                    return skill.UseExplicit(target);
                }
            }
            return false;
        }


        public void SendPetSkill(Hashtable properties) {
            if(m_Message) {
                m_Message.SendPetSkillUsed(properties);
            }
        }

        /// <summary>
        /// Called when pet is killed ( update killed time on info)
        /// </summary>
        public void OnWasKilled() {
           // s_Log.InfoFormat("update killed time on pet".Color(LogColor.orange));
            owner.GetComponent<PetManager>().UpdateKilledTime(this);
        }

        public void SetInfo(PetInfo info) {
            
            m_Info = info;
            if(info != null ) {
                if (owner != null) {
                    nebulaObject.properties.SetProperty((byte)PS.Info, new Hashtable {
                        {(int)SPC.Active, info.active },
                        {(int)SPC.Skills, info.skills.Select(s => s.id).ToArray() },
                        {(int)SPC.Color, (int)(byte)info.color },
                        {(int)SPC.Exp, info.exp },
                        {(int)SPC.PassiveSkill, info.passiveSkill },
                        {(int)SPC.DamageType, (int)(byte)info.damageType },
                        {(int)SPC.KilledTime, info.killedTime },
                        {(int)SPC.Model, info.type },
                        {(int)SPC.OwnerGameRef, owner.Id }
                    });
                }
            }

        }

        private void SetOwner(NebulaObject inOwner) {
            m_Owner = inOwner;
        }

        public bool HasPassiveBonus(int bon) {
            if(m_PassiveBonus == null ) {
                return false;
            }
            return m_PassiveBonus.id == bon;
        }

        public void CollectChest(NebulaObject chest) {

            if (m_State == null || (m_State.name != PetState.CollectContainer)) {
                SetState(new CollectContainerState(this, chest));
            } else {
                (m_State as CollectContainerState).AddChest(chest);
            }
        }

        public bool RollMastery() {
            if(m_Info == null ) {
                return false;
            }

            float masteryProb = resource.petParameters.masteryTable[m_Info.mastery];
            if(Rand.Float01() < masteryProb) {
                return true;
            }
            return false;
        }
    }
}
