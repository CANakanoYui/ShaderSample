using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlanarReflectionFeature : ScriptableRendererFeature
{
    private PlanarReflectionPass _planarReflectionPass;

    [SerializeField] private Shader _shader;
    
    [SerializeField] private int _width = 512;
    
    [SerializeField] private int _height = 512;

    public override void Create()
    {
        _planarReflectionPass = new PlanarReflectionPass(_shader,_width,_height);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_planarReflectionPass);
    }
}
