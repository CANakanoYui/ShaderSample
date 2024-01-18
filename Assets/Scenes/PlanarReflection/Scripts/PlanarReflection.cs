using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanarReflection : MonoBehaviour
{
   private List<Mesh> _meshes = new List<Mesh>();

   public List<Mesh> Meshes => _meshes;
   
   private void Start()
   {
      PlanarReflectionManager.Instance.PlanarReflection = this;
      
      var renderers = transform.GetComponentsInChildren<Renderer>();
      foreach ( var renderer in renderers)
      {
         bool isSkinnedMeshRenderer = renderer is SkinnedMeshRenderer;
         bool isMeshRenderer = renderer is MeshRenderer;
         if (isSkinnedMeshRenderer == false && isMeshRenderer == false)
         {
            continue;
         }
         
         // メッシュを収集
         if (isSkinnedMeshRenderer)
         {
            var skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
            _meshes.Add(skinnedMeshRenderer.sharedMesh);
                    
         }else
         {
            _meshes.Add((renderer.GetComponent<MeshFilter>().sharedMesh));
         }
      }
   }
}
