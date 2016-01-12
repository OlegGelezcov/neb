using Common;
using Nebula.Server.Space.Game.Resources;
using System;
using System.Collections.Generic;
using System.Collections;
using ServerClientCommon;
using Space.Game;

namespace Nebula.Pets {
    public class PetInfo  {
        //
        private string m_Id;
        private int m_Exp;
        private PetColor m_Color;
        private string m_Type;
        private int m_PassiveSkill;
        private List<int> m_ActiveSkills;
        private bool m_Active;

        private float m_AttackBaseAdd;
        private float m_AttackColorAdd;
        private float m_AttackLevelAdd;

        private float m_HpBaseAdd;
        private float m_HpColorAdd;
        private float m_HpLevelAdd;

        private float m_OdBaseAdd;
        private float m_OdColorAdd;
        private float m_OdLevelAdd;

        private WeaponDamageType m_DamageType;
        private float m_KilledTime;

        private int m_Mastery;

        public void SetId(string id) {
            m_Id = id;
        }
        public void SetExp(int exp) {
            m_Exp = exp;
        }
        public void SetColor(PetColor color) {
            m_Color = color;
        }
        public void SetType(string model) {
            m_Type = model;
        }
        public void SetPassiveSkill(int passiveSkill) {
            m_PassiveSkill = passiveSkill;
        }
        public void SetActiveSkills(List<int> activeSkills) {
            m_ActiveSkills = activeSkills;
        }
        public void SetActive(bool active) {
            m_Active = active;
        }
        public void SetAttackParameters(float baseAdd, float colorAdd, float levelAdd ) {
            m_AttackBaseAdd = baseAdd;
            m_AttackColorAdd = colorAdd;
            m_AttackLevelAdd = levelAdd;
        }
        public void SetHpParameters(float baseAdd, float colorAdd, float levelAdd) {
            m_HpBaseAdd = baseAdd;
            m_HpColorAdd = colorAdd;
            m_HpLevelAdd = levelAdd;
        }
        public void SetOptimalDistanceParameters(float baseAdd, float colorAdd, float levelAdd ) {
            m_OdBaseAdd = baseAdd;
            m_OdColorAdd = colorAdd;
            m_OdLevelAdd = levelAdd;
        }
        public void SetDamageType(WeaponDamageType damageType) {
            m_DamageType = damageType;
        }
        public void SetKilledTime(float time) {
            m_KilledTime = time;
        }
        public void SetMastery(int mastery) {
            m_Mastery = mastery;
        }

        public bool AddActiveSkill(int skill) {
            if(m_ActiveSkills == null ) {
                m_ActiveSkills = new List<int>();
            }
            if(m_ActiveSkills.Contains(skill)) {
                return false;
            }
            m_ActiveSkills.Add(skill);
            return true;
        }

        public bool RemoveActiveSkill(int skill) {
            if(m_ActiveSkills == null ) {
                m_ActiveSkills = new List<int>();
            }
            return m_ActiveSkills.Remove(skill);
        }

        public int GetActiveSkillCount() {
            if(m_ActiveSkills != null) {
                return m_ActiveSkills.Count;
            }
            return 0;
        }

        public PetInfo() {
            m_Id = Guid.NewGuid().ToString();
            m_Exp = 0;
            m_Color = PetColor.gray;
            m_Type = string.Empty;
            m_PassiveSkill = -1;
            m_ActiveSkills = new List<int>();
            m_Active = false;
            m_DamageType = WeaponDamageType.damage;
            m_KilledTime = 0;
            m_Mastery = 0;
        }

        public PetInfo(PetSave save) {
            SetId(save.id);
            SetExp(save.exp);
            SetColor((PetColor)save.color);
            SetType(save.type);
            SetPassiveSkill(save.passiveSkill);
            SetActiveSkills(save.activeSkills);
            SetActive(save.active);
            SetAttackParameters(save.attackBaseAdd, save.attackColorAdd, save.attackLevelAdd);
            SetHpParameters(save.hpBaseAdd, save.hpColorAdd, save.hpLevelAdd);
            SetOptimalDistanceParameters(save.odBaseAdd, save.odColorAdd, save.odLevelAdd);
            SetDamageType((WeaponDamageType)(byte)save.damageType);
            SetKilledTime(save.killedTime);
            SetMastery(save.mastery);
        }

