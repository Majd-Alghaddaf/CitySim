using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] PlayerMovement target;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target.transform);
    }
}
