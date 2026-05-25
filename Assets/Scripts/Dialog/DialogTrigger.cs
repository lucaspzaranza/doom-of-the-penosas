using System;
using UnityEngine;

public class DialogTrigger : EventTrigger
{
    public static Action OnDialogBoxCreated;
    public static Action OnDialogBoxClosed;

    [SerializeField] private TextSOBase _text;
    [SerializeField] private GameObject _dialogPrefab;
    [SerializeField] private Canvas _gameSceneCanvas;

    private DialogBox _dialogBox;
    public DialogBox DialogBox => _dialogBox;

    private GameObject _dialogInstance;

    private void OnEnable()
    {
        _dialogInstance = FindAnyObjectByType<DialogBox>()?.gameObject;
        _dialogBox = _dialogInstance?.GetComponent<DialogBox>();
    }

    public override void Interact()
    {
        CreateDialogBox();
    }

    public void SetText(TextSOBase text)
    {
        _text = text;
    }

    public void CreateDialogBox()
    {
        if(_dialogInstance == null &&  DialogBox == null)
        {
            _dialogInstance = Instantiate(_dialogPrefab, _gameSceneCanvas.transform);
            _dialogBox = _dialogInstance.GetComponent<DialogBox>();
        }
        else if(_dialogInstance != null && !_dialogInstance.activeSelf)
        {
            _dialogInstance.SetActive(true);
        }

        if(DialogBox.TextSO == null)
            DialogBox.SetTextSO(_text);

        DialogBox.ListernersSetup();

        DialogBox.PlayStep(0);
        DialogBox.OnSkipButtonPressed += HandleSkip;
        OnDialogBoxCreated?.Invoke();
    }

    public void HandleSkip()
    {
        DialogBox.OnSkipButtonPressed -= HandleSkip;
        OnDialogBoxClosed?.Invoke();
    }
}
