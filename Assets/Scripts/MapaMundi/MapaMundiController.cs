using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapaMundiController : ControllerUnit, IUIController
{
    public Action<int> OnGameSceneIndexSelected;
    public Action OnBackToMainMenu;

    public override void Setup()
    {
        SetControllerFromParent<UIController>();
    }

    public override void Dispose()
    {
        gameObject.SetActive(false);
    }

    public void SelectSceneIndex(int buildIndex)
    {        
        OnGameSceneIndexSelected?.Invoke(buildIndex);
    }

    public void FireBackToMainMenuEvent()
    {
        OnBackToMainMenu?.Invoke();
    }
}
