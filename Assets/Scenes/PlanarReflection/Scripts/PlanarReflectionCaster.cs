using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlanarReflectionCaster : MonoBehaviour
{
   private readonly List<Renderer> _meshRenderers = new ();
   
   private readonly List<Mesh> _meshes = new ();

   private Material _material;

   private void Start()
   {
      var renderers = transform.GetComponentsInChildren<Renderer>();
      foreach (var renderer in renderers)
      {
         bool isSkinnedMeshRenderer = renderer is SkinnedMeshRenderer;
         bool isMeshRenderer = renderer is MeshRenderer;
         if (isSkinnedMeshRenderer == false
             && isMeshRenderer == false)
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
         
         _meshRenderers.Add(renderer);
         _material = renderer.material;
      }
   }

   private void Update()
   {
      var pass = PlanarReflectionPass.Instance;
      pass?.AddPlanarReflectionCaster(this);
   }

   public void Render(CommandBuffer cmd)
   {
      for( int i = 0; i < _meshes.Count; i++)
      {
         var transform = _meshRenderers[i].transform;
         var matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale );
         
         cmd.DrawMesh(_meshes[i], matrix, _material);
      }
   }
}
