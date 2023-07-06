using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public float rotationSpeed;
    private float oldX;
    private bool directionSetted = false;

    void FixedUpdate()
    {
        if(!directionSetted)
        {
            float newX = transform.position.x;
            rotationSpeed *= (newX < oldX)? -1 : 1;
            directionSetted = true;
        }

        transform.Rotate(Vector3.forward, rotationSpeed * Time.fixedDeltaTime);
    }

    private void LateUpdate()
    {
        if (directionSetted) return;

        oldX = transform.position.x;
    }
}
