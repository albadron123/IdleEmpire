using UnityEngine;
using System.Collections.Generic;

public class NodeStub : MonoBehaviour
{
    Transform t;
    public TMPro.TMP_Text title;
    public SpriteRenderer sr;
    public UpgradeHandle myHandle;

    public Dictionary<NodeStub, LineRenderer> connections = new Dictionary<NodeStub, LineRenderer>();


    bool isDragged = false;

    private void Start()
    {
        t = transform;        
    }

    private void Update()
    {
        if (isDragged)
        {
            transform.position = new Vector3(G.mousePosition.x, G.mousePosition.y, t.position.z);
            foreach (var edge in connections)
            {
                edge.Value.SetPositions(new Vector3[] { t.position, edge.Key.transform.position });
            }
        }
    }

    private void OnMouseDown()
    {
        isDragged = true;
    }

    private void OnMouseUp()
    {
        isDragged = false;
        DataStorage.nodePositions[myHandle] = t.position;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            MyTreeEditor.inst.ConnectNode(this);
        }   
    }
}

