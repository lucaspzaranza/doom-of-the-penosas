using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevicePopupPanelUI : MonoBehaviour
{
    public Action<List<InputDevicesSelector>> OnDeviceSelectorsAdded;

    [SerializeField] private GameObject _deviceUnitPrefab;
    [SerializeField] private Transform _playerDevicesTransform;
    [SerializeField] private List<InputDevicesSelector> _inputDevicesSelectors;

    private void OnEnable()
    {
        GetPlayersDevices();
    }

    private void GetPlayersDevices()
    {
        var gameCtrl = FindAnyObjectByType<GameController>();

        if (gameCtrl == null)
            return;

        if(gameCtrl.GameMode == GameMode.Multiplayer)
        {
            // Enter this if-clause when it still needs to instantiate more playerDevicesUnits
            if (_inputDevicesSelectors.Count < ConstantNumbers.NumberOfPlayers) 
            {
                for (int i = _inputDevicesSelectors.Count; i < ConstantNumbers.NumberOfPlayers; i++)
                {
                    var newPlayerDeviceObj = Instantiate(_deviceUnitPrefab, _playerDevicesTransform);
                    var inputDeviceSelector = newPlayerDeviceObj.GetComponent<InputDevicesSelector>();
                    _inputDevicesSelectors.Add(inputDeviceSelector);
                    inputDeviceSelector.SetPlayerName($"{ConstantStrings.PlayerTag} {_inputDevicesSelectors.Count}:");
                }
            }
            else
                InputDevicesSetActive(true);
        }
        else if (_inputDevicesSelectors.Count > 1) 
            InputDevicesSetActive(false);

        OnDeviceSelectorsAdded?.Invoke(_inputDevicesSelectors);
    }

    private void InputDevicesSetActive(bool val)
    {
        // The first instance will be always activated and doesn't need to activate/deactivate
        for (int i = 1; i < _inputDevicesSelectors.Count; i++)
        {
            _inputDevicesSelectors[i].gameObject.SetActive(val);
        }
    }
}
