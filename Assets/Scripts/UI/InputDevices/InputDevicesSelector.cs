using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class InputDevicesSelector : MonoBehaviour
{
    private static int instancesCounter;

    [SerializeField] private TextMeshProUGUI _playerName;
    [SerializeField] private TextMeshProUGUI _deviceTMPRO;
    [SerializeField] private List<InputDevice> _devices;
    [SerializeField] private GameObject _leftArrow;
    [SerializeField] private GameObject _rightArrow;

    public InputDevice SelectedDevice => _devices[_currentIndex];

    [SerializeField] private Button _deviceButton; 
    public Button DeviceButton => _deviceButton;

    private int _currentIndex;
    private int CurrentIndex
    {
        get => _currentIndex;
        set
        {
            _currentIndex = Mathf.Clamp(value, 0, _devices.Count - 1);
        }
    }

    private bool _changeDeviceBtnPressed = false;
    private List<string> _devicesNames = new List<string>();

    private void OnEnable()
    {
        _devices = InputSystem.devices.Where(device => device.displayName != ConstantStrings.Mouse).ToList();
        if(_devices.Count > 1)
            _rightArrow.SetActive(true);   

        foreach (var device in _devices)
        {
            var repeatedDevices = _devices.Where(repeatedDevice => 
                repeatedDevice.displayName == device.displayName).ToList();

            if (repeatedDevices.Count > 1)
            {
                string deviceIndex = device.name[device.name.Length - 1].ToString();
                int.TryParse(deviceIndex, out int index);
                _devicesNames.Add(device.displayName + " " + (index + 1));
            }
            else
                _devicesNames.Add(device.displayName);
        }

        SelectDevice(instancesCounter);
        CurrentIndex = instancesCounter;

        if(instancesCounter < _devices.Count - 1)
            instancesCounter++;

        CursorPosition.OnCursorMoved += ChangeDevice;
        CursorPosition.OnCursorReleased += HandleOnCursorReleased;
    }

    private void OnDisable()
    {
        CursorPosition.OnCursorMoved -= ChangeDevice;
        CursorPosition.OnCursorReleased -= HandleOnCursorReleased;
        if(instancesCounter > 0)
            instancesCounter--;

        if (_deviceButton != null)
            _deviceButton.onClick.RemoveAllListeners();
    }

    public void SetPlayerName(string playerName)
    {
        _playerName.text = playerName;
    }

    public void ChangeDevice(CursorPosition cursor, Vector2 directions)
    {
        if (!cursor.transform.parent.gameObject.IsDeviceSelectionButton() ||
            !IsButtonWithCursor(cursor))
            return;

        if(!_changeDeviceBtnPressed && directions.x != 0 && directions.y == 0)
        {
            CurrentIndex = (directions.x < 0) ?
                CurrentIndex - 1 : CurrentIndex + 1;
            _changeDeviceBtnPressed = true;
            SelectDevice(CurrentIndex);            
        }
    }

    public void HandleOnCursorReleased(CursorPosition cursor)
    {
        _changeDeviceBtnPressed = false;
    }

    private bool IsButtonWithCursor(CursorPosition cursor)
    {
        return _playerName.transform.parent.gameObject.
            Equals(cursor.transform.parent.gameObject);
    }

    private void SelectDevice(int index)
    {
        _deviceTMPRO.text = _devicesNames[index];
        _leftArrow.SetActive(index > 0);
        _rightArrow.SetActive(index < _devicesNames.Count - 1);
    }
}
