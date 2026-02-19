using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Node : MonoBehaviour
{
    [Header("UpgradeModel")]
    [SerializeField] int maxLvls;
    [SerializeField] int lvl;
    [SerializeField] int basePrice;
    [SerializeField] float pricePower;
    [SerializeField] string upgradeName;


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


    List<GameObject> upgradePoints;



    [SerializeField] Color defaultColor = Color.white;
    [SerializeField] Color selectColor = Color.yellow;


    Transform t;
    Vector3 initialPosition;
    Vector3 initialScale;

    [SerializeField]
    SpriteRenderer outlineSr;
    [SerializeField]
    TMPro.TMP_Text titleTe;
    [SerializeField]
    TMPro.TMP_Text priceTe;


    float[] omega = new float[8];
    float[] phi = new float[8];
    float[] alpha = new float[8];


    void Start()
    {
        t = transform;
        initialPosition = t.position;
        initialScale = t.localScale;
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
        //Figure out the levels
        lvl = PlayerPrefs.HasKey(upgradeName) ? PlayerPrefs.GetInt(upgradeName) : 0;
        //Draw updrades
        upgradePoints = MaximUtils.DrawCenteredListHor(upgradePointPrefab, upgradePointContainer.transform, Vector3.zero, updradePointDistance, maxLvls, 0.1f);
        ColorUpgradePoints();
        priceTe.text = CalculateCurrentPrice().ToString();
    }

    int CalculateCurrentPrice()
    {
        int powerPart = lvl > 0 ? (int)Mathf.Pow(pricePower, lvl) : 0;
        return basePrice + powerPart;
    }

    void ColorUpgradePoints()
    {
        // Not optimal but ok as there should not be a lot of upgrades
        for (int i = 0; i < lvl; ++i)
        {
            upgradePoints[i].GetComponent<SpriteRenderer>().color = selectColor;
        }
        for (int i = lvl; i < maxLvls; ++i)
        {
            upgradePoints[i].GetComponent<SpriteRenderer>().color = defaultColor;
        }
    }

    void SelectNode()
    {
        if (lvl >= maxLvls)
        {
            t.DOKill();
            Sequence seq = DOTween.Sequence();
            seq.Append(t.DORotate(new Vector3(0, 0, 5), 0.04f));
            seq.Append(t.DORotate(new Vector3(0, 0, -5), 0.08f));
            seq.Append(t.DORotate(new Vector3(0, 0, 0), 0.04f));
            seq.SetLoops(2);
            seq.OnKill(()=> { t.rotation = Quaternion.identity; });
            return;
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
        purchaseButton.SetActive(true);
    }

    void DeselectNode()
    {
        Meta.inst.currentNode = null;
        t.DOKill();
        t.DOScale(1f*initialScale, 0.15f);
        outlineSr.color = defaultColor;
        titleTe.color = defaultColor;
        purchaseButton.SetActive(false);
    }

    public void AquireUpgade()
    {
        if(!MetaEconomy.inst.UpdateBones(-CalculateCurrentPrice()))
        {
            Debug.Log("Upgrade not equired");
            return;
        }

        Debug.Log("Upgrade Aquired!");
        ++lvl;
        PlayerPrefs.SetInt(upgradeName, lvl);
        if (lvl == maxLvls)
        {
            // Fully upgraded
            purchaseButton.SetActive(false);

            // TODO: inf symbol later
            priceTe.text = "inf";

            DeselectNode();

            outlineSr.color = selectColor;
            titleTe.color = selectColor;
        }
        else
        {
            priceTe.text = CalculateCurrentPrice().ToString();
        }
        ColorUpgradePoints();
    }

    public void EnterPurchaseButton() 
    {
        purchaseButton.GetComponent<SpriteRenderer>().color = selectColor;
    }
    
    public void ExitPurchaseButton() 
    {
        purchaseButton.GetComponent<SpriteRenderer>().color = defaultColor;
    }
}
