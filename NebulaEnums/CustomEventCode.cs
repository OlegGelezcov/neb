
namespace Common
{
    public enum CustomEventCode : byte { 
        Fire = 0,
        UseSkill = 1,
        GameEvent = 2,

        InventoryItemsAdded  = 8,   //when inventory item added
        //when added or removed modules to ship model
        ShipModelUpdated = 9,
        ServiceMessage = 10,
        StationHoldUpdated = 11,    //when added or removed items to station hold,
        InventoryUpdated = 12,      //when inventory updated
        PlayerInfoUpdated = 13,     //sended when player info updated
        SkillsUpdated = 14,          //sended when skills on player updated(after changing modules)
        PlayerWeaponUpdated = 15,
        ItemShipDestroyed = 16,
        MailBoxUpdated = 17,
        TargetUpdate = 18,
        JumpToPosition = 19,
        Activator = 20,
        ReceiveExp,
        WasKilled,
        UpdateCombatStats,
        TeleportJump,
        UpdateSubZone,
        Heal,
        InvisibilityChanged,
        SetPos,
        Resurrect,
        ModelChanged,
        UnderConstructChanged,
        PassiveBonusesUpdate,
        PAssiveBonusCompleted,
        DamageDron,
        HealDron,
        GetPlanetInfo,
        GetMiningStationInfo,
        WorldRaceChanged,
        PetSkillUsed,
        ResurrectByKillEffect,
        CollectChest,
        PetsUpdate
        //CooperativeGroupRequest = 18,
        //CooperativeGroupUpdate = 19
    }

}

