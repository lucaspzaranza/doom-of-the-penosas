using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallFromPlatform : MonoBehaviour
{
    private Collider2D _collider;
    private List<Penosa> _playersOnPlatform = new List<Penosa>();

    void Start()
    {
        _collider = GetComponent<Collider2D>();   
    }

    void Update()
    {
        
    }

    public void AddPlayerOnPlatform(Penosa player)
    {
        if(!_playersOnPlatform.Contains(player))
            _playersOnPlatform.Add(player);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if(other.gameObject.tag == ConstantStrings.PlayerTag)
        {
            var player = other.gameObject.GetComponent<Penosa>();
            _playersOnPlatform.Remove(player);
        }
    }
}
