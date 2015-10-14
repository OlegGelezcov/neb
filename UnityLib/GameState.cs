namespace Nebula {
    /// <summary>
    /// The game state.
    /// </summary>
    public enum GameState {
        MasterConnected,
        MasterWaitingConnect,
        MasterDisconnected,

        LoginDisconnected,
        LoginWaitingConnect,
        LoginConnected,

        SelectCharacterDisconnected,
        SelectCharacterWaitingConnect,
        SelectCharacterConnected,

        NebulaGameWaitingConnect,
        NebulaGameConnected,
        NebulaGameWorldEntered,
        NebulaGameWorkshopEntered,
        NebulaGameChangingWorld,
        NebulaGameDisconnected
    }
}