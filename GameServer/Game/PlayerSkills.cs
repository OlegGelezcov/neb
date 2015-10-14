using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Space.Mmo.Server;
using System.Linq;

namespace Space.Game
{
    using Skills;
using Space.Game.Ship;
    using Common;

	public interface IPlayerSkills
	{
		void ReplaceSkill(int index,  UseSkill skill);
        void UseSkill(int index, MmoActor player);
        UseSkill GetSkill(int index);
	}

	public class PlayerSkills : IPlayerSkills, IInfo
	{



		//private List<UseSkill> activeSkills = new List<UseSkill>();
        private List<UseSkill> availableSkills = new List<UseSkill>();
        //private 

		public List<UseSkill> useSkills;

        private ICombatActor _owner;

        public int SkillCount {
            get {
                if (useSkills == null)
                    return 0;
                return useSkills.Count;
            }
        }

        public void UpdateSkills(ShipModel model)
        {
            string[] skills = model.Skills;

            List<int> removedIndices = new List<int>();
            for(int i = 0; i < this.useSkills.Count; i++)
            {
                if (false == skills.Contains(this.useSkills[i].ID))
                {
                    removedIndices.Add(i);
                }
            }

            foreach(int removeIndex in removedIndices)
            {
                if (this.useSkills[removeIndex] != null)
                    this.useSkills[removeIndex].Release();

                this.useSkills[removeIndex] = new EmptySkill();
            }

            List<int> freeIndices = new List<int>();
            for (int i = 0; i < this.useSkills.Count; i++)
            {
                if(this.useSkills[i].IsEmpty)
                {
                    freeIndices.Add(i);
                }
            }

            Dictionary<int, string> newSkills = new Dictionary<int, string>();
            int counter = 0;
            foreach (string skill in skills)
            {
                if(false == this.IsUseSkill(skill))
                {
                    newSkills.Add(freeIndices[counter++], skill);
                }
            }

            foreach(var pair in newSkills)
            {
                UseSkill s = this.GetAvailableSkill(pair.Value);
                if( s != null )
                {
                    this.useSkills[pair.Key] = s;
                }
            }

            if(string.IsNullOrEmpty(model.Sets.FullSetSkill))
            {
                this.useSkills[5] = EmptySkill.Get;
            }
            else
            {
                UseSkill skill = this.GetAvailableSkill(model.Sets.FullSetSkill);
                this.useSkills[5] = (skill != null) ? skill : EmptySkill.Get;
            }

            //generate update skills event when method colled
            if(_owner != null )
            {
                MmoActor actor = this._owner as MmoActor;
                if(actor != null)
                {
                    actor.EventOnSkillsUpdated();
                }
            }
            //owner sent skills updated
        }



         
		public PlayerSkills(ICombatActor actor)
		{
            _owner = actor;
			useSkills = new List<UseSkill>();
			for(int i = 0; i < 6; i++)
			{
				useSkills.Add(new EmptySkill());
			}
			//test
			//Parce();
			//
		}

        public PlayerSkills()
        {
            useSkills = new List<UseSkill>();
            for(int i = 0; i < 6; i++ )
            {
                useSkills.Add(new EmptySkill());
            }
        }

		public void ReplaceSkill(int index,  UseSkill skill)
		{
			if( index >=0 && index < useSkills.Count )
			{
                if (useSkills[index] != null) {
                    useSkills[index].Release();
                }
				useSkills[index] = skill;
			}
		}

        /// <summary>
        /// Verify parameter skill setted as Use Skill
        /// </summary>
        /// <param name="skillId"></param>
        /// <returns>true if skill is use skill</returns>
        public bool IsUseSkill(string skillId) {
            foreach (var skill in this.useSkills) {
                if (skill.ID == skillId)
                    return true;
            }
            return false;
        }
        public void UseSkill(int index, MmoActor player)
		{
			if( index >=0 && index < useSkills.Count )
			{
                useSkills[index].Use(player);
			}
		}

		public UseSkill GetSkill(int index)
		{
			if( index >=0 && index < useSkills.Count )
			{
				return useSkills[index];
			}
			else
			{
				return new EmptySkill();
			}
		}


        public void Touch() {
            useSkills.ForEach(s => s.Touch());
        }

        public void Release() {
            useSkills.ForEach(s => s.Release());
        }

        public void updateCooldown()
        {
            useSkills.ForEach((s) => {
                if( _owner != null)
                    s.updateCooldown(_owner);
            });
        }
	#region Test

        public UseSkill GetAvailableSkill(string id) {
            var aSkill = availableSkills.Where(s => s.ID == id).FirstOrDefault();
            if (aSkill == null)
                aSkill = new EmptySkill();
            return aSkill;
        }



		public void Parce()
		{
            availableSkills.Add(new IncreasedFirePower());
            availableSkills.Add(new InterferenceSystemRange());
            availableSkills.Add(new InterferenceSystemPrecision());
            availableSkills.Add(new InterferenceSystemRecharge());
            availableSkills.Add(new Rocket());
            availableSkills.Add(new HeavyRocket());
            availableSkills.Add(new AcidCharge());
            availableSkills.Add(new ScatteredShoot());
            availableSkills.Add(new CumulativeShoot());
            availableSkills.Add(new ComputedTrajectoryShoot());
            availableSkills.Add(new LightRocket());
            availableSkills.Add(new DecreasePrecision());
            availableSkills.Add(new IncreaseCooldown());
            availableSkills.Add(new BlockResist());
            availableSkills.Add(new PowerBlock());
            availableSkills.Add(new OverheatingGuns());
            availableSkills.Add(new ImprovedCooldown());
            availableSkills.Add(new PriorityDamage());
            availableSkills.Add(new IncreaseArmor());
            availableSkills.Add(new IncreaseDamage());
            availableSkills.Add(new ImprovedGuidance());
            availableSkills.Add(new Jump());

            //useSkills[0] = new HeavyRocket();
            //useSkills[1] = new AcidCharge();
            //useSkills[2] = new LightRocket();
            //useSkills[3] = new DecreasePrecision();
            //useSkills[4] = new IncreaseDamage();
            //useSkills[5] = new Jump();

            useSkills.ForEach((s) => {
                if(_owner != null)
                    s.updateCooldown(_owner);
            });
		}

	#endregion


        public Hashtable GetInfo()
        {
            Hashtable result = new Hashtable();
            for(int i = 0; i < this.useSkills.Count; i++)
            {
                result.Add(i, this.useSkills[i].ID);
            }
            return result;
        }

        public void ParseInfo(Hashtable info)
        {
            foreach(DictionaryEntry entry in info )
            {
                int index = (int)entry.Key;
                string skillId = (string)entry.Value;
                if (skillId == null)
                    skillId = string.Empty;
                if (this.useSkills[index] != null)
                    this.useSkills[index].Release();

                this.useSkills[index] = this.GetAvailableSkill(skillId);
            }
        }

        public void Replace(PlayerSkills other )
        {
            this.useSkills = other.useSkills;
        }
    }
     
}
