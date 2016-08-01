using GameMath;
using Space.Game.Resources;
using System;

namespace Nebula.Balance {
    public static class BalanceFormulas {

        /*
        public static float ComputeHP(float techFactor, float baseHP, float baseHPFactor, int level, int points, float pointValue, float pointsFactor) {
            
            
            //float first  = techFactor * baseHP * Mathf.Pow(baseHPFactor, level - 1);
            //float second = points * pointValue * Mathf.Pow(pointsFactor, level - 1);
            //return (first + second);
            
            
            float first = techFactor * baseHP * baseHPFactor*((float)level / 12.0f + 1);
            float second = points * pointValue * pointsFactor * ((float)level / 12.0f + 1);
            return (first + second);
        }*/

        public static float ComputeBase(ModuleSettingsRes res, BaseParam param, int level, int point, ColorInfo color) {
            float first = param.baseVal * (1.0f + param.levelMult * (float)level / (float)res.maxLevel);
            float second = (1.0f + param.pointMult * (float)point / (float)res.hpSpeedCargoPtMax);
            return (first + second) * color.factor * Rand.Float(param.randMin, param.randMax);
        }
        public static float Hp(ModuleSettingsRes res, BaseParam param, int level, int point, ColorInfo color) {
            return ComputeBase(res, param, level, point, color);
        }

        /*
        public static float ComputeSPEED(float techFactor, float baseSpeed, float baseSpeedFactor, int level, int points, float pointsValue, float pointsFactor) {
            
            //float first = techFactor * baseSpeed * Mathf.Pow(baseSpeedFactor, level - 1);
            //float second = points * pointsValue * Mathf.Pow(pointsFactor, level - 1);
            //return (first + second); 

            
            float first = techFactor * baseSpeed * baseSpeedFactor * ((float)level / 60.0f + 1);
            float second = points * pointsValue * pointsFactor * ((float)level / 60.0f + 1);
            return (first + second);

        }*/

        public static float Speed(ModuleSettingsRes res, BaseParam param, int level, int point, ColorInfo color) {
            return ComputeBase(res, param, level, point, color);
        }

        /*
        public static int ComputeCARGO(float techFactor, float baseCargo, float baseCargoFactor, int level, int points, float pointsValue ) {
            
            
            //float first = techFactor * baseCargo * Mathf.Pow(baseCargoFactor, level - 1);
            //float second = points * pointsValue * 0.33333F;
            //return (int)Math.Round(first + second);

            
            float first = techFactor * baseCargo * baseCargoFactor * ((float)level / 12.0f + 1);
            float second = points * pointsValue * 0.5f;
            return (int)Math.Round(first + second);
        }*/

        public static float Cargo(ModuleSettingsRes res, BaseParam param, int level, int point, ColorInfo color) {
            return ComputeBase(res, param, level, point, color);
        }
        //=========================================================================================================

      
        public static float ComputeAdditional(ModuleSettingsRes res, AddParam param, int level, int point, ColorInfo color ) {
            float val = color.factor * point * (2.0f * (float)level / (float)(level + res.maxLevel)) * (param.max / res.addPointMax) * Rand.Float(param.randMin, param.randMax);
            if(val > param.max) {
                val = param.max;
            }
            return val;
        }

        /*
        public static float ComputeRESISTANCE(int points, int pointsMax,  int level, int levelMax,  float resistMax, float colorMult) {
            //return points * pointsValue * Mathf.Pow(pointsFactor, level - 1);
            //return points * pointsValue * pointsFactor * ((float)level / 12.0f + 1);
            float resistance = colorMult * points * ( 2.0f * (float)level / (float)(level + levelMax) )* (resistMax / pointsMax) * Rand.Float(0.4f, 1.0f);
            if(resistance > resistMax ) {
                resistance = resistMax;
            }
            return resistance;
        }*/

        public static float Resistance(ModuleSettingsRes res, AddParam param, int level, int point, ColorInfo color) {
            return ComputeAdditional(res, param, level, point, color);
        }

        /*
        public static float ComputeDAMAGEBONUS(int points, float pointsValue, float pointsFactor, int level) {
            //return points * pointsValue * Mathf.Pow(pointsFactor, level - 1);
            return points * pointsValue * pointsFactor * ((float)level / 12.0f + 1);
        }*/

        public static float DamageBonus(ModuleSettingsRes res, AddParam param, int level, int point, ColorInfo color) {
            return ComputeAdditional(res, param, level, point, color);
        } 

        /*
        public static float ComputeCARGOBONUS(int points, float pointsValue, float pointsFactor, int level) {
            //return points * pointsValue * Mathf.Pow(pointsFactor, level - 1);
            return points * pointsValue * pointsFactor * ((float)level / ((float)level + 1));
        }*/

        public static float CargoBonus(ModuleSettingsRes res, AddParam param, int level, int point, ColorInfo color) {
            return ComputeAdditional(res, param, level, point, color);
        }

