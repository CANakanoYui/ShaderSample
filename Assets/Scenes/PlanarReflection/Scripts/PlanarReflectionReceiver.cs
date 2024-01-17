using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanarReflectionReceiver : MonoBehaviour
{
    private RenderTexture _reflectionTexture;

    public RenderTexture ReflectionTexture => _reflectionTexture;

    [SerializeField] private int _width = 512;
    [SerializeField] private int _height = 512;

    private void Start()
    {
        int w = Mathf.Max(_width, 2);
        int h = Mathf.Max(_height, 2);
        _reflectionTexture = new RenderTexture(_width, _height, 32, RenderTextureFormat.RGB111110Float);

        ChangeReflectionMaterial();
    }
    
    private void Update()
    {
        var pass = PlanarReflectionPass.Instance;
        pass?.AddPlanarReflectionReceiver(this);
    }

    private void ChangeReflectionMaterial()
    {
        var srcMaterial = PlanarReflectionPass.Instance.ReceiverMaterial;
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

            var newMaterial = new Material(srcMaterial);
            newMaterial.mainTexture = renderer.material.mainTexture;
            newMaterial.SetTexture("_ReflectionTex", _reflectionTexture);
            renderer.sharedMaterial = newMaterial;
        }
    }
}
