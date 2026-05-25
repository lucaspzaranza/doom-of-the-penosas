using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableObject : MonoBehaviour
{
    private const float _glowFrameInterval = 0.05f;
    private const float _blinkFrameInterval = 0.05f;

    [Tooltip("Add here the sprites which will be used for blinking and glowing when the object be hit and take damage.")]
    [SerializeField] protected List<SpriteRenderer> _sprites;
    [SerializeField] protected float _blinkDuration;
    protected float _blinkTimeCounter;
    protected float _blinkIntervalTimeCounter;
    protected bool _tookDamage;

    [SerializeField] private float _glowDuration; // 0.1f
    [SerializeField] private Color _1stColor;
    [SerializeField] private Color _2ndColor;
    [SerializeField] private Color _defaultColor;

    private bool _isGlowing;
    private float _glowIntervalTimeCounter;
    private float _glowTimeCounter;

    // Props

    protected bool _isBlinking = false;
    public bool IsBlinking => _isBlinking;

    [SerializeField] protected int _life;
    public int Life
    {
        get => _life;
        set
        {
            SetLife(value);            
        }
    }

    protected virtual void FixedUpdate()
    {
        if (_isGlowing)
        {
            _glowIntervalTimeCounter += Time.fixedDeltaTime;
            _glowTimeCounter += Time.fixedDeltaTime;
            if (_glowIntervalTimeCounter >= _glowFrameInterval)
            {
                Glow();
                _glowIntervalTimeCounter = PlayerConsts.BlinkInitialValue;
            }
        }

        if (IsBlinking)
        {
            _blinkIntervalTimeCounter += Time.fixedDeltaTime;
            _blinkTimeCounter += Time.fixedDeltaTime;
            if (_blinkIntervalTimeCounter >= _blinkFrameInterval)
            {
                Blink();
                _blinkIntervalTimeCounter = PlayerConsts.BlinkInitialValue;
            }
        }
    }

    public void Blink()
    {
        if (_blinkTimeCounter < _blinkDuration)
        {
            foreach (var sprite in _sprites)
            {
                float transparency = sprite.color.a == 1 ? 0f : 1f;
                Color blinkColor = new Color(255f, 255f, 255f, transparency);
                sprite.color = blinkColor;
            }
        }
        else
        {
            foreach (var sprite in _sprites)
            {
                _isBlinking = false;
                Color normalColor = new Color(255f, 255f, 255f, 1f);

                sprite.color = normalColor;
                _blinkTimeCounter = 0f;
            }
        }
    }

    public virtual void TakeDamage(int damage, bool force = false)
    {
        if (!IsBlinking || force)
        {
            Life -= damage;
            _tookDamage = true;
            StartGlow();
        }
        else
            _tookDamage = false;
    }

    public void InitiateBlink()
    {
        _isBlinking = true;
    }

    protected virtual void SetLife(int value)
    {
        _life = value;
    }

    public void StartGlow()
    {
        _isGlowing = true;
    }

    protected void Glow()
    {
        if (_glowTimeCounter < _glowDuration)
        {
            _sprites.ForEach(sprite =>
            {
                Color blinkColor = sprite.color == _1stColor ? _2ndColor : _1stColor;
                sprite.color = blinkColor;
            });
        }
        else
        {
            _isGlowing = false;
            _sprites.ForEach(sprite =>
            {
                sprite.color = _defaultColor;
            });

            _glowTimeCounter = 0f;
        }
    }
}
