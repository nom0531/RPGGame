using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationOrientationCamera : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        var lookAtCamera = Camera.main.transform.position;
        lookAtCamera.y = transform.position.y;  // ï‚ê≥
        transform.LookAt(lookAtCamera);
    }
}
