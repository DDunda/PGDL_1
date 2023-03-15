using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public static Transform cam;
    void Update()
    {
        transform.rotation = cam.rotation;
    }
}
