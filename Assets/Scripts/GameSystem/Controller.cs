using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour, IController
{
    public virtual void Setup() { }
    public virtual void Dispose() { }
}
