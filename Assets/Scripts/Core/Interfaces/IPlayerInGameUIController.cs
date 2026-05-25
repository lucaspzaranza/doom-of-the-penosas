using System;
using System.Collections.Generic;

public interface IPlayerInGameUIController
{
    event Action<byte> OnCountdownIsOver;

    void Dispose();
    void UpdateHUDWithRideArmor(byte playerID, IRideArmor rideArmor, bool isEquipping);
    void SetGameOverContainerOnPlayerActive(byte playerID, bool val);
    void DestroyAllHUDs();
    void CreatePlayersHUDs(IList<IPlayerData> playersData);
    void ContinueActivation(byte playerID, bool val);
}