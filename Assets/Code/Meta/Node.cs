using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Node : MonoBehaviour
{
    
    



    //Updrade View
    [Header("UpgradeView")]
    [SerializeField]
    GameObject upgradePointPrefab;
    [SerializeField]
    float updradePointDistance = 0.1f;
    [SerializeField]
    GameObject upgradePointContainer;
    [SerializeField]
    GameObject purchaseButton;


    [SerializeField]
    GameObject equipButton;
    [SerializeField]
    TMPro.TMP_Text equipText;


    List<GameObject> upgradePoints;



    [SerializeField] Color defaultColor = Color.white;
    [SerializeField] Color selectColor = Color.yellow;
    [SerializeField] Color disabledColor = Color.grey;


    Transform t;
    Vector3 initialPosition;
    Vector3 initialScale;

    [SerializeField]
    SpriteRenderer outlineSr;
    [SerializeField]
    TMPro.TMP_Text titleTe;
    [SerializeField]
    TMPro.TMP_Text priceTe;
    [SerializeField]
    SpriteRenderer spriteSr;
    [SerializeField]
    SpriteRenderer backgroundSr;



    float[] omega = new float[8];
    float[] phi = new float[8];
    float[] alpha = new float[8];

    public UpgradeHandle myHandle;


    private void Awake()
    {
        initialScale = transform.localScale;
        if (!G.upgradeStates[(int)myHandle].visible)
        {
            gameObject.SetActive(false);
        }
    }

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

        InitNode();
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

    private void OnMouseDown()
    {
        if (Meta.inst.currentNode == this)
        {
            DeselectNode();
        }
        else
        {
            SelectNode();
        }
    }

    void InitNode()
    {
        
        if (DataStorage.allUpgrades[(int)myHandle].isBuildingUpdrade && G.equippedBuildings.Contains(DataStorage.allUpgrades[(int)myHandle].buildingHandle.Value))
        {
            equipButton.SetActive(true);
            equipText.text = "equipped";
        }
        else
        {
            equipButton.SetActive(false);
            equipText.text = "equip";
        }
        
        DOTween.Sequence()
            .Append(t.DOScale(0, 0))
            .AppendInterval(0.35f)
            .Append(t.DOScale(initialScale.x, 1f).SetEase(Ease.OutCubic));
        
        //Draw updrades
        upgradePoints = MaximUtils.DrawCenteredListHor(upgradePointPrefab, upgradePointContainer.transform, Vector3.zero, updradePointDistance, DataStorage.allUpgrades[(int)myHandle].maxLvls, 0.1f);
        ColorUpgradePoints();

        //Style only
        int lvl = G.upgradeStates[(int)myHandle].upgradeLvl;
        int maxLvl = DataStorage.allUpgrades[(int)myHandle].maxLvls;
        if (lvl == 0)
        {
            outlineSr.color = disabledColor;
            titleTe.color = disabledColor;
            spriteSr.color = new Color(disabledColor.r, disabledColor.g, disabledColor.b, 0.5f);
        }
        else if (lvl < maxLvl)
        {
            outlineSr.color = defaultColor;
            titleTe.color = defaultColor;
            spriteSr.color = Color.white;
        }
        else
        {
            
            outlineSr.color = selectColor;
            titleTe.color = selectColor;
            spriteSr.color = Color.yellow;
        }

        if (lvl < maxLvl)
        {
            priceTe.text = DataStorage.CalculateUpgradePrice(myHandle).ToString();
        }
        else
        {
            priceTe.text = "inf";
        }


    }



    void ColorUpgradePoints()
    {
        int lvl = G.upgradeStates[(int)myHandle].upgradeLvl;
        int maxLvl = DataStorage.allUpgrades[(int)myHandle].maxLvls;
        if (lvl == 0)
        {
            for (int i = 0; i < maxLvl; ++i)
            {
                upgradePoints[i].GetComponent<SpriteRenderer>().color = disabledColor;
            }
        }
        else
        {
            // Not optimal but ok as there should not be a lot of upgrades
            for (int i = 0; i < lvl; ++i)
            {
                upgradePoints[i].GetComponent<SpriteRenderer>().color = selectColor;
            }
            for (int i = lvl; i < maxLvl; ++i)
            {
                upgradePoints[i].GetComponent<SpriteRenderer>().color = defaultColor;
            }
        }
    }

    bool selected = false;
    void SelectNode()
    {
        int lvl = G.upgradeStates[(int)myHandle].upgradeLvl;
        int maxLvl = DataStorage.allUpgrades[(int)myHandle].maxLvls;

        selected = true;
        if (lvl < maxLvl)
        {
            purchaseButton.SetActive(true);    
        }

        if (Meta.inst.currentNode != null)
        {
            Meta.inst.currentNode.DeselectNode();
        }
        Meta.inst.currentNode = this;
        t.DOKill();
        t.DOScale(1.1f * initialScale, 0.15f);
        outlineSr.color = selectColor;
        titleTe.color = selectColor;
        if (DataStorage.allUpgrades[(int)myHandle].isBuildingUpdrade && G.upgradeStates[(int)myHandle].upgradeLvl > 0)
        {
            equipButton.SetActive(true);
        }
    }

    void DeselectNode()
    {
        int lvl = G.upgradeStates[(int)myHandle].upgradeLvl;
        int maxLvl = DataStorage.allUpgrades[(int)myHandle].maxLvls;

        selected = false;
        Meta.inst.currentNode = null;
        t.DOKill();
        t.DOScale(1f*initialScale, 0.15f);
        if (lvl >= maxLvl)
        {
            outlineSr.color = selectColor;
        }
        else if (lvl == 0)
        {
            outlineSr.color = disabledColor;
            titleTe.color = disabledColor;
            spriteSr.color = new Color(disabledColor.r, disabledColor.g, disabledColor.b, 0.5f);
        }
        else
        {
            outlineSr.color = defaultColor;
            titleTe.color = defaultColor;
        }
        purchaseButton.SetActive(false);
        if (DataStorage.allUpgrades[(int)myHandle].isBuildingUpdrade && !G.equippedBuildings.Contains(DataStorage.allUpgrades[(int)myHandle].buildingHandle.Value))
        {
            equipButton.SetActive(false);
        }
    }

    public void AquireUpgade()
    {
        int currentPrice = DataStorage.CalculateUpgradePrice(myHandle);
        if (!MetaEconomy.inst.UpdateBones(-currentPrice))
        {
            t.DOKill();
            Sequence seq = DOTween.Sequence();
            seq.Append(t.DORotate(new Vector3(0, 0, 5), 0.04f));
            seq.Append(t.DORotate(new Vector3(0, 0, -5), 0.08f));
            seq.Append(t.DORotate(new Vector3(0, 0, 0), 0.04f));
            seq.SetLoops(2);
            seq.OnKill(() => { t.rotation = Quaternion.identity; });


            Debug.Log("Upgrade not equired");
            return;
        }

        StartCoroutine(Meta.inst.UpdateBonesSpentSlider(G.bonesSpent + currentPrice));

        int lvl = G.upgradeStates[(int)myHandle].upgradeLvl;
        int maxLvl = DataStorage.allUpgrades[(int)myHandle].maxLvls;

        List<UpgradeHandle> upgradeHandles = new List<UpgradeHandle>();

        bool spectialCondition = lvl == 1 && (myHandle == UpgradeHandle.Cubo || myHandle == UpgradeHandle.Bubil || myHandle == UpgradeHandle.Tawa);
        if (lvl == 0 || spectialCondition)
        {
            foreach (Edge e in Meta.inst.edges)
            {
                if (e.a == this && !G.upgradeStates[(int)e.b.myHandle].visible)
                {
                    upgradeHandles.Add(e.b.myHandle);
                    e.b.gameObject.transform.localScale = Vector3.zero;
                    e.b.gameObject.SetActive(true);
                    StartCoroutine(Meta.inst.RenderNewEdge(e, 1));
                }
                if (e.b == this && !G.upgradeStates[(int)e.a.myHandle].visible)
                {
                    upgradeHandles.Add(e.a.myHandle);
                    e.a.gameObject.transform.localScale = Vector3.zero;
                    e.a.gameObject.SetActive(true);
                    StartCoroutine(Meta.inst.RenderNewEdge(e, -1));
                }
            }
        }


        G.AquireUpgrade(myHandle, upgradeHandles);
        //now we *know* that level is increased in G.AquireUpgrade(...)
        ++lvl;

        //visually change the current upgrade
        if (lvl == 1)
        {
            spriteSr.color = defaultColor;
            if (DataStorage.allUpgrades[(int)myHandle].isBuildingUpdrade)
            {
                equipButton.SetActive(true);
            }
        }
        if (lvl == maxLvl)
        {
            // Fully upgraded
            purchaseButton.SetActive(false);

            // TODO: inf symbol later
            priceTe.text = "inf";

            DeselectNode();

            outlineSr.color = selectColor;
            titleTe.color = selectColor;
            spriteSr.color = selectColor;
        }
        else
        {
            priceTe.text = DataStorage.CalculateUpgradePrice(myHandle).ToString();
        }
        ColorUpgradePoints();

        
        
    }

    
    public void PressEquip()
    {
        
        if (DataStorage.allUpgrades[(int)myHandle].isBuildingUpdrade && !G.equippedBuildings.Contains(DataStorage.allUpgrades[(int)myHandle].buildingHandle.Value))
        {
            bool success = Meta.inst.AddToEquipmentList(this);
            if (success)
            {
                equipText.text = "unequip?";
            }
            else
            {
                //fails   
                t.DOKill();
                Sequence seq = DOTween.Sequence();
                seq.Append(t.DORotate(new Vector3(0, 0, 5), 0.04f));
                seq.Append(t.DORotate(new Vector3(0, 0, -5), 0.08f));
                seq.Append(t.DORotate(new Vector3(0, 0, 0), 0.04f));
                seq.SetLoops(2);
                seq.OnKill(() => { t.rotation = Quaternion.identity; });
            }

        }
        else
        {
            if (!selected)
            {
                equipButton.SetActive(false);
                equipButton.GetComponent<SpriteRenderer>().color = defaultColor;
            }
            equipText.text = "equip";
            Meta.inst.RemoveFromEquipmentList(this);
        }
    }

    public void EnterPurchaseButton() 
    {
        
        purchaseButton.GetComponent<SpriteRenderer>().color = selectColor;
    }
    
    public void ExitPurchaseButton() 
    {
        purchaseButton.GetComponent<SpriteRenderer>().color = defaultColor;
    }

    public void EnterEquipButton()
    {
        equipButton.GetComponent<SpriteRenderer>().color = selectColor;
        if (G.equippedBuildings.Contains(DataStorage.allUpgrades[(int)myHandle].buildingHandle.Value))
        {
            equipText.text = "unequip?";
        }
    }

    public void ExitEquipButton()
    {
        equipButton.GetComponent<SpriteRenderer>().color = defaultColor;
        if (G.equippedBuildings.Contains(DataStorage.allUpgrades[(int)myHandle].buildingHandle.Value))
        {
            equipText.text = "equipped";
        }
        else
        {
            equipText.text = "equip";
        }
    }
}
