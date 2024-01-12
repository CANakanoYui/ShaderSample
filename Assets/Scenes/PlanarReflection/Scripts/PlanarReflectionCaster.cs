using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlanarReflectionCaster : MonoBehaviour
{
   private List<Renderer> meshRenderers = new ();
   
   private List<Mesh> meshes = new ();

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
            meshes.Add(skinnedMeshRenderer.sharedMesh);
                    
         }else
         {
            meshes.Add((renderer.GetComponent<MeshFilter>().sharedMesh));
         }
         
         meshRenderers.Add(renderer);
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
      for( int i = 0; i < meshes.Count; i++)
      {
         var transform = meshRenderers[i].transform;
         var matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale );
         
         cmd.DrawMesh(meshes[i], matrix, _material);
      }
   }
}
