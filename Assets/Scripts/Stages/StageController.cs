using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

public class StageController : ControllerUnit
{
    public Action<StageSO> OnStageClear;

    [SerializeField] private Canvas _stageCanvas;

    [SerializeField] private StageSO _currentStage;
    public StageSO CurrentStage => _currentStage;

    [SerializeField] private List<StageSO> _stages;
    public IReadOnlyList<StageSO> Stages => _stages;

    [SerializeField] private GameObject _stageClearText;
    public GameObject StageClearText => _stageClearText;

    [SerializeField] private RideArmorType _rideArmorRequired;
    public RideArmorType RideArmorRequired => _rideArmorRequired;

    private bool _fKeyPressed = false;

    public override void Setup()
    {
        base.Setup();

        _currentStage = _stages[0];
        RideArmor.OnRideArmorEquipped += HandleOnRideArmorEquipped;
        RideArmor.OnRideArmorChangedRequired += HandleOnRideArmorChangedRequired;

        Boss.OnBossDefeated += HandleOnBossDefeated;
    }

    public override void Dispose()
    {
        RideArmor.OnRideArmorEquipped -= HandleOnRideArmorEquipped;
        RideArmor.OnRideArmorChangedRequired -= HandleOnRideArmorChangedRequired;

        Boss.OnBossDefeated -= HandleOnBossDefeated;
    }

    // FOR TEST PURPOSES ONLY! REMOVE THIS!
    private void Update()
    {
        if (_fKeyPressed)
            return;

        if(Input.GetKeyDown(KeyCode.F) && 
            TryToGetGameControllerFromParent().GameStatus == GameStatus.InGame)
        {
            _fKeyPressed = true;
            HandleOnBossDefeated();
        }
    }
    
    public void ResetAllStagesClear()
    {
        for (int i = 0; i < _stages.Count; i++)
        {
            _stages[i].SetStageclear(false);
        }
    }

    public void SetStagesClearFromTo(int index)
    {
        for (int i = 0; i < index; i++)
        {
            if (!_stages[i].StageClear)
                _stages[i].SetStageclear(true);
        }
    }

    public void SetCurrentStageSO(StageSO stage)
    {
        _currentStage = stage;
    }

    public IEnumerator StageClearEvent()
    {
        _stageCanvas = FindObjectsByType<Canvas>(FindObjectsSortMode.None)
            .FirstOrDefault(canvas => canvas.transform.parent == null);

        var stageClearTxt = Instantiate(StageClearText, _stageCanvas.transform);

        yield return new WaitForSeconds(ConstantNumbers.TimeToShowStageClearTxt);

        Destroy(stageClearTxt);
    }

    public void HandleOnRideArmorEquipped(RideArmor rideArmor)
    {
        if (rideArmor.Required)
            _rideArmorRequired = rideArmor.Type;
    }

    public void HandleOnRideArmorChangedRequired(RideArmorType type, bool value)
    {
        if(value)
            _rideArmorRequired = type;
        else
            _rideArmorRequired = RideArmorType.None;
    }

    /// <summary>
    /// Event handler to fire the StageClear Event when the stage is clear, i.e.:
    /// when the boss is defeated.
    /// </summary>
    public void HandleOnBossDefeated()
    {
        print("You defeated a boss! Congratulations.");
        StartCoroutine(nameof(StageClearEvent));
        OnStageClear?.Invoke(_currentStage);

        if(!_currentStage.StageClear)
            _currentStage.SetStageclear(true);

        _fKeyPressed = false;
    }
}
