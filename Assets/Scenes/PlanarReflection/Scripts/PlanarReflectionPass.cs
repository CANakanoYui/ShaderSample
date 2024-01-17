using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlanarReflectionPass : ScriptableRenderPass
{
    private static PlanarReflectionPass _instance;

    public static PlanarReflectionPass Instance => _instance;

    private int _reflectionId = Shader.PropertyToID("_ReflectionTex");

    private static ProfilingSampler _profilingSampler = new ProfilingSampler(nameof(PlanarReflectionPass));
    
    private int _width;

    private int _height;

    public PlanarReflectionPass(int width,int height)
    {
        _instance = this;
        renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
        
        _width = Mathf.Max(width, 2);
        _height = Mathf.Max(height, 2);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.camera != Camera.main) return;

        var cmd = CommandBufferPool.Get();
        
        using (new ProfilingScope(cmd, _profilingSampler))
        {
            var originalDesc = renderingData.cameraData.cameraTargetDescriptor;
            cmd.GetTemporaryRT(_reflectionId, _width, _height, originalDesc.depthBufferBits);
            cmd.SetRenderTarget(_reflectionId);
            cmd.ClearRenderTarget(true, true, Color.black, 1.0f);

            // foreach (var caster in _casters)
            // {
            //     caster.Render(cmd, _receiver);
            // }
        }

        // 擬似的なカメラを作って↓カリングデータ拾ってきて。
        // renderingData.cameraData.camera.TryGetCullingParameters();

        // ビュー行列 = 平面から見た行列

        // 射影行列 = CalculateObliqueMatrix(平面形を送る)
        // cmd.SetProjectionMatrix();

        // カメラの止水台を作る。
        // context.Cull();

        // context.DrawRenderers();

        // マスク画像を作るのはDrawMesh(平面)でやってて ColorMask0 Stencil Always
        // RenderingLayMask

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void FrameCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(_reflectionId);
    }
}