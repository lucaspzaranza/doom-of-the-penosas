using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public GameObject[] huds = new GameObject[2];

    [Header("Game Over")]
    public GameObject GameOverContainerObject;
    public GameObject Title;
    public GameObject Countdown;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 2)
            huds[1].SetActive(true);
    }
}
