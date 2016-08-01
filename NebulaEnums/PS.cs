﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Common
{
    public enum PS : byte
    {
        MechanicalShieldFullDamageAbsorb = 0,
        MechanicalShieldCurrentDamageAbsorb = 1,
        MechanicalShieldEnabled = 2,

        PowerFieldShieldFullDamageAbsorb = 3,
        PowerFieldShieldRecoverSpeed = 4,
        PowerFieldShieldCurrentPowerPercent = 5,
        PowerFieldShieldEnabled = 6,

        CurrentHealth = 7,
        MaxHealth = 8,
        Destroyed = 9,
        Model = 10,
        AIState = 11,
        ControlState = 12,
        ActionState = 13,
        Acceleration = 14,
        MinLinearSpeed = 15,
        MaxLinearSpeed = 16,
        CurrentLinearSpeed = 17,
        AngleSpeed = 18,
        CurrentEnergy = 19,
        MaxEnergy = 20,
        Workshop = 21,

        WeaponReloadInterval = 22,

        LightShotReady = 23,
        HeavyShotReady = 24,
        Damage = 25,
        LightShotReloadTimer = 26,
        HeavyShotReloadTimer = 27,
        HitProb = 28,
        MinHitProb = 29,
        OptimalDistance = 30,
        WeaponRange = 31,
        ProbFar2OptimalDistance = 32,
        ProbNear2OptimalDistance = 33,
        MaxHitSpeed = 34,
        MaxFireDistance = 35,

        IsStatic = 36,
        TargetType = 37,
        HasTarget = 38,
        TargetId = 39,
        InterestAreaAttached = 40,
        ViewDistanceEnter = 41,
        ViewDistanceExit = 42,
        BotSubType = 43,
        Level = 44,
        TypeName = 45,
        Race = 46,
        Description = 47,
                
        ModulePrefabs = 48,
        UseSkill = 49,
        AsteroidContent = 50,
        AsteroidData = 51,
        Bonuses = 52,

        ShiftPressed = 53,
        FromEvent = 54,
        EventId = 55,
        EventWorldId = 56,
        
        //HasTarget = 57,
        //TargetId = 58,
        //TargetType = 59,
        PlanetType = 57,
        Ship = 58,
        Difficulty = 59,
        SubType = 60,
        Info = 61,
        ModelInfo = 62,
        Name = 63,
        LightCooldown = 64,
        HeavyCooldown = 65,
        Fraction = 66,
        Event = 67,
        CharacterID = 68,
        Login = 69,
        Radius = 70,
        Active = 71,
        IgnoreDamage = 72,
        IgnoreDamageTimer = 73,
        RaceStatus = 74,
        DataId = 75,
        Resist = 76,
        CritDamage = 77,
        CritChance = 78,
        InCombat = 79,
        SubZoneID = 80,
        InnerRadius = 81,
        OuterRadius = 82,
        Invisibility = 83,
        UnderConstruction = 84,
        ConstructionTimer = 85,
        CharacterName = 86,
        GuildName = 87,
        GuildId = 88,
        OwnerId = 89,
        Timer = 90,
        BotGroup = 91,
        ContractId = 92,
        Mark = 93,
        Group = 94,
        Icon = 95,
        WeaponState = 96,
        SkillsBlocked = 97
    }
}
