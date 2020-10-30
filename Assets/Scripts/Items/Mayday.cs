using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Mayday : SpecialItem
{
    public const byte maxMissiles = 5;
    private const float missileHeight = 4f;
    private const float xOffset = 1.5f;
    public GameObject missile;
    private bool itemInUse;
    private AssetBundle missileBundle;

    public override void GetItem<T>(Penosa player)
    {
        player.Inventory.AddItem<Mayday>();
    }

    private IEnumerator InstantiateMissiles(float interval)
    {
        itemInUse = true;
        float min = Camera.main.ViewportToWorldPoint(Camera.main.rect.min).x;
        float max = Camera.main.ViewportToWorldPoint(Camera.main.rect.max).x;
        float x = Camera.main.ViewportToWorldPoint(Camera.main.rect.min).x;        

        for(int i = 0; i < maxMissiles; i++)
        {
            Vector2 newPos = new Vector2(x, missileHeight);
            Instantiate(missile, newPos, missile.transform.rotation); 
            yield return new WaitForSeconds(interval);
            x += xOffset;
        }
        itemInUse = false;            
        if(parentSlot.Player.Inventory.SelectedSlot.Amount == 0) 
        {
            missileBundle.Unload(true);
            RemoveItemIfAmountEqualsZero();
        }
    }

    public override void Use()
    {     
        if(!itemInUse)
        {
            base.Use();
            if(missileBundle == null)
                missileBundle = AssetBundle.LoadFromFile("Assets/AssetBundles/projectiles");
            missile = missileBundle.LoadAsset<GameObject>("Missile");
            StartCoroutine(InstantiateMissiles(0.5f));            
        }
    }
}