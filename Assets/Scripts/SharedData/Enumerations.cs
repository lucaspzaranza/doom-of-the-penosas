using System.Collections;
using System.Collections.Generic;

namespace SharedData.Enumerations
{
    /// <summary>
    /// Enum to get if the game is in Singleplayer or Multiplayer mode.
    /// </summary>
    public enum GameMode
    {
        Singleplayer = 0,
        Multiplayer = 1
    }

    public enum LobbyState
    {
        GameModeSelection = 0,
        PlayerSelection = 1,
        ReadyToStart = 2
    }

    public enum GameStatus
    {
        Menu = 0,
        Loading = 1,
        InGame = 2,
        GameOver = 3,
        Cutscene = 4
    }

    public enum Penosas
    {
        None = -1,
        Geruza = 0,
        Dolores = 1
    }

    public enum SpecialItemType
    {
        Adrenaline = 0,
        JetCopter = 1,
        Mayday = 2,
        WalkTalk = 3
    }
}