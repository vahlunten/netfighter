namespace AtariJetFighter.Messages
{

    /// <summary>
    /// Enumeration to represent different message types. 
    /// </summary>
    public enum UpdateMessageType : byte
    {
        UserControl,
        UpdateTransform,
        SpawnPlayer,
        SpawnProjectile,
        DestroyObject,
        UpdateScore,
        RestartGame,
        RoundUpdate
    }
}
