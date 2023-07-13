using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinueButtonSetInteractable : MonoBehaviour
{
    [SerializeField] private Button _continueButton;

    private void OnEnable()
    {
        var gameCtrl = FindAnyObjectByType<GameController>();

        if(gameCtrl != null)
        {
            int stages = gameCtrl.PersistenceController.LoadCompletedStages();
            _continueButton.interactable = stages > 0;
        }
    }
}
