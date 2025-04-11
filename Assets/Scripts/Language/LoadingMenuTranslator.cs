using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingMenuTranslator : MenuTranslator
{
    [SerializeField] private TextMeshProUGUI _loadingTMPRO;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override void Translate()
    {
        _loadingTMPRO.text = lang.LoadingText;
    }
}
