using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapaMundiController : ControllerUnit, IUIController
{
    public Action<int> OnGameSceneIndexSelected;
    public Action OnBackToMainMenu;    

    [SerializeField] private List<Button> _stageButtons;
    public List<Button> StageButtons => _stageButtons;

    public override void Setup()
    {
        SetControllerFromParent<UIController>();
    }

    public override void Dispose()
    {
        gameObject.SetActive(false);
    }

    public override void LoadGameObjectsReferencesFromControllerBackup(ControllerBackup backup)
    {
        MapaMundiControllerBackup mapaMundiBackup = backup as MapaMundiControllerBackup;
        _stageButtons = mapaMundiBackup.StageButtons;

        OnReferencesLoaded?.Invoke();
    }

    public void SelectSceneIndex(int buildIndex)
    {        
        OnGameSceneIndexSelected?.Invoke(buildIndex);
    }

    public void FireBackToMainMenuEvent()
    {
        OnBackToMainMenu?.Invoke();
    }

    public void ActivateStageLoaders(int completedStages)
    {
        print($"Completed Stages: {completedStages}");

        for (int i = 0; i < completedStages + 1; i++)
        {
            if (i >= _stageButtons.Count)
                break;

            _stageButtons[i].interactable = true;
        }
    }
}
