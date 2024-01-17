using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlanarReflectionCaster : MonoBehaviour
{
   private readonly List<Renderer> _meshRenderers = new List<Renderer>();
   
   private readonly List<Material> _materials = new List<Material>();
   
   private readonly List<Mesh> _meshes = new List<Mesh>();

   private void Start()
   {
      var casterMaterial = PlanarReflectionPass.Instance.CasterMaterial;
      var renderers = transform.GetComponentsInChildren<Renderer>();
      foreach (var renderer in renderers)
      {
         bool isSkinnedMeshRenderer = renderer is SkinnedMeshRenderer;
         bool isMeshRenderer = renderer is MeshRenderer;
         if (isSkinnedMeshRenderer == false && isMeshRenderer == false)
         {
            continue;
         }
         
         if (isSkinnedMeshRenderer)
         {
            var skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
            _meshes.Add(skinnedMeshRenderer.sharedMesh);
                    
         }else
         {
            _meshes.Add((renderer.GetComponent<MeshFilter>().sharedMesh));
         }
         
         var material = new Material(casterMaterial);
         material.mainTexture = renderer.sharedMaterial.mainTexture;
         _meshRenderers.Add(renderer);
         _materials.Add(material);
      }
   }

   private void Update()
   {
      var pass = PlanarReflectionPass.Instance;
      pass?.AddPlanarReflectionCaster(this);
   }

   public void Render(CommandBuffer cmd,PlanarReflectionReceiver reciever)
   {
      Vector3 axisX = new Vector3();
      Vector3 axisY = new Vector3();
      Vector3 axisZ = new Vector3();
      // 平面の上方向がZ軸になる
      axisZ = reciever.transform.rotation * Vector3.up;
      axisX = reciever.transform.rotation * Vector3.right;
      axisY = reciever.transform.rotation * Vector3.forward;
      
      // 平面空間になる行列を作成する
      Matrix4x4 mPlaneToWorld = new Matrix4x4();
      mPlaneToWorld.SetColumn(0, axisX.normalized);
      mPlaneToWorld.SetColumn(1, axisY.normalized);
      mPlaneToWorld.SetColumn(2, axisZ.normalized);
      UnityEngine.Vector4 pos = reciever.transform.position;
      pos.w = 1.0f;
      mPlaneToWorld.SetColumn(3, pos);
      
      var mWorldToPlane = Matrix4x4.Inverse(mPlaneToWorld);

      for( int i = 0; i < _materials.Count; i++)
      {
         var mesh = _meshes[i];
         var transform = _meshRenderers[i].transform;
         var matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
         
         var mReflection = Matrix4x4.Scale(new Vector3(1.0f, 1.0f, -1.0f));
         matrix = mPlaneToWorld * mReflection * mWorldToPlane * matrix;
                
         cmd.DrawMesh(mesh, matrix, _materials[i]);
      } 
   }
}
