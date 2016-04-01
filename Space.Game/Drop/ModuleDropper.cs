// ModuleDropper.cs
// Nebula
// 
// Created by Oleg Zhelestcov on Thursday, November 6, 2014 2:04:25 PM
// Copyright (c) 2014 KomarGames. All rights reserved.
//

namespace Space.Game.Drop
{
    using Common;
    using GameMath;
    using Nebula.Balance;
    using Space.Game.Resources;
    using Space.Game.Ship;
    using System;
    using System.Collections.Generic;

    public class ModuleDropper : Dropper
    {


        //private Dictionary<Difficulty, float> mHPDiff = new Dictionary<Difficulty, float> {
        //    {  Difficulty.starter, 0.2f },
        //    {  Difficulty.easy, 0.5f },
        //    {  Difficulty.easy2, 0.7f },
        //    {  Difficulty.medium, 1f },
        //    {  Difficulty.none, 1f },
        //    {  Difficulty.hard, 2.2f },
        //    {  Difficulty.boss, 2.5f },
        //    {  Difficulty.boss2, 2f }
        //};

        public class ModuleDropParams
        {
            public readonly IRes resource;
            public Workshop workshop;
            public ShipModelSlotType slotType;
            public readonly int level;
            public readonly Dictionary<string, int> craftingMaterials;
            public readonly Difficulty difficulty;

            public string templateID;
            public bool useTemplateID;
            public ModuleInfo moduleInfo;
            public ObjectColor color;
            public string set;



            public ModuleDropParams(
                IRes resource,  
                Workshop inWorkshop,
                ShipModelSlotType inSlotType, 
                int level,  
                Difficulty difficulty, 
                Dictionary<string, int> incraftingMaterials,
                ObjectColor color,
                string set)
            {
                this.resource = resource;
                this.level = level;
                this.difficulty = difficulty;
                this.craftingMaterials = incraftingMaterials;
                workshop = inWorkshop;
                slotType = inSlotType;
                this.color = color;
                this.set = set;
            }

            public ModuleDropParams (IRes resource, string inTemplateID, int level, Difficulty difficulty,
                Dictionary<string, int> incraftingMaterials, ObjectColor color,
                string set) {
                this.resource = resource;
                this.level = level;
                this.difficulty = difficulty;
                this.craftingMaterials = incraftingMaterials;
                templateID = inTemplateID;
                useTemplateID = true;

                moduleInfo = resource.ModuleTemplates.Module(templateID);
                slotType = moduleInfo.Type;
                workshop = moduleInfo.Workshop;
                this.color = color;
                this.set = set;
            }
        }

        private readonly ModuleDropParams dropParams;

        public ModuleDropper(ModuleDropParams dp) {
            dropParams = dp;
        }



        public override IDroppable Drop() {
            return DropModule();
        }

