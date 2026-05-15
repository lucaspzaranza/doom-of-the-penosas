using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class CinemachineCameraSelector : MonoBehaviour
{
    private GameController _gameCtrl;
    private CinemachineVirtualCamera _selectedCam;
    public CinemachineVirtualCamera SelectedCam => _selectedCam;

    private float _deadZoneWidth;
    public float DeadZoneWidth=> _deadZoneWidth;

    private float _softZoneWidth;
    public float SoftZoneWidth=> _softZoneWidth;

    private void OnEnable()
    {
        if(_gameCtrl == null)
            _gameCtrl = FindAnyObjectByType<GameController>();

        Penosa.OnPlayerInCameraEdge += HandleOnPlayerInCameraEdge;
        Penosa.OnPlayerOutCameraEdge += HandleOnPlayerOutCameraEdge;
        Penosa.OnResetCameraBounds += HandleOnResetCameraBounds;

        string gameMode = _gameCtrl ? _gameCtrl.GameMode.ToString() : ConstantStrings.Singleplayer;

        var cineBrain = Camera.main.GetComponent<CinemachineBrain>();
        if (!cineBrain) return;

        var virtualCams = FindObjectsByType<CinemachineVirtualCamera>(FindObjectsSortMode.None);    

        foreach (var cam in virtualCams)
        {
            if (cam.name.Contains(gameMode))
            {
                _selectedCam = cam;
                _selectedCam.Priority = 20;
                StartCoroutine(UpdateCameraPlayerToFollow());
                //print($"Camera {_selectedCam.name} is active for {gameMode} mode.");
                _selectedCam.gameObject.SetActive(true);
                var transposer = _selectedCam.GetCinemachineComponent<CinemachineFramingTransposer>();
                if (transposer != null)
                {
                    _softZoneWidth = transposer.m_SoftZoneWidth;
                    _deadZoneWidth = transposer.m_DeadZoneWidth;
                }
            }
            else
            {
                cam.Priority = 10;
                cam.gameObject.SetActive(false);
            }
        }
    }

    private void OnDisable()
    {
        Penosa.OnPlayerInCameraEdge -= HandleOnPlayerInCameraEdge;
        Penosa.OnPlayerOutCameraEdge -= HandleOnPlayerOutCameraEdge;
        Penosa.OnResetCameraBounds -= HandleOnResetCameraBounds;
    }

    public CinemachineFramingTransposer GetTransposer() => 
        SelectedCam.GetCinemachineComponent<CinemachineFramingTransposer>();

    private IEnumerator UpdateCameraPlayerToFollow()
    {
        if (_gameCtrl == null)
            yield break;

        while (_gameCtrl.PlayerController.PlayersData[0].Player == null)
        {
            //print("Waiting for player data to be ready...");
            yield return new WaitForEndOfFrame();
        }

        _selectedCam.Follow = _gameCtrl.PlayerController.PlayersData[0].Player.transform;
    }

    private void HandleOnPlayerInCameraEdge()
    {
        print("HandleOnPlayerInCameraEdge");
        var transposer = GetTransposer();

        transposer.m_DeadZoneWidth = ConstantNumbers.CameraScreenLimitWidth;
        transposer.m_SoftZoneWidth = ConstantNumbers.CameraScreenLimitWidth;
    }

    private void HandleOnPlayerOutCameraEdge()
    {
        print("HandleOnPlayerOutCameraEdge");

        var transposer = GetTransposer();

        if (transposer.m_DeadZoneWidth >= _deadZoneWidth)
        {
            float newValue = Time.deltaTime * ConstantNumbers.CameraScreenLimitWidthSpeed;
            transposer.m_DeadZoneWidth = Mathf.Clamp(transposer.m_DeadZoneWidth - newValue, _deadZoneWidth, ConstantNumbers.CameraScreenLimitWidth);
        }

        if (transposer.m_SoftZoneWidth >= _softZoneWidth)
        {
            float newValue = Time.deltaTime * ConstantNumbers.CameraScreenLimitWidthSpeed;
            transposer.m_SoftZoneWidth = Mathf.Clamp(transposer.m_DeadZoneWidth - newValue, _softZoneWidth, ConstantNumbers.CameraScreenLimitWidth);
        }
    }

    private void HandleOnResetCameraBounds()
    {
        print("HandleOnResetCameraBounds");

        var transposer = GetTransposer();

        transposer.m_SoftZoneWidth = _softZoneWidth;
        transposer.m_DeadZoneWidth = _deadZoneWidth;
    }
}
