using NUnit.Framework;
using SharedData.Enumerations;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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

    private string CurrentStepText => _currentStep.GetText(_currentLanguage.Language);

    private bool _showTextInProgress;
    public bool ShowTextInProgress => _showTextInProgress;

    [SerializeField] private Button _nextStepBtn;
    public Button NextStepBtn => _nextStepBtn;

    [SerializeField] private Button _skipBtn;
    public Button SkipBtn => _skipBtn;

    [SerializeField] private TextSOBase _textSO;
    public TextSOBase TextSO => _textSO;

    [SerializeField] private Step _currentStep;
    public Step CurrentStep => _currentStep;

    private bool _canNextStep;
    public bool CanNextStep => _canNextStep;

    private int _stepCounter;
    public int StepCounter => _stepCounter;

    private float _timeCounter;
    private int _charIndex;

    private LanguageSO _currentLanguage;
    private void OnEnable()
    {
        Step.OnStepInitialized += HandleOnStepInitialized;
        LanguageEvents.OnLanguageChanged += HandleLanguageChanged;
        LanguageEvents.OnLanguageRequested?.Invoke();

        if (TextSO != null)
            NewStepSetup(TextSO.Steps[0]);

        _stepCounter = 0;

        if (!CanNextStep)
            SetCanNextStep(true);
    }

    void Update()
    {
        //if (_gameCtrlInstance == null) return;

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

    public void SetCanNextStep(bool val) => _canNextStep = val;

    public void ListernersSetup()
    {
        NextStepBtn.onClick.RemoveAllListeners();
        SkipBtn.onClick.RemoveAllListeners();

        NextStepBtn.onClick.AddListener(() =>
        {
            if (CanNextStep)
                NextButtonPressed();
        });

        _skipBtn.interactable = TextSO.Skippable;

        SkipBtn.onClick.AddListener(() =>
        {
            Skip();
        });
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
        _canNextStep = true;
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
        //Debug.Log($"_stepCounter: {_stepCounter}, TextSO.Steps.Count: {TextSO.Steps.Count}");
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
        TextSO.Steps[index].InitializeStep();
    }

    private void HandleOnStepInitialized(Step step)
    {
        if (!string.IsNullOrEmpty(CurrentStepText))
            _showTextInProgress = true;
    }

    public void Skip()
    {
        //print("Skip");
        //Debug.Log($"_stepCounter: {_stepCounter}, TextSO.Steps.Count: {TextSO.Steps.Count}");
        if (_stepCounter >= TextSO.Steps.Count || TextSO.Skippable)
        {
            ResetTextDisplayCounterData();
            _stepCounter = 0;
            OnSkipButtonPressed.Invoke();
            gameObject.SetActive(false);
        }
    }

    private void HandleLanguageChanged(LanguageSO language)
    {
        _currentLanguage = language;
    }

    private void OnDisable()
    {
        if (DiagBoxTxt != null)
            DiagBoxTxt.text = string.Empty;

        _showTextInProgress = false;

        Step.OnStepInitialized -= HandleOnStepInitialized;
        LanguageEvents.OnLanguageChanged -= HandleLanguageChanged;
    }
}
