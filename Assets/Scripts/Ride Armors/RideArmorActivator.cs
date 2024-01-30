using SharedData.Enumerations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideArmorActivator : MonoBehaviour
{
    [SerializeField] private RideArmor _rideArmor;
    public RideArmor RideArmor => _rideArmor;

    private void OnEnable()
    {
        if(_rideArmor == null)
            _rideArmor = GetComponentInParent<RideArmor>();
    }
}
