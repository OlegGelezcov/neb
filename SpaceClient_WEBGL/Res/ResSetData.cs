

namespace Nebula.Client.Res
{
    using Common;
    using global::Common;
    using System.Collections.Generic;

    public class ResSetData
    {
        private readonly string id;
        private readonly string name;
        private readonly int unlockLevel;
        private readonly float dropProb;
        private readonly Workshop workshop;
        private readonly int skill;
        private readonly bool isDefault;

        public ResSetData(string id, string name, int unlockLevel, float dropProb, Workshop workshop, int skill, bool isDefault)
        {
            this.id = id;
            this.name = name;
            this.unlockLevel = unlockLevel;
            this.dropProb = dropProb;
            this.workshop = workshop;
            this.skill = skill;
            this.isDefault = isDefault;
        }

        public int Skill {
            get {
                return skill;
            }
        }

        public bool IsDefault {
            get {
                return isDefault;
            }
        }

        public string Id
        {
            get
            {
                return this.id;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public int UnlockLevel
        {
            get
            {
                return this.unlockLevel;
            }
        }

        public float DropProb
        {
            get
            {
                return this.dropProb;
            }
        }

        public Workshop Workshop
        {
            get
            {
                return this.workshop;
            }
        }


    }
}
