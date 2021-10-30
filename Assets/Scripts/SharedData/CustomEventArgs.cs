using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateArrowPositionEventArgs
{
    public NetworkArrowPosition NetworkArrowPosition { get; private set; }
    public GameObject PreviousButton { get; private set; }

    public UpdateArrowPositionEventArgs() { }

    public UpdateArrowPositionEventArgs(NetworkArrowPosition networkArrowPosition, GameObject previousButton)
    {
        NetworkArrowPosition = networkArrowPosition;
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