using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class serves to store the GameObjects references a Controller will needs on its scenes.
/// The main purpose is to load the references whenever a Controller reenters a scene where it is necessary
/// and their references are lost due the scene change.
/// </summary>
public abstract class ControllerBackup : MonoBehaviour
{
    [SerializeField] protected ControllerUnit _controller;

    public virtual void OnEnable()
    {
        var controllerType = GetControllerType();
        StartCoroutine(FindControllerForBackupCoroutine(controllerType));
        Invoke(nameof(UpdateLanguageTexts), 0.2f);
    }

    /// <summary>
    /// Gets the Controller Type the backup will have to search in order to do its backup.
    /// </summary>
    /// <returns></returns>
    protected abstract Type GetControllerType();

    /// <summary>
    /// Set all listeners from the buttons stored on this controller.
    /// </summary>
    protected abstract void ListenersSetup();

    private IEnumerator FindControllerForBackupCoroutine(Type type)
    {
        yield return new WaitForEndOfFrame();
        FindControllerForBackup(type);

        yield return new WaitForEndOfFrame();
        ListenersSetup();
    }

    public void FindControllerForBackup(Type type)
    {
        var controller = FindAnyObjectByType(type);

        if(controller != null)
        {
            try
            {
                _controller = controller as ControllerUnit;
                _controller.LoadGameObjectsReferencesFromControllerBackup(this);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    public abstract void UpdateLanguageTexts();
}
