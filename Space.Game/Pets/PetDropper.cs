using Common;
using GameMath;
using Nebula.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Pets {
    public class PetDropper {
        
        public class PetDropSettings {
            public bool generateColor { get; private set; } = true;
            public bool generateModel { get; private set; } = true;
            public bool generatePassiveSkill { get; private set; } = true;
            public bool generateActiveSkills { get; private set; } = true;
            public bool generateDamageType { get; private set; } = true;
            public bool setManualMastery { get; private set; } = false;
            public bool generateRaceForModel { get; private set; } = false;
            private PetForceSettings overwrite { get; set; } = new PetForceSettings();

            public void OnGenerateColor() {
                generateColor = true;
            }
            public void OffGenerateColor() {
                generateColor = false;
            }
            public void OnGeneratePassiveSkill() {
                generatePassiveSkill = true;
            }
            public void OffGeneratePassiveSkill() {
                generatePassiveSkill = false;
            }
            public void OnGenerateModel() {
                generateModel = true;
            }
            public void OffGenerateModel() {
                generateModel = false;
            }
            public void OnGenerateActiveSkills() {
                generateActiveSkills = true;
            }
            public void OffGenerateActiveSkills() {
                generateActiveSkills = false;
            }
            public void OnGenerateDamageType() {
                generateDamageType = true;
            }
            public void OffGenerateDamageType() {
                generateDamageType = false;
            }
            public void OnSetMastery() {
                setManualMastery = true;
            }
            public void OffSetMastery() {
                setManualMastery = false;
            }
            public void OnGenerateRace() {
                generateRaceForModel = true;
            }
            public void OffGenerateRace() {
                generateRaceForModel = false;
            }

            public void SetColor(PetColor color) {
                overwrite.SetColor(color);
            }
            public void SetModel(string model) {
                overwrite.SetModel(model);
            }
            public void SetPassiveSkill(int skill) {
                overwrite.SetPassiveSkill(skill);
            }
            public void SetActiveSkills(List<int> activeSkills ) {
                overwrite.SetActiveSkills(activeSkills);
            }
            public void SetDamageType(WeaponDamageType damageType ) {
                overwrite.SetDamageType(damageType);
            }
            public void SetMastery(int mastery) {
                overwrite.SetMastery(mastery);
            }
            public void SetRace(Race race) {
                overwrite.SetRace(race);
            }

            public PetColor overwriteColor {
                get {
                    return overwrite.color;
                }
            }

            public string overwriteModel {
                get {
                    return overwrite.model;
                }
            }

            public int overwritePassiveSkill {
                get {
                    return overwrite.passiveSkill;
                }
            }

            public List<int> overwriteActiveSkills {
                get {
                    if(overwrite.activeSkills == null ) {
                        overwrite.SetActiveSkills(new List<int>());
                    }
                    return overwrite.activeSkills;
                }
            }

            public WeaponDamageType overwriteDamageType {
                get {
                    return overwrite.damageType;
                }
            }

            public int overwriteMastery {
                get {
                    return overwrite.mastery;
                }
            }

            public Race overwriteRace {
                get {
                    return overwrite.race;
                }
            }
        }

        public class PetForceSettings {
            public PetColor color { get;  private set; }
            public string model { get; private set; }
            public int passiveSkill { get; private set; }
            public List<int> activeSkills { get; private set; }
            public WeaponDamageType damageType { get; private set; }
            public int mastery { get; private set; }
            public Race race { get; private set; }

            public void SetColor(PetColor c) {
                color = c;
            }
            public void SetModel(string m) {
                model = m;
            }
            public void SetPassiveSkill(int ps) {
                passiveSkill = ps;
            }
            public void SetActiveSkills(List<int> ass) {
                activeSkills = ass;
            } 
            public void SetDamageType(WeaponDamageType dt) {
                damageType = dt;
            }
            public void SetMastery(int m) {
                mastery = m;
            }

            public void SetRace(Race r) {
                race = r;
            }

        }

        public PetInfo Drop(PetParameters parameters, PetDropSettings settings, PetSkillCollection skills) {
            PetInfo pet = new PetInfo();
            pet.SetId(Guid.NewGuid().ToString());
            pet.SetExp(0);

            pet.SetColor(GetColor(parameters, settings));
            pet.SetType(GetType(parameters, settings));
            pet.SetPassiveSkill(GetPassiveSkill(parameters, settings));
            pet.SetActiveSkills(GetActiveSkills(pet.color, parameters, settings, skills));
            pet.SetActive(false);


            float attackBaseAdd = Rand.Float(parameters.damage.BaseMin(), parameters.damage.BaseMax());
            float attackColorAdd = Rand.Float(parameters.damage.ColorMin(), parameters.damage.ColorMax());
            float attackLevelAdd = Rand.Float(parameters.damage.LevelMin(), parameters.damage.LevelMax());
            pet.SetAttackParameters(attackBaseAdd, attackColorAdd, attackLevelAdd);

            float hpBaseAdd = Rand.Float(parameters.hp.BaseMin(), parameters.hp.BaseMax());
            float hpColorAdd = Rand.Float(parameters.hp.ColorMin(), parameters.hp.ColorMax());
            float hpLevelAdd = Rand.Float(parameters.hp.LevelMin(), parameters.hp.LevelMax());
            pet.SetHpParameters(hpBaseAdd, hpColorAdd, hpLevelAdd);

            float odBaseAdd = Rand.Float(parameters.od.BaseMin(), parameters.od.BaseMax());
            float odColorAdd = Rand.Float(parameters.od.ColorMin(), parameters.od.ColorMax());
            float odLevelAdd = Rand.Float(parameters.od.LevelMin(), parameters.od.LevelMax());
            pet.SetOptimalDistanceParameters(odBaseAdd, odColorAdd, odLevelAdd);

            pet.SetDamageType(GetDamageType(parameters, settings));
            pet.SetMastery(GetMastery(parameters, settings));
            return pet;
        }

        private PetColor GetColor(PetParameters parameters, PetDropSettings settings) {
            if(settings.generateColor) {
                return GenerateColor(parameters);
            } else {
                return settings.overwriteColor;
            }
        }

        private string GetType(PetParameters parameters, PetDropSettings settings) {
            if(settings.generateModel ) {
                return GenerateModel(parameters, settings);
            } else {
                return settings.overwriteModel;
            }
        }

        private int GetPassiveSkill(PetParameters parameters, PetDropSettings settings) {
            if(settings.generatePassiveSkill) {
                return GeneratePassiveSkill(parameters);
            } else {
                return settings.overwritePassiveSkill;
            }
        }

        private int GeneratePassiveSkill(PetParameters parameters) {
            //not realized yet
            return -1;
        }

        private List<int> GetActiveSkills(PetColor color, PetParameters parameters, PetDropSettings settings, PetSkillCollection skills) {
            if(settings.generateActiveSkills) {
                return GenerateActiveSkills(color, parameters,  skills);
            } else {
                return settings.overwriteActiveSkills.Take(parameters.activeSkillCountTable[color]).ToList();
            }
        }

        private WeaponDamageType GetDamageType(PetParameters parameters, PetDropSettings settings) {
            if(settings.generateDamageType) {
                return GenerateDamageType(parameters, settings);
            } else {
                return settings.overwriteDamageType;
            }
        }

        private int GetMastery(PetParameters parameters, PetDropSettings settings) {
            if(settings.setManualMastery) {
                return settings.overwriteMastery;
            } else {
                return GenerateMastery();
            }
        }


        private int GenerateMastery() {
            return 0;
        }

        private WeaponDamageType GenerateDamageType(PetParameters parameters, PetDropSettings settings) {
            if(Rand.Int() % 2 == 0 ) {
                return WeaponDamageType.damage;
            } else {
                return WeaponDamageType.heal;
            }
        }

        private List<int> GenerateActiveSkills(PetColor color, PetParameters parameters, PetSkillCollection skills) {
            return skills.GetRandomSkills(parameters.activeSkillCountTable[color]);
        }

        private string GenerateModel(PetParameters parameters, PetDropSettings settings ) {
            if(settings.generateRaceForModel) {
                return parameters.typeTable.GetRandomType();
            } else {
                var ids = parameters.typeTable.GetTypes(settings.overwriteRace);
                return ids[Rand.Int(0, ids.Count - 1)];
            }
        }

        private PetColor GenerateColor(PetParameters parameters) {
            var orange = parameters.petColorDropResource.GetColor(PetColor.orange);
            float randomNumber = Rand.Float01();

            if(randomNumber < orange.prob) {
                return PetColor.orange;
            }
            var green = parameters.petColorDropResource.GetColor(PetColor.green);
            if(randomNumber < green.prob) {
                return PetColor.green;
            }

            var yellow = parameters.petColorDropResource.GetColor(PetColor.yellow);
            if(randomNumber < yellow.prob) {
                return PetColor.yellow;
            }

            var blue = parameters.petColorDropResource.GetColor(PetColor.blue);
            if(randomNumber < blue.prob ) {
                return PetColor.blue;
            }

            var white = parameters.petColorDropResource.GetColor(PetColor.white);
            if(randomNumber < white.prob) {
                return PetColor.white;
            }

            return PetColor.gray;
        }

    }
}
