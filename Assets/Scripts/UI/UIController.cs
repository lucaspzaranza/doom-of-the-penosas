using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    [Header("Game Over")]
    public GameObject GameOverContainerObject;
    public GameObject Title;
    public GameObject Countdown;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
}
