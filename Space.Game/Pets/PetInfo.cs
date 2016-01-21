using Common;
using Nebula.Server.Space.Game.Resources;
using System;
using System.Collections.Generic;
using System.Collections;
using ServerClientCommon;
using Space.Game;
using System.Linq;

namespace Nebula.Pets {
    public class PetInfo  {

        private const int kMaxMastery = 6;
        //
        private string m_Id;
        private int m_Exp;
        private PetColor m_Color;
        private string m_Type;
        private int m_PassiveSkill;
        private List<PetActiveSkill> m_ActiveSkills;
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
        public void SetActiveSkills(List<PetActiveSkill> activeSkills) {
            m_ActiveSkills = activeSkills;
        }
        public void SetActive(bool active) {
            m_Active = active;
        }

        public bool HasActiveSkill(int skill) {
            foreach(var s in m_ActiveSkills) {
                if(s.id == skill ) {
                    return true;
                }
            }
            return false;
        }

        public bool ActivateSkill(int id, bool activated) {
            var s = GetActiveSkill(id);
            if(s == null ) {
                return false;
            }
            s.SetActive(activated);
            return true;
        }

        public PetActiveSkill GetActiveSkill(int id) {
            return m_ActiveSkills.Where(s => s.id == id).FirstOrDefault();
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

        private bool ContainsSkill(int skillId ) {
            foreach(var s in m_ActiveSkills) {
                if(s.id == skillId ) {
                    return true;
                }
            }
            return false;
        }

        private bool ContainsSkill(PetActiveSkill skill) {
            foreach(var s in m_ActiveSkills) {
                if(s.id == skill.id) {
                    return true;
                }
            }
            return false;
        }

        public bool AddActiveSkill(PetActiveSkill skill) {
            if(m_ActiveSkills == null ) {
                m_ActiveSkills = new List<PetActiveSkill>();
            }
            if(ContainsSkill(skill)) {
                return false;
            }
            m_ActiveSkills.Add(skill);
            return true;
        }

        public bool RemoveActiveSkill(int skill) {
            if(m_ActiveSkills == null ) {
                m_ActiveSkills = new List<PetActiveSkill>();
            }
            int index = m_ActiveSkills.FindIndex((s) => s.id == skill);
            if(index >= 0 ) {
                m_ActiveSkills.RemoveAt(index);
                return true;
            }
            return false;
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
            m_ActiveSkills = new List<PetActiveSkill>();
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
            ParseActiveSkills(save.activeSkills);
            SetActive(save.active);
            SetAttackParameters(save.attackBaseAdd, save.attackColorAdd, save.attackLevelAdd);
            SetHpParameters(save.hpBaseAdd, save.hpColorAdd, save.hpLevelAdd);
            SetOptimalDistanceParameters(save.odBaseAdd, save.odColorAdd, save.odLevelAdd);
            SetDamageType((WeaponDamageType)(byte)save.damageType);
            SetKilledTime(save.killedTime);
            SetMastery(save.mastery);
        }

        private void ParseActiveSkills(List<Hashtable> listhash) {
            List<PetActiveSkill> skills = new List<PetActiveSkill>();
            if(listhash != null ) {
                foreach(Hashtable hash in listhash) {
                    PetActiveSkill pas = new PetActiveSkill();
                    pas.ParseInfo(hash);
                    skills.Add(pas);

                }
            }
            SetActiveSkills(skills);
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

        public List<PetActiveSkill> skills {
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

        private List<Hashtable> GetActiveSkillsSave() {
            List<Hashtable> listHash = new List<Hashtable>();
            foreach(var s in skills) {
                listHash.Add(s.GetInfo());
            }
            return listHash;
        }
        public PetSave GetSave() {
            return new PetSave {
                active = active,
                activeSkills = GetActiveSkillsSave(),
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

        private Hashtable[] GetSkillsInfo() {
            List<Hashtable> list = m_ActiveSkills.Select(s => s.GetInfo()).ToList();
            return list.ToArray();
        }

        public Hashtable GetInfo(IRes res) {
            return new Hashtable {
                {(int)SPC.Active, active  },
                {(int)SPC.Skills, GetSkillsInfo() },
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

        public PetColor nextColor {
            get {
                switch(m_Color) {
                    case PetColor.gray:
                        return PetColor.white;
                    case PetColor.white:
                        return PetColor.blue;
                    case PetColor.blue:
                        return PetColor.yellow;
                    case PetColor.yellow:
                        return PetColor.green;
                    case PetColor.green:
                        return PetColor.orange;
                    case PetColor.orange:
                        return PetColor.orange;
                    default:
                        return PetColor.orange;
                }
            }
        }

        public bool hasMaxColor {
            get {
                return (nextColor == m_Color);
            }
        }

        public int nextMastery {
            get {
                switch(mastery) {
                    case 0:
                        return 1;
                    case 1:
                        return 2;
                    case 2:
                        return 3;
                    case 3:
                        return 4;
                    case 4:
                        return 5;
                    case 5:
                        return 6;
                    case 6:
                        return 6;
                    default:
                        return 6;
                }
            }
        }

        public int maxAllowedMastery {
            get {
                switch (color) {
                    case PetColor.gray:
                        return 1;
                    case PetColor.white:
                        return 2;
                    case PetColor.blue:
                        return 3;
                    case PetColor.yellow:
                        return 4;
                    case PetColor.green:
                        return 5;
                    case PetColor.orange:
                        return 6;
                    default:
                        return 6;
                }
            }
        }

        public bool hasMaxMastery {
            get {
                return (mastery == nextMastery);
            }
        }

        public bool ImproveMastery() {
            if(false == hasMaxMastery) {
                SetMastery(nextMastery);
                return true;
            }
            return false;
        }
    }
}
