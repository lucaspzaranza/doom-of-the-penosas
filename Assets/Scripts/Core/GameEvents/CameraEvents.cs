using System;
using UnityEngine;

public static class CameraEvents
{
    public static Action OnPlayerInCameraEdge;
    public static Action OnPlayerOutCameraEdge;
    public static Action OnResetCameraBounds;
    public static Action<Transform> OnPlayerReadyToFollow;
}