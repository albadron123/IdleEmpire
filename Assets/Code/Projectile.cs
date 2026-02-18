using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public Vector3 direction = Vector3.right;
    public float velocity = 2;
    public int damage = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        transform.position += direction * Time.deltaTime * velocity;
    }
}
