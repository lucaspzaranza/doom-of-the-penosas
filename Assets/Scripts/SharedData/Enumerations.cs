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
        /// <summary>
        /// When the game is in some menu.
        /// </summary>
        Menu = 0,

        /// <summary>
        /// Status when loading a scene.
        /// </summary>
        Loading = 1,

        /// <summary>
        /// When the game is into a stage and the players are active in.
        /// </summary>
        InGame = 2,

        /// <summary>
        /// When the game is over and the players must decide if they try again or give up.
        /// </summary>
        GameOver = 3,

        /// <summary>
        /// When the game is playing a cutscene.
        /// </summary>
        Cutscene = 4
    }

    public enum Penosas
    {
        None = -1,

        /// <summary>
        /// The Tanker Penosa.
        /// </summary>
        Geruza = 0,

        /// <summary>
        /// The Ninja Penosa.
        /// </summary>
        Dolores = 1
    }

    public enum SpecialItemType
    {
        Adrenaline = 0,
        JetCopter = 1,
        Mayday = 2,
        WalkTalk = 3
    }

    public enum RideArmorType
    {
        None = -1,
        EggTank = 0,
        TireMonoWheel = 1,
        JetSkinha = 2,
        Chickencopter = 3
    }

    public enum EnemyType
    {
        Land,
        Flying
    }

    public enum FlyingChaseMode
    {
        Horizontal,
        Vertical,
        Both
    }

    public enum EnemyState
    {
        Idle,
        Patrol,
        ChasingPlayer,
        Attacking,
    }

    public enum EnemyActionStatus
    {
        None,
        Started,
        Performed,
        Canceled
    }

    public enum FlipType
    {
        Horizontal,
        Vertical,
        Both
    }

    public enum EnemyFireType
    {
        Simultaneous,
        Individual
    }
}