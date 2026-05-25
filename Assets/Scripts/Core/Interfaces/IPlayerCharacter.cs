using UnityEngine;
using System;
using SharedData.Enumerations;

public interface IPlayerCharacter
{
    event Action<byte> OnPlayerLostAllLives;
    event Action<byte> OnPlayerRespawn;
    event Action<byte> OnPlayerDeath;
    event Action<int> OnArmorLifeChanged;
    Action<byte> OnPlayerLostAllContinues { get; set; }

    /// <summary>
    /// The boolean is to send to the event if you are equipping the rideArmor or ejecting it. <br/>
    /// True for equipping, False for ejecting.
    /// </summary>
    event Action<byte, IRideArmor, bool> OnPlayerRideArmor;

    int Life { get; set; }
    int ArmorLife { get; set; }
    IInventory Inventory { get; }
    IPlayerData PlayerData { get; set; }
    Transform Transform { get; }
    GameObject GameObject { get; }
    GameObject CurrentGrenade { get; }
    Rigidbody2D Rigidbody2D { get; }
    GameObject JetCopterObject { get; }
    bool IsLeft { get; }
    bool RideArmorEquipped { get; }
    bool JetCopterActivated { get; set; }
    bool Adrenaline { get; }
    bool enabled { get; set; }
    Animator Animator { get; }

    void SetPlatform(IPlatform platform);
    void SetPlayerData(IPlayerData newPlayerData);
    void SetPlayerController(IPlayerController controller);
    void SetCameraSelector(ICameraSelector cameraSelector);
    void SetPlayerOnSceneAfterGameOver(bool val);
    void ResetPlayerData();
    void InitiateBlink();
    void Flip();
    void RideArmor(IRideArmor rideArmorToEquip);
    void SetAmmo(WeaponType weaponType, int ammo);
    // T GetComponent<T>();
}