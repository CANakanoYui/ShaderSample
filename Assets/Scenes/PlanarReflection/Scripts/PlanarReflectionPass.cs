using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlanarReflectionPass : ScriptableRenderPass
{
    private List<PlanarReflectionCaster> _casters = new List<PlanarReflectionCaster>();

    private List<PlanarReflectionReceiver> _receivers = new List<PlanarReflectionReceiver>();

    private CommandBuffer _commandBuffer;

    private static PlanarReflectionPass _instance;

    public static PlanarReflectionPass Instance => _instance;
    
    public Material CasterMaterial { get; set; }
    public Material ReceiverMaterial { get; set; }
    public Material ReceiverMaterial1 { get; set; }
    
    public PlanarReflectionPass(Shader casterShader,Shader receiverShader)
    {
        _instance = this;
        CasterMaterial = CoreUtils.CreateEngineMaterial(casterShader);
        ReceiverMaterial = CoreUtils.CreateEngineMaterial(receiverShader);
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
        if (renderingData.cameraData.camera != Camera.main) return;
        
        foreach (var receiver in _receivers)
        {
            _commandBuffer.SetRenderTarget(receiver.ReflectionTexture);
            _commandBuffer.ClearRenderTarget(true, true, Color.black, 1.0f);
            
            foreach (var caster in _casters)
            {
                caster.Render(_commandBuffer,receiver);
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