using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHUDTranslator : MenuTranslator
{
    [SerializeField] private TextMeshProUGUI _pressPauseToContinueTMPRO;
    [SerializeField] private TextMeshProUGUI _waitUntilStageOverTMPRO;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override void Translate()
    {
      
    }
}
