using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanarReflectionReceiver : MonoBehaviour
{
    [SerializeField] private float _factor = 1;

    [SerializeField] private RenderTexture _reflectionTexture;

    public RenderTexture ReflectionTexture => _reflectionTexture;

    private Material _material;

    private void Start()
    {
        _reflectionTexture.width = (int)(Screen.width * _factor);
        _reflectionTexture.height = (int)(Screen.height * _factor);
    }

    private void Update()
    {
        var pass = PlanarReflectionPass.Instance;
        pass?.AddPlanarReflectionReceiver(this);
    }
}
