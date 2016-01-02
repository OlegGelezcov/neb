using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common {
    public enum RPCErrorCode : int {
        Ok = 0,
        ObjectNotFound = 1000,
        DistanceIsFar  = 1001,
        LevelNotEnough = 1002,
        InvalidObjectType = 1003,
        InvalidObjectWorkshop = 1004,
        LowInventorySpace = 1005,
        FailAddToStation = 1006,
        InvalidRace = 1007,
        InCombat = 1008,
        AlreadyExists = 1009,
        VeryCloseToSpawnPoint = 1010,
        NeedConstructOutpostBefore = 1011,
        MaxCountOfFortificationsReached = 1012,
        VeryCloseToOtherSystemConstructions = 1013,
        InvalidWorldRace = 1014,
        TurretCountLimitReached = 1015,
        NeedMoreFortifications = 1016,
        OutpostNotFound = 1017,
        ComponentNotFound = 1018,
        LevelInNotRange = 1019,
        FortificationNotFound = 1020,
        MaxCountOfBeaconsReached = 1021,
        PlanetNotFounded = 1022,
        SlotNotFree = 1023,
        AccessDenied = 1024,
        MaxTierReached = 1025,
        DontEnoughInventoryItems = 1026,
        LearningAlreadyStarted = 1027,
        UnableRemovePvpPointsFromBalance = 1028,
        DontEnoughPvpPoints = 1029,
        InvalidInventoryType = 1030,
        ItemNotFound = 1031,
        DropListNotFound = 1032,
        NeedFreeSlots = 1033,

        UnknownError = 10002,
        
    }
}