        public ShipModule DropModule() {
            ModuleInfo moduleData;
            if (dropParams.useTemplateID) {
                moduleData = dropParams.moduleInfo;
            } else {
                moduleData = dropParams.resource.ModuleTemplates.RandomModule(dropParams.workshop, dropParams.slotType);
            }
            if(moduleData == null ) {
                throw new Exception("not found module data");
            }
            ModuleSettingData moduleSetting;
            if(!dropParams.resource.ModuleSettings.TeyGetWorkshopData(dropParams.workshop, out moduleSetting)) {

                throw new Exception("not found module settings");
            }

            ModuleSlotSettingData slotSetting;
            if(!moduleSetting.TryGetSlotSetting(dropParams.slotType, out slotSetting)) {
                throw new Exception("not found module slot settings");
            }

            ShipModule module = new ShipModule(
                dropParams.slotType, 
                Guid.NewGuid().ToString(), 
                dropParams.level, 
                moduleData.Name, 
                dropParams.workshop, 
                moduleData.Id, 
                dropParams.craftingMaterials, 
                dropParams.difficulty);

            ColorInfo colorInfo = dropParams.resource.ColorRes.Color(ColoredObjectType.Module, dropParams.color); //SelectColor(dropParams.resource);
            int[] basePoints = Rand.GenerateNumbers(3, dropParams.resource.ModuleSettings.hpSpeedCargoPtMax);

            module.SetColor(colorInfo.color);
            module.SetPrefab(moduleData.Model);


            //module.SetHP(BalanceFormulas.ComputeHP(colorInfo.factor, moduleSetting.base_hp, moduleSetting.base_hp_factor, dropParams.level, basePoints[0], slotSetting.hp_points_value, slotSetting.hp_points_factor) * mHPDiff[dropParams.difficulty]);
            //module.SetSpeed(BalanceFormulas.ComputeSPEED(colorInfo.factor, moduleSetting.base_speed, moduleSetting.base_speed_factor, dropParams.level, basePoints[1], slotSetting.speed_points_value, slotSetting.speed_points_factor));
            //module.SetHold(BalanceFormulas.ComputeCARGO(colorInfo.factor, moduleSetting.base_cargo, moduleSetting.base_cargo_factor, dropParams.level, basePoints[2], slotSetting.cargo_points_value));
            module.SetHP(BalanceFormulas.Hp(dropParams.resource.ModuleSettings, slotSetting.hp, dropParams.level, basePoints[0], colorInfo) * dropParams.resource.difficulty.module[dropParams.difficulty] );
            module.SetSpeed(BalanceFormulas.Speed(dropParams.resource.ModuleSettings, slotSetting.speed, dropParams.level, basePoints[1], colorInfo));
            module.SetHold((int)BalanceFormulas.Cargo(dropParams.resource.ModuleSettings, slotSetting.cargo, dropParams.level, basePoints[2], colorInfo));

            module.SetCritDamage(BalanceFormulas.CritDamageBonus(dropParams.resource.ModuleSettings, 
                slotSetting.critDamageBonus, 
                dropParams.level, 
                Rand.Int(1, dropParams.resource.ModuleSettings.addPointMax), 
                colorInfo)
                );

            foreach (AdditionalParameter prm in GenerateAdditionalParameters(colorInfo)) {
                SetAddionalParameter(ref module, dropParams.resource.ModuleSettings, slotSetting, prm, colorInfo);
            }

            int[] skills = dropParams.resource.skillDropping.AllowedSkills(dropParams.workshop, dropParams.slotType, dropParams.level);

            if(skills.Length > 0 ) {
                module.SetSkill(skills[Rand.Int(0, skills.Length - 1)]);
            } else {
                module.SetSkill(-1);
            }

            module.SetSet(dropParams.set);
            return module;
        }

        public enum AdditionalParameter { resist, damage_bonus, energy_bonus, crit_chance, crit_damage, speed_bonus, hold_bonus }

        private List<AdditionalParameter> GenerateAdditionalParameters(ColorInfo colorInfo) {
            Dictionary<int, AdditionalParameter> prmIndices = new Dictionary<int, AdditionalParameter> {
                { 0, AdditionalParameter.resist },
                { 1, AdditionalParameter.damage_bonus },
                { 2, AdditionalParameter.energy_bonus },
                { 3, AdditionalParameter.crit_chance },
                { 4, AdditionalParameter.crit_damage },
                { 5, AdditionalParameter.speed_bonus },
                { 6, AdditionalParameter.hold_bonus }
            };

            int numAdditionalParameters = 0;
            switch(colorInfo.color) {
                case ObjectColor.blue:
                    numAdditionalParameters = 1;
                    break;
                case ObjectColor.yellow:
                    numAdditionalParameters = 2;
                    break;
                case ObjectColor.green:
                    numAdditionalParameters = 3;
                    break;
                case ObjectColor.orange:
                    numAdditionalParameters = 3;
                    break;
            }

            List<AdditionalParameter> result = new List<AdditionalParameter>();
            for(int i = 0; i < numAdditionalParameters; i++ ) {
                int prmIndex = Rand.Int(0, 6);
                result.Add(prmIndices[prmIndex]);
            }
            return result;
        }

