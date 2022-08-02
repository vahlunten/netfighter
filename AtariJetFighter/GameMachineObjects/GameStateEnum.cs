namespace AtariJetFighter.GameMachineObjects
{
    /// <summary>
    /// This enum represents current state of the game.
    /// </summary>
    public enum GameStateEnum
    {
        /// <summary>
        /// Client is currently in game menu screen. 
        /// </summary>
        MainMenu,
        /// <summary>
        /// Game is in progress.
        /// </summary>
        GameRunning,
        /// <summary>
        /// Client had been disconnected from the server. 
        /// </summary>
        Disconnected,
        /// <summary>
        /// Client is trying to connect to the server.
        /// </summary>
        Connecting,
        /// <summary>
        /// Client failed to connect to server.
        /// </summary>
        FailedToConnect,


    }
}
