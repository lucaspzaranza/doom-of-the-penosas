using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevicePopupPanelUI : MonoBehaviour
{
    public Action<List<InputDevicesSelector>> OnDeviceSelectorsAdded;
    public Action<bool> OnInputControlsPanelActivation;

    [SerializeField] private GameObject _deviceUnitPrefab;
    [SerializeField] private InputControlsPanel _inputControlsPanel;
    [SerializeField] private Transform _playerDevicesTransform;
    [SerializeField] private List<InputDevicesSelector> _inputDevicesSelectors;

    private void OnEnable()
    {
        GetPlayersDevices();
        SetDevicesSelectorsCallback();
    }

    private void OnDisable()
    {
        RemoveDeviceSelectorsCallbacks(); 
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

    private void SetDevicesSelectorsCallback()
    {
        foreach (var deviceSelector in _inputDevicesSelectors)
        {
            deviceSelector.DeviceButton.onClick.AddListener(() =>
            {
                _inputControlsPanel.gameObject.SetActive(true);

                if (deviceSelector.SelectedDevice.displayName.Contains(ConstantStrings.Keyboard))
                    _inputControlsPanel.ActivatePanel(ConstantStrings.Keyboard);
                else
                    _inputControlsPanel.ActivatePanel(ConstantStrings.Joystick);

                OnInputControlsPanelActivation?.Invoke(true);
            });
        }

        CloseButtonCallbackSetup();
    }

    private void CloseButtonCallbackSetup()
    {
        var closeBtn = _inputControlsPanel.CloseButton.GetComponent<Button>();

        closeBtn.onClick.AddListener(() =>
        {
            _inputControlsPanel.gameObject.SetActive(false);
            OnInputControlsPanelActivation?.Invoke(false);
        });
    }

    private void RemoveDeviceSelectorsCallbacks()
    {
        foreach (var deviceSelector in _inputDevicesSelectors)
        {
            deviceSelector.DeviceButton.onClick.RemoveAllListeners();
        }
    }
}
