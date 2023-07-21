using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInGameUIController : ControllerUnit, IUIController
{
    [Space]
    [Header("HUD")]
    [SerializeField] private PlayerHUD[] _huds = new PlayerHUD[ConstantNumbers.NumberOfPlayers];
    public PlayerHUD[] HUDs => _huds;

    [SerializeField] private GameObject _HUDPrefab;

    [Space]
    [Header("Game Over")]
    [SerializeField] private GameObject _gameOverContainerObject;
    [SerializeField] private GameObject _title;
    [SerializeField] private GameObject _countdown;

    private Canvas _gameSceneCanvas;
    private Transform _hudTransform;

    public void Setup(Canvas gameSceneCanvas, IReadOnlyList<PlayerData> playersData)
    {
        if(_parentController == null)
            _parentController = GetComponentInParent<UIController>();

        if(_gameSceneCanvas == null)
        {
            _gameSceneCanvas = gameSceneCanvas;
            _hudTransform = _gameSceneCanvas.transform.Find(ConstantStrings.HUD).transform;
        }

        CreatePlayersHUDs(playersData);

        Penosa.OnPlayerRespawn += HandleOnPlayerRespawn;
    }

    public override void Dispose()
    {
        _gameSceneCanvas = null;
        for (int i = 0; i < _huds.Length; i++)
        {
            if (_huds[i] != null)
            {
                _huds[i].EventDispose();
                _huds[i] = null;
            }
        }

        Penosa.OnPlayerRespawn -= HandleOnPlayerRespawn;
        gameObject.SetActive(false);
    }

    private void CreatePlayersHUDs(IReadOnlyList<PlayerData> playersData)
    {
        foreach (var playerData in playersData)
        {
            GameObject newHUD = Instantiate(_HUDPrefab, _hudTransform);
            PlayerHUD playerHUD = newHUD.GetComponent<PlayerHUD>();

            if(playerHUD != null)
            {
                playerHUD.Player = playerData.Player;
                playerHUD.UpdateHUDValues();
                playerHUD.EventSetup();
            }

            _huds[playerData.LocalID] = playerHUD;
        }
    }
    
    public Text GetCountdownText()
    {
        return _countdown.GetComponent<Text>();
    }

    public void GameOverActivation(byte playerID, bool val)
    {
        _huds[playerID].HUDContainer.SetActive(!val);
        _huds[playerID].GameOverContainer.SetActive(val);
        _huds[playerID].CountdownActivated = val;
    }

    public void HandleOnPlayerRespawn(byte playerID)
    {
        GameOverActivation(playerID, false);
    }
}
