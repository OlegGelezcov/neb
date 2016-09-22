namespace Common {
    public static class RPCID {
        public const int rpc_ProposeContract = 1;
        public const int rpc_RestartLoop = 2;
        public const int rpc_AcceptContract = 3;
        public const int rpc_DeclineContract = 4;
        public const int rpc_CompleteContract = 5;
        public const int rpc_TestAddContractItems = 6;
        public const int rpc_TestRemoveContractItems = 7;
        public const int rpc_GetAchievments = 8;
        public const int rpc_GetParamDetail = 9;

        public const int rpc_sc_SetRaceStatus = 10;

        public const int rpc_SetPlayerMark = 11;
        public const int rpc_ResetNew = 12;
        public const int rpc_TestKill = 13;
        public const int rpc_UnlockLore = 14;

        public const int rpc_sc_DepositCreditsToCoalition = 15;
        public const int rpc_sc_DepositPvpPointsToCoalition = 16;
        public const int rpc_sc_WithdrawCreditsFromCoalition = 17;
        public const int rpc_sc_WithdrawPvpPointsFromCoalition = 18;
        public const int rpc_sc_SetCoalitionPoster = 19;

        /// <summary>
        /// Unlock all lore records instantly
        /// </summary>
        public const int rpc_TestUnlockFullLore = 20;

        public const int rpc_StartAsteroidCollecting = 21;

        /// <summary>
        /// Set force dispose flag ( for don't waiting 10 seconds disconnect from world)
        /// </summary>
        public const int rpc_ForceDispose = 22;

        /// <summary>
        /// Use single credits bag from station
        /// </summary>
        public const int rpc_UseCreditsBag = 23;

        public const int rpc_CreateCommandCenter = 24;

        public const int rpc_GetCells = 25;

        public const int rpc_CreatePlanetObjectTurret = 26;
        public const int rpc_CreatePlanetObjectResourceHangar = 27;
        public const int rpc_CreatePlanetObjectResourceAccelerator = 28;
        public const int rpc_CreatePlanetObjectMiningStation = 29;
        public const int rpc_resetSystemToNeutral = 30;
        public const int rpc_CollectOreFromPlanetMiningStation = 31;
        public const int rpc_CreateTestSharedChest = 32;
        public const int rpc_MoveAllFromInventoryToStationWithExclude = 33;
        public const int rpc_TestStun = 34;
        public const int rpc_TestAreaInvisibility = 35;
        public const int rpc_GetQuests = 36;
        public const int rpc_CompleteQuest = 37;
        public const int rpc_GetDialogs = 38;
        public const int rpc_UserEvent = 39;
        public const int rpc_ResetQuests = 40;
        public const int rpc_Activate = 41;
        public const int rpc_RestartQuest = 42;
        public const int rpc_UseQuestItem = 43;
    }
}
