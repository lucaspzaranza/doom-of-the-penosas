using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputDevicesSelector : MonoBehaviour
{
    private static int instancesCounter;

    [SerializeField] private TextMeshProUGUI _playerName;
    [SerializeField] private TextMeshProUGUI _deviceTMPRO;
    [SerializeField] private List<InputDevice> _devices;

    public InputDevice SelectedDevice => _devices[_currentIndex];

    private int _currentIndex;
    private int CurrentIndex
    {
        get => _currentIndex;
        set
        {
            _currentIndex = Mathf.Clamp(value, 0, _devices.Count - 1);
        }
    }

    private void OnEnable()
    {
        _devices = InputSystem.devices.Where(device => device.displayName != ConstantStrings.Mouse).ToList();
        SelectDevice(instancesCounter);
        CurrentIndex = instancesCounter;

        if(instancesCounter < _devices.Count - 1)
            instancesCounter++;

        CursorPosition.OnCursorMoved += ChangeDevice;
    }

    private void OnDisable()
    {
        CursorPosition.OnCursorMoved -= ChangeDevice;
        if(instancesCounter > 0)
            instancesCounter--;
    }

    public void SetPlayerName(string playerName)
    {
        _playerName.text = playerName;
    }

    private bool IsDeviceSelectionButton(GameObject parentGameObj)
    {
        if (!parentGameObj.TryGetComponent(out Button button))
            return false;

        return parentGameObj.tag == ConstantStrings.DeviceSelectionButtonTag;
    }

    public void ChangeDevice(CursorPosition cursor, Vector2 directions)
    {
        if (!IsDeviceSelectionButton(cursor.transform.parent.gameObject) ||
            !IsButtonWithCursor(cursor))
            return;

        if(directions.x != 0 && directions.y == 0)
        {
            CurrentIndex = (directions.x < 0) ?
                CurrentIndex - 1 : CurrentIndex + 1;
            SelectDevice(CurrentIndex);
        }
    }

    private bool IsButtonWithCursor(CursorPosition cursor)
    {
        return _playerName.transform.parent.gameObject.
            Equals(cursor.transform.parent.gameObject);
    }

    private void SelectDevice(int index)
    {
        _deviceTMPRO.text = _devices[index].displayName;
    }
}
