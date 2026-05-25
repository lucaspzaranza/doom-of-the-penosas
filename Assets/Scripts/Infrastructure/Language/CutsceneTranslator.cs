using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneTranslator : MenuTranslator
{
    [SerializeField] private Button _nextStepBtn;
    public Button NextStepBtn => _nextStepBtn;

    [SerializeField] private Button _skipBtn;
    public Button SkipBtn => _skipBtn;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override void Translate()
    {
        _nextStepBtn.GetComponentInChildren<TextMeshProUGUI>().text = lang.NextBtn;
        _skipBtn.GetComponentInChildren<TextMeshProUGUI>().text = lang.SkipBtn;
    }
}
