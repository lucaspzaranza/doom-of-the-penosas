using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggTank : RideArmor
{
    private const float upperLimitDegree = 90f;
    private const float lowerLimitDegree = 345f;

    [SerializeField] private GameObject _cannon;
    [SerializeField] private float _rotationSpeed;

    public override void Move(Vector2 direction)
    {
        RigiBody2DComponent.velocity = direction;
        if(direction.x != 0)
            transform.localPosition = new Vector2(0, transform.localPosition.y);

        if (direction.y != 0)
            print(direction.y);
    }

    public override void Eject()
    {
        base.Eject();
        RigiBody2DComponent.velocity = Vector2.zero;
    }

    public override void Shoot()
    {
        base.Shoot();

        print("PEI PEI PEI!");
    }

    public override void Aim(float direction)
    {
        base.Aim(direction);

        _cannon.transform.Rotate(Vector3.forward * direction * _rotationSpeed * Time.deltaTime);
        float eulerZ = _cannon.transform.localEulerAngles.z;

        if (direction == 1 && eulerZ > upperLimitDegree && eulerZ < lowerLimitDegree)
            _cannon.transform.localEulerAngles = new Vector3(0f, 0f,
                Mathf.Clamp(eulerZ, 0f, upperLimitDegree));
        else if (direction == -1 && eulerZ > upperLimitDegree && eulerZ < 360f && eulerZ < lowerLimitDegree)
            _cannon.transform.localEulerAngles = new Vector3(0f, 0f, lowerLimitDegree);
    }
}
