using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlanarReflectionFeature : ScriptableRendererFeature
{
    private PlanarReflectionPass _planarReflectionPass;
    
    [SerializeField] private Shader _casterShader;
    
    [SerializeField] private Shader _receiverShader;
    
    public override void Create()
    {
        _planarReflectionPass = new PlanarReflectionPass(_casterShader,_receiverShader);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_planarReflectionPass);
    }
}
