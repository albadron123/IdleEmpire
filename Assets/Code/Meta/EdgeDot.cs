using UnityEngine;
using System;
using System.Collections.Generic;
class EdgeDot : MonoBehaviour
{
    Transform t;

    public Transform t1;
    public Transform t2;
    public float lerpValue;


    private void Start()
    {
        t = transform;
    }

    private void Update()
    {
        t.position = 5 * Vector3.forward + 0.5f * Vector3.up + Vector3.Lerp(t1.position, t2.position, lerpValue);
    }
}
