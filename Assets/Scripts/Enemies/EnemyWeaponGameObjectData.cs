using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponGameObjectData : MonoBehaviour
{
    [SerializeField] private Transform _spawnTransform;
    public Transform SpawnTransform => _spawnTransform;

    [SerializeField] private SpriteRenderer _weaponSpriteRenderer;
    public SpriteRenderer WeaponSpriteRenderer => _weaponSpriteRenderer;

    [SerializeField] private bool _overwriteScale;
    public bool OverwriteScale => _overwriteScale;

    [DrawIfBoolEqualsTo("_overwriteScale", true)]
    [SerializeField] private Vector2 _weaponScale;

    private void OnEnable()
    {
        if(_weaponSpriteRenderer == null)
            _weaponSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetWeaponSprite(Sprite newSprite)
    {
        WeaponSpriteRenderer.sprite = newSprite;
        
        if(OverwriteScale)
            transform.localScale = new Vector2(_weaponScale.x, _weaponScale.y);
    }
}
