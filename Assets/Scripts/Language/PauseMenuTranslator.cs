using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuTranslator : MenuTranslator
{
    [SerializeField] private TextMeshProUGUI _pauseTitle;
    [SerializeField] private Button _resumeBtn;
    [SerializeField] private Button _backToMapaMundiBtn;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override void Translate()
    {
        _pauseTitle.text = lang.PauseTitle;
        _resumeBtn.GetComponentInChildren<TextMeshProUGUI>().text = lang.Resume;
        _backToMapaMundiBtn.GetComponentInChildren<TextMeshProUGUI>().text = lang.BackToMapaMundi;
    }
}
