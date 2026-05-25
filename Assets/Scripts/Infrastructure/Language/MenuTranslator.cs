using System;
using SharedData.Enumerations;
using UnityEngine;

public class MenuTranslator : MonoBehaviour, IMenuTranslator
{
    public Action OnMenuTranslated;
    protected LanguageSO lang;

    protected virtual void OnEnable()
    {
        LanguageEvents.OnLanguageChanged += HandleLanguageChanged;
        LanguageEvents.OnLanguageRequested?.Invoke();
    }

    protected virtual void OnDisable()
    {
        LanguageEvents.OnLanguageChanged -= HandleLanguageChanged;
    }

    protected void HandleLanguageChanged(LanguageSO language)
    {
        if (lang == null || (lang != null && lang.Language != language.Language))
        {
            lang = language;
            Translate();
        }
    }

    public virtual void Translate() { }
}