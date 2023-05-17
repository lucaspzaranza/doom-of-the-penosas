using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControllerBase : MonoBehaviour, IController
{
    public virtual void Setup() { }
    public virtual void Dispose() { }
}
