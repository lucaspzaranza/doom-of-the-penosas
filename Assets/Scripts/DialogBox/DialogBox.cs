using NUnit.Framework;
using SharedData.Enumerations;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

[System.Serializable]
public class DialogBox : MonoBehaviour
{
    public static Action OnNextButtonPressed;
    public static Action OnSkipButtonPressed;

    [SerializeField] private DialogBoxType _dialogBoxType;
    public DialogBoxType DialogBoxType => _dialogBoxType;

    [SerializeField] private TextMeshProUGUI _diagBoxTxt;
    public TextMeshProUGUI DiagBoxTxt => _diagBoxTxt;

    private string CurrentStepText => _currentStep.GetText(_gameCtrlInstance.Language);

    private bool _showTextInProgress;
    public bool ShowTextInProgress => _showTextInProgress;

    [SerializeField] private TextSOBase _textSO;
    public TextSOBase TextSO => _textSO;

    [SerializeField] private Step _currentStep;
    public Step CurrentStep => _currentStep;

    private int _stepCounter;
    public int StepCounter => _stepCounter;

    private float _timeCounter;
    private int _charIndex;

    private GameController _gameCtrlInstance;

    private void OnEnable()
    {
        if(_gameCtrlInstance == null)
            _gameCtrlInstance = FindFirstObjectByType<GameController>();

        Step.OnStepInitialized += HandleOnStepInitialized;

        if(TextSO != null)
            NewStepSetup(TextSO.Steps[0]);

        _stepCounter = 0;
    }

    void Update()
    {
        if (_gameCtrlInstance == null) return;

        if (!ShowTextInProgress || CurrentStep == null || DiagBoxTxt == null ||
            string.IsNullOrEmpty(CurrentStepText)) return;

        if (DiagBoxTxt.text.Length == CurrentStepText.Length)
        {
            ResetTextDisplayCounterData();
            _showTextInProgress = false;
        }

        _timeCounter += Time.deltaTime;
        if (_timeCounter >= CurrentStep.TextSpeed)
        {
            DiagBoxTxt.text += CurrentStepText[_charIndex];
            _timeCounter = 0;
            _charIndex++;
        }
    }

    private void ResetTextDisplayCounterData()
    {
        _charIndex = 0;
        _timeCounter = 0;
    }

    private void NewStepSetup(Step step)
    {
        _currentStep = step;
        DiagBoxTxt.text = string.Empty;
    }

    public void SetTextSO(TextSOBase textSO)
    {
        _textSO = textSO;
        NewStepSetup(TextSO.Steps[0]);
    }

    public void NextButtonPressed()
    {
        if (ShowTextInProgress)
            _diagBoxTxt.text = CurrentStepText;
        else
            NextStep();

        OnNextButtonPressed?.Invoke();
    }
   
    public void NextStep()
    {
        _stepCounter++;
        if (_stepCounter >= TextSO.Steps.Count)
            Skip();
        else
        {
            ResetTextDisplayCounterData();
            NewStepSetup(TextSO.Steps[_stepCounter]);
            PlayStep(_stepCounter);
        }
    }

    public void PlayStep(int index)
    {
        //print("Step is: " + index);
        //print(_currentStep.GetText(_gameCtrlInstance.Language));
        TextSO.Steps[index].InitializeStep();
    }

    private void HandleOnStepInitialized(Step step)
    {
        if (!string.IsNullOrEmpty(CurrentStepText))
            _showTextInProgress = true;
    }

    public void Skip()
    {
        //Debug.Log($"_stepCounter: {_stepCounter}, Steps.Count: {Steps.Count}");
        if (_stepCounter >= TextSO.Steps.Count || TextSO.Skippable)
        {
            _stepCounter = 0;
            OnSkipButtonPressed.Invoke();
        }
    }

    private void OnDisable()
    {
        if (DiagBoxTxt != null)
            DiagBoxTxt.text = string.Empty;

        _showTextInProgress = false;

        Step.OnStepInitialized -= HandleOnStepInitialized;
    }
}
