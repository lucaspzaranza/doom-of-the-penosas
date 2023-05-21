using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapaMundiController : ControllerUnit, IUIController
{
    public Action<int> OnSceneIndexSelected;

    public override void Setup()
    {
        SetControllerFromParent<UIController>();
    }

    public override void Dispose()
    {

    }

    public void SelectSceneIndex(int buildIndex)
    {
        OnSceneIndexSelected?.Invoke(buildIndex);
    }
}
