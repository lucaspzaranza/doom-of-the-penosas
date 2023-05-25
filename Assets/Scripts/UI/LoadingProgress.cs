using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingProgress : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Image _progressBar;
    [SerializeField] private TextMeshProUGUI _progressTxt;

    private void OnEnable()
    {
        _canvas.worldCamera = Camera.main;
    }

    public void SetProgress(float progress)
    {
        _progressTxt.text = $"{progress * 100f}%";
        _progressBar.fillAmount = progress;
    }
}
