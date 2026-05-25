using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringAnimation : MonoBehaviour
{
    [SerializeField] private bool _activated;
    public bool Activated => _activated;

    [SerializeField] private float _compressionLimit;
    [SerializeField] private float _speed;

    private bool _compressed = false;
    private float _initYScale;

    private void OnEnable()
    {
        _activated = true;
    }

    void Start()
    {
        _initYScale = transform.localScale.y;
    }

    void Update()
    {
        if(_activated)
        {
            Vector2 scale = transform.localScale;

            if(!_compressed)
            {
                transform.localScale = new Vector2(scale.x, scale.y - (_speed * Time.deltaTime));

                if(transform.localScale.y <= _compressionLimit)
                    _compressed = true;
            }
            else
            {
                transform.localScale = new Vector2(scale.x, scale.y + (_speed * Time.deltaTime));

                if(transform.localScale.y >= _initYScale)
                {
                    transform.localScale = new Vector2(scale.x, _initYScale);
                    _compressed = false;
                    _activated = false;
                }
            }
        }
    }
}
