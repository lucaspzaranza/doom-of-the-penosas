using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapaMundiMenuTranslator : MenuTranslator
{
    [SerializeField] private List<Button> _stageButtons;
    public List<Button> StageButtons => _stageButtons;

    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _subtitle;
    [SerializeField] private Button _backBtn;

    public override void Translate()
    {
        base.Translate();

        _title.text = lang.MapaMundiTitle;
        _subtitle.text = lang.SelectAStageToGo;
        _backBtn.GetComponentInChildren<TextMeshProUGUI>().text = lang.BackFromLangMenu;

        int counter = 1;
        StageButtons.ForEach(stageBtn =>
        {
            stageBtn.GetComponentInChildren<TextMeshProUGUI>().text = $"{lang.Stage} {counter}";
            counter++;
        });
    }
}
