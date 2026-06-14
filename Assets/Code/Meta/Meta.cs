using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

[System.Serializable]
public struct Edge
{
    public Node a;
    public Node b;
}


public class Meta : MonoBehaviour
{
    [SerializeField] Sprite defaultCursorSpr;

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


    
    [SerializeField]
    SpriteRenderer[] equipmentListSr;

    Transform t;

    [SerializeField]
    TMPro.TMP_Text bonesSpentTe;
    [SerializeField]
    Transform bonesSpentArrow;
    [SerializeField]
    Transform bonesSpentSlider;

    [SerializeField] Sprite resurrectButtonSpr;
    [SerializeField] Sprite resurrectButtonFocusSpr;

    bool canResurrect = true;

    [SerializeField] Node nodePfb;



    void ShowEquipmentList()
    {
        for (int i = 0; i < G.equippedBuildingsSize; ++i)
        {
            equipmentListSr[i].transform.parent.gameObject.SetActive(true);
            if (i < G.equippedBuildings.Count)
            {
                equipmentListSr[i].sprite = DataStorage.allBuildings[(int)G.equippedBuildings[i]].icon;
            }
            else
            {
                equipmentListSr[i].sprite = null;
            }
        }
        for (int i = G.equippedBuildingsSize; i < G.equippedBuildingsCapacity; ++i)
        {
            equipmentListSr[i].transform.parent.gameObject.SetActive(false);
        }
    }

    public void IncrementEquipmentListSize()
    {
        G.UpgradeEquipBuildingsSize();
        ShowEquipmentList();
    }

    public bool AddToEquipmentList(Node n)
    {
        if (G.equippedBuildingsSize <= G.equippedBuildings.Count)
        {
            return false;
        }
        G.equippedBuildings.Add(DataStorage.allUpgrades[(int)n.myHandle].buildingHandle.Value);
        equipmentListSr[G.equippedBuildings.Count - 1].sprite = DataStorage.allBuildings[(int)DataStorage.allUpgrades[(int)n.myHandle].buildingHandle.Value].icon;

        G.SaveEquipmentListToPlayerPrefs();

        canResurrect = G.equippedBuildings.Count > 0;
        return true;
    }

    
    public void RemoveFromEquipmentList(Node n)
    {
        int index = G.equippedBuildings.IndexOf(DataStorage.allUpgrades[(int)n.myHandle].buildingHandle.Value);
        G.equippedBuildings.RemoveAt(index);
        for (int i = index; i < G.equippedBuildingsSize; ++i)
        {           
            if (i < G.equippedBuildings.Count)
            {
                equipmentListSr[i].sprite = ((i+1) != G.equippedBuildingsSize) ? 
                                                equipmentListSr[i + 1].sprite : 
                                                null;
            }
            else
            {
                equipmentListSr[i].sprite = null;
            }
        }

        G.SaveEquipmentListToPlayerPrefs();

        canResurrect = G.equippedBuildings.Count > 0;
    }

    void Start()
    {
        t = transform;
        if (inst != null)
        {
            Debug.LogError("Meta.cs is singleton!");
            Destroy(this);
        }
        else
        {
            inst = this;
        }

        InstantiateAllNodes();
            

        G.SetCursor(defaultCursorSpr);
        
        canResurrect = G.equippedBuildings.Count > 0;
        ShowEquipmentList();

        
        StartCoroutine(RenderEdges());

        StartCoroutine(SoundManager.inst.ChangeBackgroundProfile(new AudioClip[2] { DataStorage.SOUND_FIREPLACE, DataStorage.SOUND_FIREPLACE_MUSIC}, new float[]{0.35f,0.2f}, new float[]{1f,0.8f}));
        SoundManager.inst.AddBackground(DataStorage.SOUND_BROWN_NOISE, 0.04f, 1, true);
    }

    private void InstantiateAllNodes()
    {
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
            nodePfb.gameObject.SetActive(false);
            Dictionary<UpgradeHandle, Node> allNodes = new Dictionary<UpgradeHandle, Node>();
            foreach (var element in DataStorage.nodePositions)
            {
                Node inst = Instantiate(nodePfb, new Vector3(element.Value.x, element.Value.y, nodePfb.transform.position.z), Quaternion.identity);
                inst.myHandle = element.Key;
                allNodes[element.Key] = inst;
            }
            edges = new List<Edge>();
            foreach (var element in DataStorage.nodeConnections)
            {
                edges.Add(new Edge() { a = allNodes[element.Key], b = allNodes[element.Value] });
            }
            nodePfb.gameObject.SetActive(true);
            foreach (var node in allNodes.Values)
            {
                node.gameObject.SetActive(true);
            }
        }
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
                if (G.upgradeStates[(int)edges[i].a.myHandle].visible && G.upgradeStates[(int)edges[i].b.myHandle].visible)
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

    public void StartAgain(Transform button)
    {
        if (canResurrect)
        {
            SceneManager.LoadScene("SampleScene");
        }
        else
        {
            MaximUtils.DOCancelShake(button);
        }
    }

    public void MouseEnterResurrect(SpriteRenderer sr)
    {
        sr.sprite = resurrectButtonFocusSpr;
    }

    public void MouseExitResurrect(SpriteRenderer sr)
    {
        sr.sprite = resurrectButtonSpr;
    }



    private void Update()
    {
        if (Bootloader.BUILD_WITH_EDITOR)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                SceneManager.LoadScene(G.SCENE_META_EDITOR);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            StartCoroutine(SoundManager.inst.ChangeBackgroundProfile(new AudioClip[2] { DataStorage.SOUND_FIREPLACE, DataStorage.SOUND_FIREPLACE_MUSIC }, new float[] { 0.35f, 0.2f }, new float[] { 1f, 0.8f }, 5));
        }


        ScrollTheScreen();
        /*
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
        */
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
