using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateArrowPositionEventArgs
{
    public GameObject PreviousButton { get; private set; }

    public UpdateArrowPositionEventArgs() { }

    public UpdateArrowPositionEventArgs(GameObject previousButton)
    {
        PreviousButton = previousButton;
    }
}

public class SelectButtonEventArgs
{
    public SelectButtonEventArgs() { }

    public SelectButtonEventArgs(int index, Button buttonPressed)
    {
        Index = index;
        ButtonPressed = buttonPressed;
    }

    public int Index { get; private set; }
    public Button ButtonPressed { get; private set; }
}