        private void SetAddionalParameter(ref ShipModule module, ModuleSettingsRes res, ModuleSlotSettingData slotSetting,  AdditionalParameter prm, ColorInfo color) {
            int pointsMax = 10;

            
            switch(prm) {
                case AdditionalParameter.resist:
                    //module.SetResist(BalanceFormulas.ComputeRESISTANCE(Rand.Int(1, pointsMax), pointsMax, dropParams.level, dropParams.resource.Leveling.CapLevel(), slotSetting.resist_max, color.factor));
                    module.SetCommonResist(BalanceFormulas.Resistance(res, slotSetting.resist, dropParams.level, Rand.Int(1, res.addPointMax), color));
                    break;
                case AdditionalParameter.damage_bonus:
                    //module.SetDamageBonus(BalanceFormulas.ComputeDAMAGEBONUS(Rand.Int(1, pointsMax), slotSetting.damage_bonus_points_value, slotSetting.damage_bonus_points_factor, dropParams.level));
                    module.SetDamageBonus(BalanceFormulas.DamageBonus(res, slotSetting.damageBonus, dropParams.level, Rand.Int(1, res.addPointMax), color));
                    break;
                case AdditionalParameter.energy_bonus:
                    //module.SetEnergyBonus(BalanceFormulas.ComputeENERGYBONUS(Rand.Int(1, pointsMax), slotSetting.energy_bonus_points_value, slotSetting.energy_bonus_points_factor, dropParams.level));
                    module.SetEnergyBonus(BalanceFormulas.EnergyBonus(res, slotSetting.energyBonus, dropParams.level, Rand.Int(1, res.addPointMax), color));
                    break;
                case AdditionalParameter.crit_chance:
                    //module.SetCritChance(BalanceFormulas.ComputeCRITCHANCE(Rand.Int(1, pointsMax), slotSetting.critical_chance_points_value, slotSetting.critical_chance_points_factor, dropParams.level));
                    module.SetCritChance(BalanceFormulas.CritChance(res, slotSetting.critChanceBonus, dropParams.level, Rand.Int(1, res.addPointMax), color));
                    break;
                case AdditionalParameter.crit_damage:
                    //module.SetCritDamage(BalanceFormulas.ComputeCRITDAMAGEBONUS(Rand.Int(1, pointsMax), slotSetting.critical_damage_points_value, slotSetting.critical_damage_points_factor, dropParams.level));
                    module.SetCritDamage(BalanceFormulas.CritDamageBonus(res, slotSetting.critDamageBonus, dropParams.level, Rand.Int(1, res.addPointMax), color));
                    break;
                case AdditionalParameter.speed_bonus:
                    //module.SetSpeedBonus(BalanceFormulas.ComputeSPEEDBONUS(Rand.Int(1, pointsMax), slotSetting.speed_bonus_points_value, slotSetting.speed_bonus_points_factor, dropParams.level));
                    module.SetSpeedBonus(BalanceFormulas.SpeedBonus(res, slotSetting.speedBonus, dropParams.level, Rand.Int(1, res.addPointMax), color));
                    break;
                case AdditionalParameter.hold_bonus:
                    //module.SetHoldBonus(BalanceFormulas.ComputeCARGOBONUS(Rand.Int(1, pointsMax), slotSetting.cargo_bonus_points_value, slotSetting.cargo_bonus_points_factor, dropParams.level));
                    module.SetHoldBonus(BalanceFormulas.CargoBonus(res, slotSetting.cargoBonus, dropParams.level, Rand.Int(1, res.addPointMax), color));
                    break;
            }
        }

        //private ObjectColor preparedColor;
        //private bool usePreparedColor = false;

        //public void SetColor(ObjectColor color) {
        //    preparedColor = color;
        //    usePreparedColor = true;
        //}

        //private ColorInfo SelectColor(IRes resource) {
        //    if(usePreparedColor) {
        //        return resource.ColorRes.Color(ColoredObjectType.Module, preparedColor);
        //    }

        //    Dictionary<int, ObjectColor> colorIndices = new Dictionary<int, ObjectColor> {
        //        { 0, ObjectColor.orange },
        //        { 1, ObjectColor.green },
        //        { 2, ObjectColor.yellow },
        //        { 3, ObjectColor.blue },
        //        { 4, ObjectColor.white }
        //    };

        //    float[] probs = new float[5];
        //    int index = 0;
        //    float sum = resource.ColorRes.Orange(ColoredObjectType.Module).prob;
        //    probs[index++] = sum;
        //    sum += resource.ColorRes.Green(ColoredObjectType.Module).prob;
        //    probs[index++] = sum;
        //    sum += resource.ColorRes.Yellow(ColoredObjectType.Module).prob;
        //    probs[index++] = sum;
        //    sum += resource.ColorRes.Blue(ColoredObjectType.Module).prob;
        //    probs[index++] = sum;
        //    probs[index] = 1;

        //    int targetIndex = 4;
        //    float val = Rand.Float01();
        //    for(int i = 0; i < probs.Length; i++) {
        //        if( val < probs[i]) {
        //            targetIndex = i;
        //            break;
        //        }
        //    }

        //    return resource.ColorRes.Color(ColoredObjectType.Module, colorIndices[targetIndex]);

        //}
    }
}
