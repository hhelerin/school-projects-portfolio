namespace GameBrain;

public enum EGameStatus
{
    Ongoing,
    XWon,
    OWon,
    GameOver,  // No winner; no moves left
    GameAbandoned,
    DoubleWin
}