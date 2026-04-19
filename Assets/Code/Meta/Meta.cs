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
    TMPro.TMP_Text devilsTe;

    [SerializeField]
    TMPro.TMP_Text bonesSpentTe;
    [SerializeField]
    Transform bonesSpentArrow;
    [SerializeField]
    Transform bonesSpentSlider;

    [SerializeField] Sprite resurrectButtonSpr;
    [SerializeField] Sprite resurrectButtonFocusSpr;

    bool canResurrect = true;



    void InitBonesSpentSlider()
    {
        bonesSpentSlider.localScale = new Vector3(0, bonesSpentSlider.localScale.y, bonesSpentSlider.localScale.z);
        bonesSpentArrow.localPosition = new Vector3(0, bonesSpentArrow.transform.position.y, bonesSpentArrow.transform.position.z);
        bonesSpentTe.text = $"{0}/{G.maxBonesSpent}";
        
        StartCoroutine(UpdateBonesSpentSlider(0));
    }

    public IEnumerator UpdateBonesSpentSlider(int newBonesSpent)
    {
        const float UPDATE_TIME = 0.75f;

        int oldBonesSpent = G.bonesSpent;
        G.bonesSpent = Mathf.Clamp(newBonesSpent, 0, G.maxBonesSpent);
        float fraction = (float)G.bonesSpent / G.maxBonesSpent;
        bonesSpentSlider.DOScale(new Vector3(fraction, bonesSpentSlider.localScale.y, bonesSpentSlider.localScale.z), UPDATE_TIME);
        bonesSpentArrow.DOLocalMove(new Vector3(fraction, bonesSpentArrow.localPosition.y, bonesSpentArrow.localPosition.z), UPDATE_TIME);
        float timer = 0;
        while (timer < UPDATE_TIME)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            if (timer >= UPDATE_TIME)
            {
                timer = UPDATE_TIME;
                bonesSpentTe.text = $"{G.bonesSpent}/{G.maxBonesSpent}";
            }
            else
            {
                bonesSpentTe.text = $"{(int)(Mathf.Lerp(oldBonesSpent, G.bonesSpent, timer / UPDATE_TIME))}/{G.maxBonesSpent}";
            }
        }
    }


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


        
        canResurrect = G.equippedBuildings.Count > 0;
        ShowEquipmentList();

        InitBonesSpentSlider();
        StartCoroutine(RenderEdges());

        StartCoroutine(SoundManager.inst.ChangeBackgroundProfile(new AudioClip[2] { DataStorage.SOUND_FIREPLACE, DataStorage.SOUND_FIREPLACE_MUSIC}, new float[]{0.35f,0.2f}, new float[]{1f,0.8f}));
        SoundManager.inst.AddBackground(DataStorage.SOUND_BROWN_NOISE, 0.04f, 1, true);
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
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            StartCoroutine(SoundManager.inst.ChangeBackgroundProfile(new AudioClip[2] { DataStorage.SOUND_FIREPLACE, DataStorage.SOUND_FIREPLACE_MUSIC }, new float[] { 0.35f, 0.2f }, new float[] { 1f, 0.8f }, 5));
        }
        MaximUtils.RenderShakyText(devilsTe, 0.012f, 15);

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
