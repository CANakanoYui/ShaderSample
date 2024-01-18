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

    private const string ProfilerTag = nameof(PlanarReflectionPass);

    private ShaderTagId _shaderTagId = new ShaderTagId("ReflectionCaster");

    private ProfilingSampler _profilingSampler = new ProfilingSampler(ProfilerTag);

    private int _width;

    private int _height;
    
    private float _clipPlaneOffset = 0.07f;

    private float _planeOffset;
    
    private Material _material;

    public PlanarReflectionPass(Shader shader,int width, int height)
    {
        _instance = this;
        renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;

        _material = CoreUtils.CreateEngineMaterial(shader);
        _width = Mathf.Max(width, 2);
        _height = Mathf.Max(height, 2);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var cmd = CommandBufferPool.Get();

        using (new ProfilingScope(cmd, _profilingSampler))
        {
            cmd.GetTemporaryRT(_reflectionId, _width, _height, 32);
            cmd.SetRenderTarget(_reflectionId);
            cmd.ClearRenderTarget(true, true, Color.black, 1.0f);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            
            if (PlanarReflectionManager.Instance.PlanarReflection != null)
            {
                foreach (var mesh in PlanarReflectionManager.Instance.PlanarReflection.Meshes)
                {
                    cmd.DrawMesh(mesh,PlanarReflectionManager.Instance.PlanarReflection.transform.localToWorldMatrix,_material);
                }   
                
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
            }

            var camera = renderingData.cameraData.camera;
            Vector3 pos = Vector3.zero;
            Vector3 normal = Vector3.up;

            if (PlanarReflectionManager.Instance.PlanarReflection != null)
            {
                var transform = PlanarReflectionManager.Instance.PlanarReflection.transform;
                pos = transform.position + Vector3.up * _planeOffset;
                normal = transform.up;
            }
            
            var d = -Vector3.Dot(normal, pos) - _clipPlaneOffset;
            var reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);
            var reflection = Matrix4x4.identity;
            reflection *= Matrix4x4.Scale(new Vector3(1, -1, 1));

            CalculateReflectionMatrix(ref reflection, reflectionPlane);

            var reflectionViewMatrix = camera.worldToCameraMatrix * reflection;

            var clipPlane = CameraSpacePlane(reflectionViewMatrix, pos - Vector3.up * 0.1f, normal, 1.0f);
            var projection = camera.CalculateObliqueMatrix(clipPlane);

            var cullMat = Matrix4x4.Frustum(-1,1,-1,1, camera.nearClipPlane,  camera.farClipPlane);
            camera.TryGetCullingParameters(out var cullingParameters);
            cullingParameters.cullingMatrix = cullMat * reflectionViewMatrix;

            var cullingResults = context.Cull(ref cullingParameters);
            var sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
            var drawingSettings = CreateDrawingSettings(_shaderTagId, ref renderingData, sortFlags);
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
            filteringSettings.renderingLayerMask = 1;

            // 反射用の行列に差し替える
            cmd.SetViewProjectionMatrices(reflectionViewMatrix, projection);
            context.ExecuteCommandBuffer(cmd);

            context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);

            // 元に戻す
            var cameraData = renderingData.cameraData;
            cmd.SetViewProjectionMatrices(cameraData.GetViewMatrix(), cameraData.GetProjectionMatrix());
            cmd.SetGlobalVector(_reflectionId, cameraData.worldSpaceCameraPos);
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

    private Vector4 CameraSpacePlane(Matrix4x4 worldToCameraMatrix, Vector3 pos, Vector3 normal, float sideSign)
    {
        var offsetPos = pos + normal * _clipPlaneOffset;
        var m = worldToCameraMatrix;
        var cameraPosition = m.MultiplyPoint(offsetPos);
        var cameraNormal = m.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(cameraNormal.x, cameraNormal.y, cameraNormal.z, -Vector3.Dot(cameraPosition, cameraNormal));
    }

    private void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
    {
        reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
        reflectionMat.m01 = (-2F * plane[0] * plane[1]);
        reflectionMat.m02 = (-2F * plane[0] * plane[2]);
        reflectionMat.m03 = (-2F * plane[3] * plane[0]);

        reflectionMat.m10 = (-2F * plane[1] * plane[0]);
        reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
        reflectionMat.m12 = (-2F * plane[1] * plane[2]);
        reflectionMat.m13 = (-2F * plane[3] * plane[1]);

        reflectionMat.m20 = (-2F * plane[2] * plane[0]);
        reflectionMat.m21 = (-2F * plane[2] * plane[1]);
        reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
        reflectionMat.m23 = (-2F * plane[3] * plane[2]);

        reflectionMat.m30 = 0F;
        reflectionMat.m31 = 0F;
        reflectionMat.m32 = 0F;
        reflectionMat.m33 = 1F;
    }
}