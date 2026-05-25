using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathZone : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == ConstantStrings.PlayerTag)
        {
            var playerScript = other.gameObject.GetComponent<Penosa>();

            if(playerScript != null)
            {
                playerScript.PlayerLostALife();
                playerScript.Rigidbody2D.linearVelocity = Vector2.zero;
                if(playerScript.PlayerData.Lives > 0)
                    playerScript.gameObject.transform.position = PlayerEvents.GetPlayerStartPosition?.Invoke() ?? Vector2.zero;
            }
        }
    }
}
