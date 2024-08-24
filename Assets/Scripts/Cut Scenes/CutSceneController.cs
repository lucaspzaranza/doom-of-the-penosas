using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneController : Controller
{
    [SerializeField] private CutSceneSO _currentCutscene;
    public CutSceneSO CurrentCutscene => _currentCutscene;

    public override void Dispose()
    {
        throw new System.NotImplementedException();
    }

    public override void Setup()
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator ShowTextInSequence()
    {
        yield return null;
    }
}
