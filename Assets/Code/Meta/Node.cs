using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    Transform t;
    Vector3 initialPosition;
    float[] omega = new float[8];
    float[] phi = new float[8];
    float[] alpha = new float[8];


    void Start()
    {
        t = transform;
        initialPosition = t.position;
        for (int i = 0; i < 8; ++i)
        {
            omega[i] = Random.Range(0.1f * Mathf.PI, 0.2f * Mathf.PI);
            phi[i] = Random.Range(0, 2 * Mathf.PI);
            alpha[i] = Random.Range(0.1f, 0.2f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float offsetX = alpha[0] * Mathf.Sin(omega[0] * Time.time + phi[0]) +
                        alpha[1] * Mathf.Sin(omega[1] * Time.time + phi[1]) +
                        alpha[2] * Mathf.Sin(omega[2] * Time.time + phi[2]) +
                        alpha[3] * Mathf.Sin(omega[3] * Time.time + phi[3]);
        float offsetY = alpha[4] * Mathf.Sin(omega[4] * Time.time + phi[4]) +
                        alpha[5] * Mathf.Sin(omega[5] * Time.time + phi[5]) +
                        alpha[6] * Mathf.Sin(omega[6] * Time.time + phi[6]) +
                        alpha[7] * Mathf.Sin(omega[7] * Time.time + phi[7]);
        t.position = initialPosition + new Vector3(offsetX, offsetY, 0);
    }
}
