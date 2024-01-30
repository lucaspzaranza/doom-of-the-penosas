using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PlayerIcon
{
    public Penosas Character;
    public Sprite PlayerIconSprite;
    public byte PlayerID;
}

public class PlayerInGameUIController : ControllerUnit, IUIController
{
    public Action<byte> OnCountdownIsOver;
    
    [Space]
    [Header("HUD")]
    [SerializeField] private PlayerHUD[] _huds = new PlayerHUD[ConstantNumbers.NumberOfPlayers];
    public PlayerHUD[] HUDs => _huds;

    [SerializeField] private PlayerIcon[] _playerIcons = new PlayerIcon[ConstantNumbers.NumberOfPlayers];

    [SerializeField] private GameObject _HUDPrefab;

    [SerializeField] private List<Sprite> _rideArmorsSprites;

    [Space]
    [Header("Game Over")]
    [SerializeField] private GameObject _gameOverContainerObject;
    [SerializeField] private GameObject _title;
    [SerializeField] private GameObject _countdown;

    private Canvas _gameSceneCanvas;
    private Transform _hudTransform;

    private UIController UICtrl => (UIController)_parentController;

    private GameController GameController => UICtrl.TryToGetGameControllerFromParent();

    public void Setup(Canvas gameSceneCanvas, IReadOnlyList<PlayerData> playersData)
    {
        if(_parentController == null)
            _parentController = GetComponentInParent<UIController>();

        if(_gameSceneCanvas == null)
        {
            _gameSceneCanvas = gameSceneCanvas;
            _hudTransform = _gameSceneCanvas.transform.Find(ConstantStrings.HUD).transform;
        }

        OnCountdownIsOver += UICtrl.OnCountdownIsOver;
        CreatePlayersHUDs(playersData);
    }

    private void HUDEventDispose(int index)
    {
        if (_huds[index].Player != null)
            _huds[index].Player.OnPlayerRespawn -= HandleOnPlayerRespawn;
        _huds[index].OnCountdownIsOver -= OnCountdownIsOver;
        _huds[index].EventDispose();
        _huds[index] = null;
    }

    public override void Dispose()
    {
        _gameSceneCanvas = null;
        for (int i = 0; i < _huds.Length; i++)
        {
            if (_huds[i] != null)
            {
                HUDEventDispose(i);
            }
        }

        OnCountdownIsOver -= UICtrl.OnCountdownIsOver;
        gameObject.SetActive(false);
    }

    public void CreatePlayersHUDs(IReadOnlyList<PlayerData> playersData)
    {
        foreach (var playerData in playersData)
        {
            GameObject newHUD = Instantiate(_HUDPrefab, _hudTransform);
            PlayerHUD playerHUD = newHUD.GetComponent<PlayerHUD>();
            _huds[playerData.LocalID] = playerHUD;

            playerHUD.PlayerIcon = _playerIcons
                        .SingleOrDefault(icon => icon.Character == GameController
                        .PlayerController.PlayersData[playerData.LocalID].Character)
                        .PlayerIconSprite;

            if (!playerData.GameOver && playerHUD != null)
            {
                playerHUD.Player = playerData.Player;
                playerHUD.UpdateHUDValues();
                playerHUD.EventSetup();
                playerHUD.OnCountdownIsOver += OnCountdownIsOver;
                playerData.Player.OnPlayerRespawn += HandleOnPlayerRespawn;
            }
            else if(playerData.GameOver)
                SetGameOverContainerOnPlayerActive(playerData.LocalID, true);
        }
    }

    public void ContinueActivation(byte playerID, bool val)
    {
        _huds[playerID].HUDContainer.SetActive(!val);
        _huds[playerID].ContinueContainer.SetActive(val);
        _huds[playerID].CountdownActivated = val;
    }

    public void HandleOnPlayerRespawn(byte playerID)
    {
        ContinueActivation(playerID, false);
    }

    public void HideHUDs()
    {
        foreach (var hud in _huds)
        {
            if(hud != null)
                hud.gameObject.SetActive(false);
        }
    }

    public void SetGameOverContainerOnPlayerActive(byte playerID, bool val)
    {
        if(_huds[playerID].HUDContainer.activeSelf)
            _huds[playerID].HUDContainer.SetActive(!val);

        _huds[playerID].ContinueContainer.SetActive(!val);
        _huds[playerID].GameOverContainer.SetActive(val);

        if (val)
        {
            if (GameController.PlayerController.PlayersData[playerID].Continues <= 0)
                _huds[playerID].GameOverText.text = ConstantStrings.HUDNoMoreContinues;
            else
                _huds[playerID].GameOverText.text = ConstantStrings.HUDGameOver;
        }
    }

    public void DestroyAllHUDs()
    {
        for (int i = 0; i < _huds.Length; i++)
        {
            if (_huds[i] != null)
            {
                Destroy(_huds[i].gameObject);
                HUDEventDispose(i);
            }
        }

        _huds = new PlayerHUD[ConstantNumbers.NumberOfPlayers];
    }

    public void UpdateHUDWithRideArmor(byte playerID, RideArmor rideArmor, bool isEquipping)
    {
        if (isEquipping)
        {
            _huds[playerID].PlayerIcon = _rideArmorsSprites[(int)rideArmor.RideArmorType];
            rideArmor.OnRideArmorLifeChanged += HUDs[playerID].UpdateRideArmorLife;
            _huds[playerID].UpdateRideArmorLife(rideArmor.Life);
        }
        else
        {
            _huds[playerID].PlayerIcon = _playerIcons
                .Single(playerIconData => playerIconData.Character == 
                _huds[playerID].Player.PlayerData.Character).PlayerIconSprite;

            _huds[playerID].UpdateRideArmorLife(0);
            rideArmor.OnRideArmorLifeChanged -= HUDs[playerID].UpdateRideArmorLife;
        }
    }
}
