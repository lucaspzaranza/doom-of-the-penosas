using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistenceController : ControllerUnit
{
    public override void Setup()
    {
        base.Setup();
    }

    public override void Dispose()
    {

    }
    
    public void SaveCompletedStages(int completedStages)
    {
        //print("completedStages: " + completedStages);
        PlayerPrefs.SetInt(ConstantStrings.CompletedStagesKey, completedStages);
    }

    public int LoadCompletedStages()
    {
        return PlayerPrefs.GetInt(ConstantStrings.CompletedStagesKey);
    }
}
