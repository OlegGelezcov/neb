
namespace Space.Game.Drop
{
    using System;
    using Common;
    using Space.Game.Inventory.Objects;
    using System.Collections.Generic;
    using Space.Game.Ship;
    using Space.Game.Resources;
    using GameMath;
    using Nebula.Balance;

    public class WeaponDropper : Dropper
    {
        private Dictionary<Difficulty, float> mDmgDiff = new Dictionary<Difficulty, float> {
            { Difficulty.starter, 0.5f },
            {  Difficulty.easy, 0.8f },
            {  Difficulty.easy2, 1f },
            {  Difficulty.medium, 1.5f },
            {  Difficulty.none, 1f },
            {  Difficulty.hard, 2f },
            {  Difficulty.boss, 3f },
            {  Difficulty.boss2, 4f }
        };

        public class WeaponDropParams
        {
            public readonly Workshop workshop;
            public readonly int level;
            public readonly Difficulty difficulty;
            public readonly IRes resource;
            public readonly WeaponDamageType damageType;

            public WeaponDropParams(IRes resource, int level, Workshop workshop, WeaponDamageType damageType, Difficulty difficulty)
            {
                this.resource = resource;
                this.level = level;
                this.workshop = workshop;
                this.damageType = damageType;
                this.difficulty = difficulty;
            }
        }

        private readonly WeaponDropParams dropParams;
        private readonly float mRemapWeight = 0;

        public WeaponDropper(WeaponDropParams paramaters, float remapWeight = 0f)
        {
            this.dropParams = paramaters;
            mRemapWeight = remapWeight;
        }

 
        public override IDroppable Drop()
        {
            return DropWeapon();
        }

        public WeaponObject DropWeapon(ColorInfo colorInfo) {
            WeaponData data = dropParams.resource.RandomWeapon(dropParams.workshop);
            if (data == null) {
                throw new Exception("not found weapon data");
            }

            WeaponWorkshopSetting setting;
            if (!dropParams.resource.WeaponSettings.TryGetSetting(dropParams.workshop, out setting)) {
                throw new Exception("Not found weapon settings");
            }

            int[] points = Rand.GenerateNumbers(2, 10);

            float damage = BalanceFormulas.ComputeWeaponDAMAGE(colorInfo.factor, setting.base_damage, setting.base_damage_factor,
                dropParams.level, points[0], setting.damage_points_value, setting.damage_points_factor) * mDmgDiff[dropParams.difficulty];

            float distance = BalanceFormulas.ComputeWeaponOPTIMALDISTANCE(colorInfo.factor, setting.base_optimal_distance, setting.base_optimal_distance_factor,
                dropParams.level, points[1], setting.optimal_distance_points_value, setting.optimal_distance_points_factor);

            //float critChance = 0; // 
            float critChance = colorInfo.factor * setting.base_crit_chance * Rand.Int(1, 10) * (((float)dropParams.level / 60.0f + 1));


            //if (colorInfo.isBetterThanWhite) {
            //    critChance = colorInfo.factor * setting.base_crit_chance * Rand.Int(1, 10);
            //}

            return new WeaponObject(Guid.NewGuid().ToString(),
                data.Id, dropParams.level, damage, distance, colorInfo.color, dropParams.damageType, critChance, (int)(byte)dropParams.workshop);
        }

        public WeaponObject DropWeapon() {

            ColorInfo colorInfo = SelectColor(dropParams.resource);
            return DropWeapon(colorInfo);

        }

        private ColorInfo SelectColor(IRes resource) {

            return resource.ColorRes.GenColor(ColoredObjectType.Weapon, mRemapWeight);

            /*
            Dictionary<int, ObjectColor> colorIndices = new Dictionary<int, ObjectColor> {
                { 0, ObjectColor.orange },
                { 1, ObjectColor.green },
                { 2, ObjectColor.yellow },
                { 3, ObjectColor.blue },
                { 4, ObjectColor.white }
            };

            float[] probs = new float[5];
            int index = 0;
            float sum = resource.ColorRes.Orange(ColoredObjectType.Weapon).prob;
            probs[index++] = sum;
            sum += resource.ColorRes.Green(ColoredObjectType.Weapon).prob;
            probs[index++] = sum;
            sum += resource.ColorRes.Yellow(ColoredObjectType.Weapon).prob;
            probs[index++] = sum;
            sum += resource.ColorRes.Blue(ColoredObjectType.Weapon).prob;
            probs[index++] = sum;
            probs[index] = 1;

            int targetIndex = 4;
            float val = Rand.Float01();
            for (int i = 0; i < probs.Length; i++) {
                if (val < probs[i]) {
                    targetIndex = i;
                    break;
                }
            }

            return resource.ColorRes.Color(ColoredObjectType.Weapon, colorIndices[targetIndex]);
            */
        }
    }
}
