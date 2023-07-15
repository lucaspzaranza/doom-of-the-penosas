using SharedData.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class StageController : ControllerUnit
{
    public Action<StageSO> OnStageClear;

    [SerializeField] private StageSO _currentStage;
    public StageSO CurrentStage => _currentStage;

    [SerializeField] private List<StageSO> _stages;
    public IReadOnlyList<StageSO> Stages => _stages;

    public override void Setup()
    {
        base.Setup();

        _currentStage = _stages[0];

        // When I Add boss logic, this line will be necessary.
        // Boss.OnBossDefeated += HandleOnBossDefeated;
    }

    public override void Dispose()
    {
        // When I Add boss logic, this line will be necessary.
        // Boss.OnBossDefeated -= HandleOnBossDefeated;
    }

    // FOR TEST PURPOSES ONLY! REMOVE THIS!
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) && 
            TryToGetGameControllerFromParent().GameStatus == GameStatus.InGame)
            HandleOnBossDefeated();
    }

    public void SetCurrentStageSO(StageSO stage)
    {
        print($"Setting {stage.name} as the current stage.");
        _currentStage = stage;
    }

    /// <summary>
    /// Event handler to fire the StageClear Event when the stage is clear, i.e.:
    /// when the boss is defeated.
    /// </summary>
    public void HandleOnBossDefeated()
    {
        print("HandleOnBossDefeated");
        OnStageClear?.Invoke(_currentStage);
    }
}
