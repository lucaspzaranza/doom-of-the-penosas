using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public static Action<AutoRotate> OnSpinningProjectileEnabled;

    [SerializeField] private float _rotationSpeed;

    private void OnEnable()
    {
        OnSpinningProjectileEnabled?.Invoke(this);
    }

    void FixedUpdate()
    {
        transform.Rotate(Vector3.forward, _rotationSpeed * Time.fixedDeltaTime);
    }

    public void SetRotationDirection(int direction)
    {
        _rotationSpeed *= direction;
    }

    private void OnDisable()
    {
        // It always has to be negative by default;
        _rotationSpeed = -MathF.Abs(_rotationSpeed);
    }
}
