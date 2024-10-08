using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameModeTitleSelector : MonoBehaviour
{
    public TextMeshProUGUI _subtitle;

    private void OnEnable()
    {
        var gameCtrl = FindAnyObjectByType<GameController>();
        LanguageSO lang = gameCtrl.CurrentLanguage;

        if(gameCtrl != null)
            _subtitle.text = gameCtrl.IsNewGame? lang.NewGameSubtitle : lang.ContinueSubtitle;
    }
}
