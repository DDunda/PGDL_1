using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FC_Controller : MonoBehaviour
{
    void Start()
    {
        FaceCamera.cam = Camera.main.transform;
    }

    void Update()
    {
        FaceCamera.cam = Camera.main.transform;
    }
}
