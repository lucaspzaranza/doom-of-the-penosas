using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTranslator : MonoBehaviour
{
    public Action OnMenuTranslated;

    protected GameController gameCtrl;
    protected LanguageSO lang;

    protected virtual void OnEnable()
    {
        UpdateControllerAndLanguageReferences();
    }

    public void UpdateControllerAndLanguageReferences()
    {
        gameCtrl = FindAnyObjectByType<GameController>();
        lang = gameCtrl.CurrentLanguage;
    }

    public virtual void Translate() 
    {
        UpdateControllerAndLanguageReferences();
    }
}