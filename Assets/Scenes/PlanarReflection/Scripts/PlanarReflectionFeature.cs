using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlanarReflectionFeature : ScriptableRendererFeature
{
    private PlanarReflectionPass _planarReflectionPass;
    
    public override void Create()
    {
        _planarReflectionPass = new();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_planarReflectionPass);
    }
}
