using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateController : MonoBehaviour
{
    private float _speed = 0.1f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, _speed, 0);
    }
}
