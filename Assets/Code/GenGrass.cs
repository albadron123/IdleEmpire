using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenGrass : MonoBehaviour
{
    [SerializeField] GameObject grassPrefab;
    [SerializeField] Color c1;
    [SerializeField] Color c2;
    void Start()
    {
        for (int i = 0; i < 100000; ++i)
        {
            GameObject inst = Instantiate(grassPrefab, new Vector3(Random.Range(-6.0f, 6.0f), Random.Range(-6.0f, 6.0f), 1), Quaternion.identity);
            inst.GetComponent<SpriteRenderer>().color = Color.Lerp(c1, c2, Random.value);
            //inst.transform.localScale = new Vector3(Random.Range(-0.8f, 1.3f), Random.Range(-0.8f, 1.3f), 1);
        }
    }

    
    void Update()
    {
        
    }
}
