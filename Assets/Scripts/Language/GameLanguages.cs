using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameLanguages
{
    [SerializeField] private LanguageSO _english;
    public LanguageSO English => _english;

    [SerializeField] private LanguageSO _portuguese;
    public LanguageSO Portuguese => _portuguese;
}
