using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "StageSO", menuName = "ScriptableObjects/StageSO")]
public class StageSO : ScriptableObject
{
    [SerializeField] private string _name;
    public string Name => _name;

    [SerializeField] private int _sceneIndex;
    public int SceneIndex => _sceneIndex;

    [SerializeField] private bool _stageClear;
    public bool StageClear => _stageClear;

    [SerializeField] private bool _isFinalStage;
    public bool IsFinalStage => _isFinalStage;

    public void SetStageclear(bool val)
    {
        _stageClear = val;
    }

    // Include Boss data which will fire the boss defeated event
    // which will be handled by the StageController to fire the
    // OnStageClear event. The OnBossDefeated event must be static
}
