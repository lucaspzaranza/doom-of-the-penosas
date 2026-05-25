using UnityEngine;

public interface IGameController
{
  public IPlayerController PlayerController { get; }
  GameObject GetProjectileFromPool(GameObject projectile);
}