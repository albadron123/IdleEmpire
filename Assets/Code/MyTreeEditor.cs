using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class MyTreeEditor : MonoBehaviour
{
    [SerializeField] GameObject upgradeSpawnButtonPfb;
    [SerializeField] GameObject upgradeNodeStup;
    [SerializeField] LineRenderer sampleLine;

    Transform t;


    List<GameObject> buttons = null;

    public static MyTreeEditor inst;


    float scrollMinY = 0;
    float scrollMaxY = 10;

    NodeStub selectedNode = null;


    Dictionary<UpgradeHandle, NodeStub> instancesOfNodes = new Dictionary<UpgradeHandle, NodeStub>();


    void Start()
    {
        if (inst == null)
        {
            inst = this;
        }
        else
        {
            Debug.LogError("More then 1 MyTreeEditor on the scene!");
            Destroy(gameObject);
            return;
        }

        t = Camera.main.transform;

        InitButtons();

        if (DataStorage.nodeConnections == null)
        {
            DataStorage.nodeConnections = new List<KeyValuePair<UpgradeHandle, UpgradeHandle>>();
        }
        if (DataStorage.nodePositions == null)
        {
            DataStorage.nodePositions = new Dictionary<UpgradeHandle, Vector2>();
        }
        if (DataStorage.nodePositions.Count > 0 || DataStorage.nodeConnections.Count > 0)
        {
            LoadNodeStatesFromStorage();
        }
    }

    private void LoadNodeStatesFromStorage()
    {
        //Load nodes positions
        foreach (var element in DataStorage.nodePositions)
        {
            NodeStub node = CreateNode(element.Key, false);
            node.transform.position = new Vector3(element.Value.x, element.Value.y, node.transform.position.z);
        }
        //Load nodes connections
        foreach (var element in DataStorage.nodeConnections)
        {
            ConnectNodes(instancesOfNodes[element.Key], instancesOfNodes[element.Value], false);
        }
    }

    private void InitButtons()
    {
        buttons = MaximUtils.DrawGrid(upgradeSpawnButtonPfb, transform, new Vector3(-8f, -2.5f, 0), 0.1f, 1f, 12, DataStorage.allUpgrades.Length);

        for (int i = 0; i < buttons.Count; ++i)
        {
            buttons[i].GetComponent<SpriteRenderer>().sprite = DataStorage.allUpgrades[i].icon;
            int capturedIndex = i;
            buttons[i].GetComponent<Interactable>().e.AddListener(() => CreateNode((UpgradeHandle)capturedIndex, true));
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(G.SCENE_META);
        }

        ScrollTheScreen();
    }

    // TODO:
    // Set up the locations of nodes (in the Meta.cs) to be correct

    public NodeStub CreateNode(UpgradeHandle handle, bool modify)
    {
        if (!instancesOfNodes.ContainsKey(handle))
        {
            GameObject inst = Instantiate(upgradeNodeStup, new Vector3(t.position.x, t.position.y, 0), Quaternion.identity);
            NodeStub ns = inst.GetComponent<NodeStub>();
            UpgradeData myUpgrade = DataStorage.allUpgrades[(int)handle];
            ns.sr.sprite = myUpgrade.icon;
            ns.title.text = myUpgrade.title;
            ns.myHandle = handle;
            instancesOfNodes.Add(handle, ns);

            if (modify) DataStorage.nodePositions[handle] = t.position;

            return ns;
        }
        else
        {
            t.position = new Vector3(t.position.x, instancesOfNodes[handle].transform.position.y, t.position.z);
            t.position = new Vector3(t.position.x, Mathf.Clamp(t.position.y, scrollMinY, scrollMaxY), t.position.z);
        }
        return null;
    }

    public void SaveResults()
    {

    }

    public void ConnectNode(NodeStub n)
    {
        if (selectedNode == null)
        {
            selectedNode = n;
            return;
        }
        if (selectedNode == n)
        {
            selectedNode = null;
            return;
        }

        //Actual connection/disconnection

        if (DataStorage.nodeConnections.Exists(x => x.Key == selectedNode.myHandle && x.Value == n.myHandle || x.Key == n.myHandle && x.Value == selectedNode.myHandle))
        {
            DataStorage.nodeConnections.RemoveAll(x => x.Key == selectedNode.myHandle && x.Value == n.myHandle || x.Key == n.myHandle && x.Value == selectedNode.myHandle);
            LineRenderer lr = n.connections[selectedNode];
            Destroy(lr);
            n.connections.Remove(selectedNode);
            selectedNode.connections.Remove(n);
        }
        else
        {
            ConnectNodes(n, selectedNode, true);
        }

        selectedNode = null;
    }

    private void ConnectNodes(NodeStub n1, NodeStub n2, bool modify)
    {
        if(modify) DataStorage.nodeConnections.Add(new KeyValuePair<UpgradeHandle, UpgradeHandle>(n1.myHandle, n2.myHandle));
        LineRenderer lr = Instantiate(sampleLine);
        lr.SetPositions(new Vector3[] { n1.transform.position, n2.transform.position });
        n1.connections.Add(n2, lr);
        n2.connections.Add(n1, lr);
    }

    bool pressedToScroll = false;
    float initialScrollY = 0;
    float initialMouseScrollY = 0;
    private void ScrollTheScreen()
    {
        //The key way of scrolling
        float scrollingVelocity = 7.5f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            t.position += scrollingVelocity * Time.deltaTime * Vector3.up;

        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            t.position += scrollingVelocity * Time.deltaTime * Vector3.down;
        }

        //The MousePress way of scrolling
        if (Input.GetMouseButtonDown(0) && !MaximUtils.DoSquareOverlapAny(G.mousePosition, new Vector2(0.05f, 0.05f)))
        {
            pressedToScroll = true;
            initialScrollY = t.position.y;
            initialMouseScrollY = Input.mousePosition.y;
        }
        if (Input.GetMouseButtonUp(0))
        {
            pressedToScroll = false;
        }

        if (pressedToScroll)
        {
            float mouseDelta = initialMouseScrollY - Input.mousePosition.y;
            float worldDeltaY = (mouseDelta / (float)Screen.height) * ((float)Camera.main.orthographicSize * 2f);
            Debug.Log($"scroll: {Input.mousePosition.y }, {initialMouseScrollY} {Camera.main.ScreenToWorldPoint(new Vector3(0, Input.mousePosition.y - initialMouseScrollY, 0)).y}");
            t.position = new Vector3(t.position.x, initialScrollY + worldDeltaY, t.position.z);
        }

        //Clamp the position
        t.position = new Vector3(t.position.x, Mathf.Clamp(t.position.y, scrollMinY, scrollMaxY), t.position.z);
    }

}
