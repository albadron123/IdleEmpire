using UnityEngine;

public class GenGrass : MonoBehaviour
{
    [SerializeField] GameObject grassPrefab;
    [SerializeField] Color c1;
    [SerializeField] Color c2;
    [SerializeField] Color c3;
    [SerializeField] Color c4;
    [SerializeField] float maxMagnitude;

    void Start()
    {
        for (int i = 0; i < 50000; ++i)
        {
            GameObject inst = Instantiate(grassPrefab, new Vector3(Random.Range(-6f,6f), Random.Range(-6f, 6f), 1), Quaternion.identity);
            inst.GetComponent<SpriteRenderer>().color = Color.Lerp(c3, c4, Random.value);
            //inst.transform.localScale = new Vector3(Random.Range(-0.8f, 1.3f), Random.Range(-0.8f, 1.3f), 1);
        }


        



        
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            GenerateCircle(G.mousePosition, 1f);
        }
        if (Input.GetMouseButtonDown(1))
        {
            Gen1(G.mousePosition, 1f);
        }

    }


    void GenerateCircle(Vector2 position, float radius)    
    {
        for (int i = 0; i < 500/3*radius; ++i)
        {
            Vector2 pos = position + MaximUtils.RandomVector2FixMagnitude(radius + MaximUtils.Gaussian(0, 0.2f));
            GameObject inst = Instantiate(grassPrefab, new Vector3(pos.x, 0.8f * pos.y, 1), Quaternion.identity);
            inst.GetComponent<SpriteRenderer>().color = Color.Lerp(c1, c2, Random.value);
            float value = Random.Range(0.3f, 0.6f);
            inst.transform.localScale = new Vector3(value, value, 1);
        }

        
    }

    void Gen1(Vector2 position, float radius)
    {
        for (int i = 0; i < 10000 / 3 * radius; ++i)
        {
            Vector2 pos = position + MaximUtils.RandomVector2(0.85f * radius) + MaximUtils.RandomVector2(0.5f);
            GameObject inst = Instantiate(grassPrefab, new Vector3(pos.x, 0.8f * pos.y, 1), Quaternion.identity);
            inst.GetComponent<SpriteRenderer>().color = Color.Lerp(c1, c2, Random.value);
            //inst.transform.localScale = new Vector3(Random.Range(-0.8f, 1.3f), Random.Range(-0.8f, 1.3f), 1);
        }
    }
}
