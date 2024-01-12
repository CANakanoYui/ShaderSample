using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlanarReflectionPass : ScriptableRenderPass
{
    private List<PlanarReflectionCaster> _casters = new();

    private List<PlanarReflectionReceiver> _receivers = new();

    private CommandBuffer _commandBuffer;

    public Material Material { get; set; }

    private static PlanarReflectionPass _instance;

    public static PlanarReflectionPass Instance => _instance;
    
    public PlanarReflectionPass()
    {
        _instance = this;
        renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
        _commandBuffer = CommandBufferPool.Get(nameof(PlanarReflectionPass));
    }

    public void AddPlanarReflectionCaster(PlanarReflectionCaster caster)
    {
        _casters.Add(caster);
    }

    public void AddPlanarReflectionReceiver(PlanarReflectionReceiver receiver)
    {
        _receivers.Add(receiver);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.camera.name != "ReflectionCamera") return;
        
        foreach (var receiver in _receivers)
        {
            _commandBuffer.SetRenderTarget(receiver.ReflectionTexture);
            _commandBuffer.ClearRenderTarget(true, true, new Color(0, 0, 0, 0), 1.0f);
            
            foreach (var caster in _casters)
            {
                caster.Render(_commandBuffer);
            }
        }

        context.ExecuteCommandBuffer(_commandBuffer);
        _casters.Clear();
        _receivers.Clear();
    }

    public override void FrameCleanup(CommandBuffer cmd)
    {
        _commandBuffer.Clear();
    }
}