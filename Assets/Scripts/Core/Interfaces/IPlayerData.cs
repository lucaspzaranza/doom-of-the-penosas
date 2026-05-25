using SharedData.Enumerations;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IPlayerData
{
    event Action<int> OnLifeChanged;
    event Action<int> OnLivesChanged;
    event Action<WeaponType, int> OnWeaponLevelChanged;
    event Action<WeaponType, int> OnWeaponAmmoChanged;

    IPlayerCharacter Player { get; }
    Penosas Character { get; }
    InputDevice InputDevice { get; set; }
    byte LocalID { get; set; }
    bool GameOver { get; set; }
    int Continues { get; set; }
    int Lives { get; set; }
    IInventoryData InventoryData { get; }
    byte _1stWeaponLevel { get; set; }
    int _1stWeaponAmmoProp { get; set; }
    byte _2ndWeaponLevel { get; set; }
    int _2ndWeaponAmmoProp { get; set; }
    GameObject PlayerGameObject { get; }
    GameObject Current1stShot { get; }
    GameObject Current2ndShot { get; }

    void SetPlayerScriptFromInstance(IPlayerCharacter playerScript);
    void SetPlayerGameObjectFromInstance(GameObject playerGameObject);
    void InventoryDataSetup(IPlayerCharacter player, bool isNewGame);
    void FireOnLifeChanged(int newLife);
}