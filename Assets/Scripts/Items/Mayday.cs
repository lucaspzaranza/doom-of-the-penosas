using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Mayday : SpecialItem
{
    public const byte maxMissiles = 5;
    private const float missileHeight = 4f;
    private const float xOffset = 1.5f;
    private GameObject missile;

    private IEnumerator InstantiateMissiles(float interval)
    {
        ItemInUse = true;
        float x = Camera.main.ViewportToWorldPoint(Camera.main.rect.min).x;        

        for(int i = 0; i < maxMissiles; i++)
        {
            Vector2 newPos = new Vector2(x, missileHeight);
            Instantiate(missile, newPos, missile.transform.rotation);
            yield return new WaitForSeconds(interval);
            x += xOffset;
        }

        ItemInUse = false;            

        if(ItemSlot.Player.Inventory.SelectedSlot.Amount == 0) 
            RemoveItemIfAmountEqualsZero();
    }

    public override void Use()
    {
        missile = Inventory.MissilePrefab;
        base.Use();
        StartCoroutine(InstantiateMissiles(0.5f));            
    }

    public override void GetPlayerValues()
    {
        throw new System.NotImplementedException();
    }
}