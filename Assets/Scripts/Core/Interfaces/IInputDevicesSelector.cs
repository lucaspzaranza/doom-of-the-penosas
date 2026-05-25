using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInputDevicesSelector
{
    GameObject GameObject { get; }
    InputDevice SelectedDevice { get; }
}