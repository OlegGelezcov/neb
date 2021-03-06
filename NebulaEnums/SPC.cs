﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerClientCommon {

    /// <summary>
    /// Sub parameter code
    /// </summary>
    public enum SPC : int {
        ServerType          = 1,
        IpAddress           = 2,
        Protocol            = 3,
        Port                = 4,
        Index               = 5,
        Id                  = 6,
        Name                = 7,
        Level               = 8,
        Workshop            = 9,
        Deleted             = 10,
        Race                = 11,
        Exp                 = 12,
        Model               = 13,
        Group               = 14,
        GameRefId           = 15,
        DisplayName         = 16,
        SelectedCharacterId = 17,
        Characters          = 18,
        Count               = 19,
        Info                = 20,
        SlotsUsed = 21,
        MaxSlots = 22,
        Items = 23,
        ItemType = 24,
        Template = 25,
        NumSlots = 26,
        ProductionSpeed = 27,
        ProtectionInterval = 28,
        Health = 29,
        MaterialType = 30,
        Color = 31,
        CraftMaterials = 32,
        //HeavyShotInfo = 33,
        //LightShotInfo = 34,
        OptimalDistance = 35,
        Range = 36,
        CritChance = 37,
        DamageType = 38,
        MultiCount = 39,
        HasSkill = 40,
        UseTime = 41,
        IsOn = 42,
        Timer = 43,
        Cooldown = 44,
        SkillType = 45,
        Hold = 46,
        Inventory = 47,
        Type = 48,
        Set = 49,
        Speed = 50,
        Resist = 51,
        DamageBonus = 52,
        DistanceBonus = 53,
        Skill = 54,
        HoldType = 55,
        Energy = 56,
        EnergyRestoration = 57,
        Difficulty = 58,
        Prefab = 59,
        CritDamage = 60,
        HasWeapon = 61,
        //LightReady = 62,
        //HeavyReady=63,
        //HeavyCooldown = 64,
        //LightCooldown = 65,
        //DONT_USED_HitProb = 66,
        //LightTimer = 67,
        //HeavyTimer = 68,
        Source = 69,
        //HeavyDamage = 70,
        //LightDamage = 71,
        Damage = 72,
        ContainerId = 73,
        ContainerType = 74,
        ContainerItemId = 75,
        ContainerItemType = 76,
        ErrorMessageId = 77,
        IsGod = 78,
        SpecificInfo = 79,
        VarStageTimer = 80,
        VarPiratesKilled = 81,
        VarSecurityKilled = 82,
        VarStationsDestroyed = 83,
        VarBossKilled = 84,
        WorldId = 85,
        Variables = 86,
        VarKilledCounter = 87,
        VarDataCollected = 88,
        VarCollectedOre0001 = 89,
        VarCollectedOre0002 = 90,
        VarCollectedOre0003 = 91,
        VarCollectedOre0004 = 92,
        VarCollectedOre0005 = 93,
        VarCollectedOre0006 = 94,
        VarKillWave1 = 95,
        VarKillWave2 = 96,
        VarKillWave3 = 97,
        VarKillWave4 = 98,
        VarKillWave5 = 99,
        VarKillCounter1 = 100,
        VarKillCounter2 = 101,
        Description = 102,
        Credits = 103,
        Active = 104,
        Inputs = 105,
        Position = 106,
        CurrentStage = 107,
        Members = 108,
        Opened = 109,
        ChatMessageGroup = 110,
        ChatMessage = 111,
        ChatSourceLogin = 112,
        ChatMessageSourceName = 113,
        ChatMessageId = 114,
        ChatReceiverLogin = 115,
        ChatSourceName = 116 ,
        IsLeader = 117,
        Buffs = 118,
        ItemId = 119,
        RequestType = 120,
        Data = 121,
        FromCharacterId = 122,
        FromDisplayName = 123,
        ToExcludeCharacterId = 124,
        ToExcludeDisplayName = 125,
        //TargetId,
        TargetType = 126,
        DONT_USED_IsHitted = 127,
        ActualDamage = 128,
        DONT_USED_OverheatingGuns = 129,
        DONT_USED_FireBlocked = 130,
        DONT_USED_FireAllowed = 131,
        DONT_USED_IsCritical = 132,
        //ShotType,
        Message = 133,
        SourceType = 134,
        Target = 135,
        ChatMessageTime = 136,
        OwnedRace = 137,
        ZoneInfo = 138,
        StageId = 139,
        StartText = 140,
        TaskText = 141,
        IsFinal = 142,
        IsSuccess = 143,
        Timeout = 144,
        AttachedObject = 145,
        Mails = 146,
        SenderId = 147, 
        SenderType = 148,
        ReceiverId = 149,
        ReceiverType = 150,
        Title = 151,
        Body = 152,
        Attachments = 153,
        Readed = 154,
        Time = 155,
        SenderName = 156,
        ReceiverName = 157,
        RequireWeaponSlot = 158,
        Duration = 159,
        SlotType = 160,
        Radius = 161,
        Action = 162,
        InitialOwnedRace = 163,
        ZoneType = 164,
        SlotCount = 165,
        Tag = 166,
        CollectedContent = 167,
        Application = 168,
        MaxHealth = 169,
        Destroyed = 170,
        LastFireTime = 171,
        Ready = 172,
        FarProb = 173,
        NearProb = 174,
        FarDist = 175,
        NearDist = 176,
        MaxHitSpeed = 177,
        MaxFireDistance = 178,
        CurHealth = 179,
        Locations = 180,
        Content = 181,
        PlacingType = 182,
        RespondAction = 183,
        SourceServiceType = 184,
        Text = 185,
        Handled = 186,
        CharacterId = 187,
        Notifications = 188,
        Login = 189,
        Status = 190,
        GuildRating = 191,
        Guild = 192,
        Price = 193,
        ObjectType = 194,
        EnergyBonus = 195,
        SpeedBonus = 196,
        HoldBonus = 197,
        StartRace = 198,
        CurrentRace = 199,
        WorldType = 200,
        UnderAttack = 201,
        AttackRace = 202,
        RaceStatus = 203,
        Distance = 204,
        ReturnCode = 205,
        PlayerCount = 206,
        ShotID = 207,
        MinPrice = 208,
        MaxPrice = 209,
        MinLevel = 210,
        MaxLevel = 211,
        SubType = 212,
        ControlState = 213,
        HealValue = 214,
        ModerCount = 215,
        MaxModerCount = 216,
        CharacterName = 217,
        Points = 218,
        Voices = 219,
        RegistrationStartTime = 220,
        VoteStartTime = 221,
        VoteEndTime = 222,
        VotingStarted = 223,
        RegistrationStarted = 224,
        CurrentTime = 225,
        SourceGameRefID = 226,
        TargetGameRefID = 227,
        SourceLogin = 228,
        TargetLogin = 229,
        Binded = 230,
        Interval = 231,
        Value = 232,
        Capacity = 233,
        MoneyType = 234,
        Lang = 235,
        PostURL = 236,
        ImageURL = 237,
        IgnoreDamagaeAtStart = 238,
        IgnoreDamageInterval = 239,
        CreateChestOnKilling = 240,
        FixedInputDamage = 241,
        AdditionalHp = 242,
        Fraction = 243,
        Badge = 244,
        Size = 245,
        SubZoneID = 246,
        UseTargetHpForDamage = 247,
        TargetHpPercentDamage = 248,
        AlignWithForwardDirection = 249,
        RotationSpeed = 250,
        UseHitProbForAgro = 251,
        AttackMovingType = 252,
        Splittable = 253,
        Tier = 254,
        LearningStarted = 255,
        LearnStartTime = 256,
        LearnEndTime = 257,
        LearnProgress = 258,
        Free = 259,
        AssignedStationID = 260,
        AssignedStationType = 261,
        AssignedStationRace = 262,
        PlanetSlots = 263,
        MiningStationOwnedPlayer = 264,
        NebulaElementId = 265,
        MaxCount = 266,
        WarnCount = 267,
        MaxWarnCount = 268,
        PreviousRace = 269,
        Icon = 270,
        PvpPoints = 271,
        GuildName = 272,
        ShiftState = 273,
        Expire = 274,
        InapId = 275,
        DropList = 276,
        Skills = 277,
        PassiveSkill = 278,
        KilledTime = 279,
        Mastery = 280,
        Skin = 281,
        PetColor = 282,
        Pet = 283,
        MovedCount = 284,
        OwnerGameRef = 285,
        Total = 286,
        CurrentCount = 287,
        CurrentTotal = 288,
        ContractState = 289 ,
        ContractStage = 290,
        ContractCategory = 291,
        SourceWorld = 292,
        Counter = 293,
        TargetWorld = 294,
        Contract = 295,
        DeclineStart = 296,
        DeclineEnd = 297,
        AchievmentId = 298,
        AchievmentTierId = 299,
        SPEED_ControlIsMoving = 300,
        SPEED_IsStopped = 301,
        SPEED_ModelSpeed = 302,
        SPEED_BonusesAdd = 303,
        SPEED_PassiveAbilitiesAdd = 304,
        SPEED_AccelerationAdd = 305,

        RESIST_Blocked = 306,
        RESIST_ModelValue = 307,
        RESIST_BonusesAdd = 308,
        RESIST_PassiveBonusesAdd = 309,

        ShipParamName = 310,
        PrevId = 311,
        PrevType = 312,
        IsNew = 313,
        LoreRecords = 314,
        RocketDamage = 315,
        LaserDamage = 316,
        AcidDamage = 317,
        WeaponBaseType = 318,
        RocketResist = 319,
        AcidResist = 320,
        LaserResist = 321,
        DayPoster = 322,
        Transactions = 323,
        TargetCharacterId = 324,
        TargetCharacterName = 325,
        IPv6Address = 326,
        DepositedCount = 327,
        PlanetObjectType = 328,
        Row = 329,
        Column = 330,
        HasCellObject = 331,
        Cells = 332,
        OreId = 333,
        ShotState = 334,
        WeaponState = 335,
        SkillUseState = 336,
        Critical = 337,
        Var1 = 338,
        Var2 = 339,
        Var3 = 340,
        State = 341,
        CompletedQuests = 342,
        ActiveQuests = 343,
        CompletedDialogs = 344,
        Quest = 345,
        ConditionName = 346,
        ExpectedCount = 347,
        ActualCount = 348,
        VariableName = 349,
        Interactable = 350,
        StartedQuests = 351,
        NewQuest = 352,
        Module = 353
    }
}
