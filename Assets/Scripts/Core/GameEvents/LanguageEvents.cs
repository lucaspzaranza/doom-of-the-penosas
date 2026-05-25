
using SharedData.Enumerations;
using System;

public static class LanguageEvents
{
    public static Action<LanguageSO> OnLanguageChanged;
    public static Action OnLanguageRequested;
}