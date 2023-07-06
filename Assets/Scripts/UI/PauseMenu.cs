using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SharedData.Enumerations;
using System;

public class PauseMenu : MonoBehaviour
{
    public static Action OnResume;
    public static Action OnBackToMainMenu;

    [SerializeField] private Button _resumeBtn;
    [SerializeField] private Button _backToMainMenuBtn;

    private int _frameCounter = 0;

    private void OnEnable()
    {
        _frameCounter = 0;
    }

    private void Update()
    {
        // The only way I found to assign these listeners without the PauseMenu button
        // be called when the user press it for the first time and deactivate it immediately
        if (_frameCounter == 1)
            ButtonsListenersSetup();

        if(_frameCounter <= 1)
            _frameCounter++;   
    }

    private void ButtonsListenersSetup()
    {
        _resumeBtn.onClick.AddListener(() =>
        {
            OnResume?.Invoke();
        });

        _backToMainMenuBtn.onClick.AddListener(() =>
        {
            OnBackToMainMenu?.Invoke();
        });
    }

    private void OnDisable()
    {
        _resumeBtn.onClick.RemoveAllListeners();
        _backToMainMenuBtn.onClick.RemoveAllListeners();
    }
}
