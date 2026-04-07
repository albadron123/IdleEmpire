using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct Edge
{
    public Node a;
    public Node b;
}


public class Meta : MonoBehaviour
{
    public static Meta inst = null;

    [HideInInspector]
    public Node currentNode = null;

    public List<Edge> edges;

    [SerializeField]
    GameObject edgeDotPfb;

    [SerializeField]
    const int DOTS_IN_EDGE = 10;

    [SerializeField]
    float scrollMinY;
    [SerializeField]
    float scrollMaxY;

    Transform t;


    void Start()
    {
        t = transform;
        PlayerPrefs.DeleteAll();
        if (inst != null)
        {
            Debug.LogError("Meta.cs is singleton!");
            Destroy(this);
        }
        else
        {
            inst = this;
        }

        StartCoroutine(RenderEdges());
    }


    IEnumerator RenderEdges()
    {
        yield return new WaitForSeconds(0.5f);
        List<int> perms = MaximUtils.RandomPermutations(0, DOTS_IN_EDGE);
        float waitTime = 0.01f;
        while (perms.Count > 0)
        {
            int j = perms[perms.Count-1];
            perms.RemoveAt(perms.Count - 1);
            for (int i = 0; i < edges.Count; ++i)
            {
                if (edges[i].a.visible && edges[i].b.visible)
                {
                    Transform aT = edges[i].a.transform;
                    Transform bT = edges[i].b.transform;

                    GameObject inst = Instantiate(edgeDotPfb, 5 * Vector3.forward + 0.5f * Vector3.up + Vector3.Lerp(aT.position, aT.position, 0.1f + 0.8f * ((float)j / DOTS_IN_EDGE)), Quaternion.identity);
                    inst.GetComponent<EdgeDot>().t1 = aT;
                    inst.GetComponent<EdgeDot>().t2 = bT;
                    inst.GetComponent<EdgeDot>().lerpValue = 0.1f + 0.8f * ((float)j / DOTS_IN_EDGE);
                }
            }
            yield return new WaitForSeconds(waitTime);
            waitTime += 0.01f;
        }
    }

    public IEnumerator RenderNewEdge(Edge e, int direction)
    {
        //direction == 1 -- from a to b
        //direction == -1 -- from b to a
        Transform aT;
        Transform bT;
        if (direction > 0)
        {
            aT = e.a.transform;
            bT = e.b.transform;
        }
        else
        {
            bT = e.a.transform;
            aT = e.b.transform;
        }

        for (int i = 0; i < DOTS_IN_EDGE; ++i)
        {
            GameObject inst = Instantiate(edgeDotPfb, 5 * Vector3.forward + 0.5f * Vector3.up + Vector3.Lerp(aT.position, aT.position, 0.1f + 0.8f * ((float)i / DOTS_IN_EDGE)), Quaternion.identity);
            inst.GetComponent<EdgeDot>().t1 = aT;
            inst.GetComponent<EdgeDot>().t2 = bT;
            inst.GetComponent<EdgeDot>().lerpValue = 0.1f + 0.8f * ((float)i / DOTS_IN_EDGE);
            yield return new WaitForSeconds(0.05f);
        }

    }

    public void StartAgain()
    {
        SceneManager.LoadScene("SampleScene");
    }



    private void Update()
    {
        float mousePositionYClamped = Mathf.Clamp(Camera.main.ScreenToWorldPoint(Input.mousePosition).y, t.position.y - 5f, t.position.y + 5f);
        if (mousePositionYClamped > t.position.y + 4f)
        {
            float dist = Mathf.Abs(t.position.y + 4f - mousePositionYClamped);
            t.position = Vector3.Lerp(t.position, new Vector3(t.position.x, mousePositionYClamped, t.position.z), 2f*Time.deltaTime * Mathf.Pow(dist, 2));
        }
        if (mousePositionYClamped < t.position.y - 4.5f)
        {
            float dist = Mathf.Abs(t.position.y - 4.5f - mousePositionYClamped);
            t.position = Vector3.Lerp(t.position, new Vector3(t.position.x, mousePositionYClamped, t.position.z), 6f*Time.deltaTime * Mathf.Pow(dist, 2));
        }
        t.position = new Vector3(t.position.x, Mathf.Clamp(t.position.y, scrollMinY, scrollMaxY), t.position.z);
    }
}