        #region Properties
        public WeaponDamageType damageType {
            get {
                return m_DamageType;
            }
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

        public float killedTime {
            get {
                return m_KilledTime;
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

        public int mastery {
            get {
                return m_Mastery;
            }
        }


        private float attackBaseAdd {
            get {
                return m_AttackBaseAdd;
            }
        }
        private float attackColorAdd {
            get {
                return m_AttackColorAdd;
            }
        }
        private float attackLevelAdd {
            get {
                return m_AttackLevelAdd;
            }
        }

        private float hpBaseAdd {
            get {
                return m_HpBaseAdd;
            }
        }

        private float hpColorAdd {
            get {
                return m_HpColorAdd;
            }
        }

        private float hpLevelAdd {
            get {
                return m_HpLevelAdd;
            }
        }

        private float odBaseAdd {
            get {
                return m_OdBaseAdd;
            }
        }

        private float odColorAdd {
            get {
                return m_OdColorAdd;
            }
        }

        private float odLevelAdd {
            get {
                return m_OdLevelAdd;
            }
        } 
        #endregion

        public float Damage(IPetParamResource res, ILeveling levelRes) {
            float cm = res.ColorMult(this);
            float bv = res.BaseValue();
            float cv = res.ColorValue();
            float lv = res.LevelValue();
            int level = levelRes.LevelForExp(exp);
            //return (res.BaseValue(this) + attackBaseAdd) + res.ColorMult(this) * (res.ColorValue(this) + attackColorAdd) + res.Level(this) * (res.LevelValue(this) + attackLevelAdd);
            return (bv + attackBaseAdd) + cm * cm * (cv + attackColorAdd) + level * cm * (lv + attackLevelAdd);
        }

        public float Hp(IPetParamResource res, ILeveling levelRes) {
            float cm = res.ColorMult(this);
            float bv = res.BaseValue();
            float cv = res.ColorValue();
            float lv = res.LevelValue();
            int level = levelRes.LevelForExp(exp);

            //return (res.BaseValue(this) + hpBaseAdd) + res.ColorMult(this) * (res.ColorValue(this) + hpColorAdd) + res.Level(this) * (res.LevelValue(this) + hpLevelAdd);
            return (bv + hpBaseAdd) + cm * cm * (cv + hpColorAdd) + level * cm * (lv + hpLevelAdd);
        }

        public float OptimalDistance(IPetParamResource res, ILeveling levelRes) {
            float cm = res.ColorMult(this);
            float bv = res.BaseValue();
            float cv = res.ColorValue();
            float lv = res.LevelValue();
            int level = levelRes.LevelForExp(exp);
            //return (res.BaseValue(this) + odBaseAdd) + res.ColorMult(this) * (res.ColorValue(this) + odColorAdd) + res.Level(this) * (res.LevelValue(this) + odLevelAdd);
            return (bv + odBaseAdd) + cm * cm * (cv + odColorAdd) + level * cm * (lv + odLevelAdd);
        }

        public float Cooldown(IPetParamResource res) {
            return res.BaseValue() / res.ColorMult(this);
        }



        public void AddExp(int addExp ) {
            m_Exp += addExp;
        }

        public PetSave GetSave() {
            return new PetSave {
                active = active,
                activeSkills = skills,
                color = (int)color,
                exp = exp,
                id = id,
                passiveSkill = passiveSkill,
                type = type,
                attackBaseAdd = attackBaseAdd,
                attackColorAdd = attackColorAdd,
                attackLevelAdd = attackLevelAdd,
                hpBaseAdd = hpBaseAdd,
                hpColorAdd = hpColorAdd,
                hpLevelAdd = hpLevelAdd,
                odBaseAdd = odBaseAdd,
                odColorAdd = odColorAdd,
                odLevelAdd = odLevelAdd,
                damageType = (int)(byte)damageType,
                killedTime = killedTime,
                mastery = mastery
            };
        }

        public Hashtable GetInfo(IRes res) {
            return new Hashtable {
                {(int)SPC.Active, active  },
                {(int)SPC.Skills, skills.ToArray() },
                {(int)SPC.Color, (int)color },
                {(int)SPC.Exp, exp },
                {(int)SPC.Id, id },
                {(int)SPC.PassiveSkill, passiveSkill },
                {(int)SPC.Type, type },
                {(int)SPC.Damage, Damage(res.petParameters.damage, res.Leveling ) },
                {(int)SPC.MaxHealth, Hp(res.petParameters.hp, res.Leveling ) },
                {(int)SPC.OptimalDistance, OptimalDistance(res.petParameters.od, res.Leveling) },
                {(int)SPC.Cooldown, Cooldown(res.petParameters.cooldown) },
                {(int)SPC.DamageType, (int)(byte)damageType },
                {(int)SPC.KilledTime, killedTime },
                {(int)SPC.Mastery, mastery },
                {(int)SPC.CurrentTime, CommonUtils.SecondsFrom1970() }
            };
        }
    }
}
