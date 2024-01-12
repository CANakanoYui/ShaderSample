using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanarReflectionReceiver : MonoBehaviour
{
    [SerializeField] private int _factor = 1;
    
    public RenderTexture ReflectionTexture { get; set; }

    private Material _material;

    private void Start()
    {
        _factor = Math.Max(_factor, 1);
        ReflectionTexture = new(Screen.width / _factor,Screen.height / _factor,32);
    }

    private void Update()
    {
        var pass = PlanarReflectionPass.Instance;
        pass?.AddPlanarReflectionReceiver(this);
    }
}
