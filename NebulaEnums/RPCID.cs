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
    }
}
