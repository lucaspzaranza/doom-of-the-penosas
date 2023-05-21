using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : ControllerUnit
{
    public Action<Scene> OnSceneLoaded;

    public override void Setup()
    {
        base.Setup();
        EventHandlerSetup();
    }

    public override void Dispose()
    {
        SceneManager.sceneLoaded -= HandleOnSceneLoaded;
    }

    private void EventHandlerSetup()
    {
        SceneManager.sceneLoaded += HandleOnSceneLoaded;
    }

    public void LoadScene(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }

    private void HandleOnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        //print($"Scene {scene.name} loaded with success.");
        OnSceneLoaded?.Invoke(scene);
    }
}
