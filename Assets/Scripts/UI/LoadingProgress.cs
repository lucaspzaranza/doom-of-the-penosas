using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingProgress : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _progressBar;
    [SerializeField] private TextMeshProUGUI _progressTxt;
    [SerializeField] private float _fadeRate;

    private bool _fadeIn;

    private bool _isInFade;
    public bool IsInFade => _isInFade;

    public void SetProgress(float progress)
    {
        _progressTxt.text = $"{progress * 100f}%";
        _progressBar.fillAmount = progress;
    }

    public void Fade(bool val)
    {
        _isInFade = true;
        _fadeIn = val;
    }

    private void FixedUpdate()
    {
        if(_isInFade)
        {
            if(_fadeIn)
            {
                _canvasGroup.alpha = Mathf.Clamp01(_canvasGroup.alpha + _fadeRate);
                if (_canvasGroup.alpha >= 1f)
                    _isInFade = false;
            }
            else
            {
                _canvasGroup.alpha = Mathf.Clamp01(_canvasGroup.alpha - _fadeRate);
                if (_canvasGroup.alpha <= 0f)
                {
                    _isInFade = false;
                    Destroy(gameObject);
                }
            }
        }
    }
}