        /*
        public static float ComputeENERGYBONUS(int points, float pointsValue, float pointsFactor, int level) {
            //return points * pointsValue * Mathf.Pow(pointsFactor, level - 1);
            return points * pointsValue * pointsFactor * ((float)level / 12.0f + 1.0f);
        }*/

        public static float EnergyBonus(ModuleSettingsRes res, AddParam param, int level, int point, ColorInfo color) {
            return ComputeAdditional(res, param, level, point, color);
        }

        /*
        public static float ComputeSPEEDBONUS(int points, float pointsValue, float pointsFactor, int level ) {
            //return points * pointsValue * Mathf.Pow(pointsFactor, level - 1);
            return points * pointsValue * pointsFactor * ((float)level / 12.0f + 1.0f);
        }*/
        public static float SpeedBonus(ModuleSettingsRes res, AddParam param, int level, int point, ColorInfo color) {
            return ComputeAdditional(res, param, level, point, color);
        }

        /*
        public static float ComputeCRITCHANCE(int points, float pointsValue, float pointsFactor, int level) {
            //return points * pointsValue * Mathf.Pow(pointsFactor, level - 1);
            return points * pointsValue * pointsFactor * ((float)level / 60.0f + 1.0f);
        }*/

        public static float CritChance(ModuleSettingsRes res, AddParam param, int level, int point, ColorInfo color) {
            return ComputeAdditional(res, param, level, point, color);
        }

        /*
        public static float ComputeCRITDAMAGEBONUS(int points, float pointsValue, float pointsFactor, int level) {
            //return points * pointsValue * Mathf.Pow(pointsFactor, level - 1);
            return points * pointsValue * pointsFactor * ((float)level / ((float)level + 1.0f));
        }*/

        public static float CritDamageBonus(ModuleSettingsRes res, AddParam param, int level, int point, ColorInfo color) {
            return ComputeAdditional(res, param, level, point, color);
        }


        //======================================================================================================

        public static float ComputeBaseWeapon(WeaponDropSettings res, BaseParam param, int level, int point, ColorInfo color) {
            float first = param.baseVal * (1.0f + param.levelMult * (float)level / (float)res.maxLevel);
            float second = (1.0f + param.pointMult * (float)point / (float)res.damageOdPtMax);
            return (first + second) * color.factor * Rand.Float(param.randMin, param.randMax);
        }

        public static float ComputeAdditionalWeapon(WeaponDropSettings res, AddParam param, int level, int point, ColorInfo color) {
            float val = color.factor * point * (2.0f * (float)level / (float)(level + res.maxLevel)) * (param.max / 10.0f) * Rand.Float(param.randMin, param.randMax);
            if (val > param.max) {
                val = param.max;
            }
            return val;
        }

        /*
        public static float ComputeWeaponDAMAGE(float techFactor, float baseDamage, float baseDamageFactor, int level, int damagePoints, float damagePointsValue, float damagePointsFactor) {
            
            
            //float first = techFactor * baseDamage * Mathf.Pow(baseDamageFactor, level - 1);
            //float second = damagePoints * damagePointsValue * Mathf.Pow(damagePointsFactor, level - 1);
            //return (first + second);

            
            float first = techFactor * baseDamage * baseDamageFactor * ((float)level / 12.0f + 1);
            float second = damagePoints * damagePointsValue * damagePointsFactor * ((float)level / 12.0f + 1);
            return (first + second);
        }*/


        public static float WeaponDamage(WeaponDropSettings res, BaseParam param, int level, int point, ColorInfo color) {
            return ComputeBaseWeapon(res, param, level, point, color);
        }

        public static float WeaponOptimalDistance(WeaponDropSettings res, BaseParam param, int level, int point, ColorInfo color) {
            return ComputeBaseWeapon(res, param, level, point, color);
        }

        public static float WeaponCriticalChance(WeaponDropSettings res, AddParam param, int level, int point, ColorInfo color) {
            return ComputeAdditionalWeapon(res, param, level, point, color);
        }
        /*
        public static float ComputeWeaponOPTIMALDISTANCE(float techFactor, float baseOptimalDistance, float baseOptimalDistanceFactor, int level, int distancePoints, float distancePointsValue, float distancePointsFactor) {
            
            
            //float first = techFactor * baseOptimalDistance * Mathf.Pow(baseOptimalDistanceFactor, level - 1);
            //float second = distancePoints * distancePointsValue * Mathf.Pow(distancePointsFactor, level - 1);
            //return (first + second);

            
            float first =  baseOptimalDistance * baseOptimalDistanceFactor * ((float)level / 12.0f + 1);
            float second = distancePoints * distancePointsValue * distancePointsFactor * ((float)level / 12.0f + 1);
            return (first + second);
        }*/

        public static float RemapParameter(int level, int groupCount ) {
            float baseVal = 0;
            switch(groupCount) {
                case 1:
                    baseVal = 0f;
                    break;
                case 2:
                    baseVal = 0.05f;
                    break;
                case 3:
                    baseVal = 0.1f;
                    break;
                case 4:
                    baseVal = 0.15f;
                    break;
                     
            }

            return baseVal + level * 0.002f;
        }
    }
}
