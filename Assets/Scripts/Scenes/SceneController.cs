using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : ControllerUnit
{
    public Action<Scene> OnSceneLoaded;

    [SerializeField] private GameObject _loadingScreen;
    private LoadingProgress _loadingScreenProgress;

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
        StartCoroutine(LoadAsync(sceneId));
    }

    private void HandleOnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        //print($"Scene {scene.name} loaded with success.");
        OnSceneLoaded?.Invoke(scene);
    }

    IEnumerator LoadAsync(int sceneId)
    {
        Time.timeScale = 0.1f;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);

        var loadingScreenInstance = Instantiate(_loadingScreen);
        _loadingScreenProgress = loadingScreenInstance.GetComponent<LoadingProgress>();

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            _loadingScreenProgress.SetProgress(progress);

            yield return null;
        }

        Time.timeScale = 1.0f;
    }
}
