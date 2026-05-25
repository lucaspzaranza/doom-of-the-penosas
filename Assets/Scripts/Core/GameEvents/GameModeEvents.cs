using SharedData.Enumerations;
using System;

public static class GameModeEvents
{
    public static Action<GameMode> OnGameModeSet;
    public static Func<GameMode> RequestGameMode;
}