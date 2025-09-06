using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropellerSpin : MonoBehaviour
{
    [Tooltip("Rotations per minute")]
    public float rpm = 1200f;

    [Tooltip("Local axis to spin around. Default X axis (1,0,0). Use (0,0,1) or other if needed.")]
    public Vector3 localAxis = Vector3.right;

    [Tooltip("Invert rotation direction")]
    public bool invert = false;

    void Update()
    {
        float degPerSec = rpm * 360f / 60f;
        float angle = degPerSec * Time.deltaTime * (invert ? -1f : 1f);
        transform.Rotate(localAxis.normalized, angle, Space.Self);
    }
}
