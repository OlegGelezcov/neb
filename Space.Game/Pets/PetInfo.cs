using Common;
using System;
using System.Collections.Generic;

namespace Nebula.Pets {
    public class PetInfo {
        //
        private string m_Id;
        private int m_Exp;
        private PetColor m_Color;
        private string m_Type;
        private int m_PassiveSkill;
        private List<int> m_ActiveSkills;
        private bool m_Active;


        public PetInfo() {
            m_Id = Guid.NewGuid().ToString();
            m_Exp = 0;
            m_Color = PetColor.gray;
            m_Type = string.Empty;
            m_PassiveSkill = -1;
            m_ActiveSkills = new List<int>();
            m_Active = false;
        }

        public PetInfo(PetSave save) {
            m_Id = save.id;
            m_Exp = save.exp;
            m_Color = (PetColor)save.color;
            m_Type = save.type;
            m_PassiveSkill = save.passiveSkill;
            m_ActiveSkills = save.activeSkills;
            m_Active = save.active;
        }

        public string id {
            get {
                return m_Id;
            }
        }

        public List<int> skills {
            get {
                return m_ActiveSkills;
            }
        }

        public int exp {
            get {
                return m_Exp;
            }
        }

        public PetColor color {
            get {
                return m_Color;
            }
        }

        public int passiveSkill {
            get {
                return m_PassiveSkill;
            }
        }

        public bool active {
            get {
                return m_Active;
            }
        }

        public string type {
            get {
                return m_Type;
            }
        }

        public PetSave GetSave() {
            return new PetSave {
                active = active,
                activeSkills = skills,
                color = (int)color,
                exp = exp,
                id = id,
                passiveSkill = passiveSkill,
                type = type
            };
        }
    }
}
