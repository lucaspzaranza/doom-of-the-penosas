using Mirror;
using SharedData.Enumerations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

public class ArrowSelector : NetworkBehaviour
{
    private RectTransform lobbyMenuTransform;

    public override void OnStartClient()
    {
        base.OnStartClient();

        lobbyMenuTransform = GameObject.Find("Lobby Menu").GetComponent<RectTransform>(); 
        gameObject.transform.SetParent(lobbyMenuTransform, false);
    }
}