using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanarReflectionCameraController : MonoBehaviour
{
    private Camera _camera;

    private void Start()
    {
        _camera = GetComponent<Camera>();
    }
    
    private void LateUpdate()
    {
        _camera.fieldOfView = Camera.main.fieldOfView;
        _camera.transform.position = new Vector3(Camera.main.transform.position.x, -Camera.main.transform.position.y, Camera.main.transform.position.z);
        _camera.transform.rotation = Quaternion.Euler(-Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, 0f);
    }
}
