using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpriteAddedEventArgs : EventArgs
{
    public SpriteAddedEventArgs(Sprite sprite)
    {
        NewSprite = sprite;
    }

    public Sprite NewSprite {get; set;}
}
