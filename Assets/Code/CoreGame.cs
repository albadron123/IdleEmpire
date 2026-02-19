using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class Building
{
    public enum BuildingType
    {
        Tower, 
        CubeProduction,
        BlahProduction,
        MajorTower,

        Count
    };
    public BuildingType myType;
    public GameObject buildingPfb;
    public int myLvl;
}

[System.Serializable]
public class Resource
{
    public enum ResourceType
    {
        cubes,
        blah,
        bones,
        Count
    }
    public Sprite icon;
    public TMPro.TMP_Text te;
    public int value;
}

[System.Serializable]
public class BuildingTag
{
    public SpriteRenderer sr;
    public int price;
    public TMPro.TMP_Text priceTe;
    public TMPro.TMP_Text titleTe;
    public Building.BuildingType type;
}

public class CoreGame : MonoBehaviour
{
    public static string TAG_BLOB_PLACE = "BlobPlace";
    public static string TAG_ENEMY = "Enemy";
    public static string TAG_PROJECTILE = "Projectile";
    public static string TAG_BUILDING_PLACEMENT = "BuildingPlacement";


    public static string[] BUILDING_NAMES = new string[(int)Building.BuildingType.Count] {"Tawa","Cubo","Bubil","Major"};

    public List<Building> allBuidlings = new List<Building>();
    public List<BuildingTag> allBuildingTags = new List<BuildingTag>();
    public Resource[] allResources;

    [SerializeField] BuildingObject mainTower;
    public List<BuildingObject> builtObjects;

    public static CoreGame inst;


    public DragObject draggedObject = null;

    public GameObject sliderPfb;
    public GameObject moreResourcePfb;


    public BuildingObject selectedBuilding = null;



    public bool canDrag = true;
    public bool canBuild = true;

    GameObject currentlyPlacingBuilding = null;
    BuildingTag currentlyBuildingTag = null;
    SpriteRenderer lastPlacementSquare = null;
    Vector2 mousePosition;

    float attackTimer = 0;
    float attackCount = 1;

    [SerializeField] TMPro.TMP_Text attackTe;

    public GameObject projectilePfb;

    //Remove later
    float personPrice = 20;
    [SerializeField] TMPro.TMP_Text personPriceTe;
    [SerializeField] GameObject blobPfb;


    List<GameObject> upgradeButtons = null;
    [SerializeField] Transform upgradesContainer;
    [SerializeField] GameObject upgradeButtonPfb;

    void Start()
    {

        if (inst != null)
        {
            Destroy(gameObject);
        }

        inst = this;

        // Show all resources
        for (int i = 0; i < (int)Resource.ResourceType.Count; ++i)
        {
            ChangeResource((Resource.ResourceType)i, 0);
        }

        builtObjects = new List<BuildingObject>();
        builtObjects.Add(mainTower);


        StartCoroutine(AttackLogic());
        StartCoroutine(ResourceGenerationLogic());
    }

    [SerializeField] List<GameObject> enemyGroups = new List<GameObject>();



    float timeScale = 1;

    IEnumerator AttackLogic()
    {
        while (true)
        {
            //plan attack    
            attackTimer = 30;
            do
            {
                attackTe.text = $"Next attack in {attackTimer} seconds";
                yield return new WaitForSeconds(1f / timeScale);
                --attackTimer;
            } while (attackTimer >= 0);

            attackTe.text = "Attack starts!";
            //Generate army
            for (int i = 0; i < attackCount; ++i)
            {
                float angle = Random.Range(0, Mathf.PI * 2);
                Instantiate(enemyGroups[Random.Range(0, enemyGroups.Count)],
                    mainTower.transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * 6f,
                    Quaternion.identity);
                yield return new WaitForSeconds(0.5f);
            }

            ++attackCount;
        }
    }

    [SerializeField] GameObject clickableBlockPfb;
    [SerializeField] GameObject clickableBlobPfb;

    IEnumerator ResourceGenerationLogic()
    {
        while (true)
        {
            //Step 1: Wait
            yield return new WaitForSeconds(Random.Range(2, 5));
            //Step 2: Generate resource in free place
            GameObject resPfb = (Random.value > 0.5f) ? clickableBlobPfb : clickableBlockPfb;
            GameObject resInst = Instantiate(resPfb, new Vector3(Random.Range(-5f, 8f), Random.Range(-4.5f, 4.5f), 50),
                Quaternion.identity);
            Vector3 resInitialScale = resInst.transform.localScale;
            resInst.transform.localScale = new Vector3(0, 0, 1);
            resInst.transform.DOScale(resInitialScale, 0.6f);
        }
    }

    public void PressBuildingButton(int btId)
    {
        BuildingTag bt = allBuildingTags[btId];
        if (bt.price <= allResources[(int)Resource.ResourceType.cubes].value)
        {
            StartBuilding(bt);
        }
    }


    public void PressBuyPersonButton()
    {
        if ((int)personPrice <= allResources[(int)Resource.ResourceType.blah].value)
        {
            // Pay for the person
            ChangeResource(Resource.ResourceType.blah, -(int)personPrice);
            //Create person
            Instantiate(blobPfb,
                mainTower.transform.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0).normalized *
                Random.Range(-1, 1),
                Quaternion.identity);
            //Inflate price
            personPrice *= 1.3f;
            personPriceTe.text = ((int)personPrice).ToString();
        }
    }

    public void StartBuilding(BuildingTag buildingTag)
    {
        canDrag = false;
        canBuild = false;

        //Instantiate building
        string buildingName = BUILDING_NAMES[(int)buildingTag.type];
        
        int buildingLevel = PlayerPrefs.HasKey(buildingName)?PlayerPrefs.GetInt(buildingName):0;

        currentlyBuildingTag = buildingTag;
        currentlyPlacingBuilding =
            Instantiate(allBuidlings.Find(x => x.myType == currentlyBuildingTag.type && x.myLvl == buildingLevel).buildingPfb,
                new Vector3(mousePosition.x, mousePosition.y, -5), Quaternion.identity);
        lastPlacementSquare = currentlyPlacingBuilding.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    public void BuildHere()
    {


        bool canBuildHere = CanBuildHere();
        if (canBuildHere)
        {
            canDrag = true;
            canBuild = true;
            // Aquire
            ChangeResource(Resource.ResourceType.cubes, -currentlyBuildingTag.price);
            // Upgrade price
            currentlyBuildingTag.price *= 2;
            currentlyBuildingTag.priceTe.text = currentlyBuildingTag.price.ToString();
            // Build
            currentlyPlacingBuilding.transform.position = new Vector3(currentlyPlacingBuilding.transform.position.x,
                currentlyPlacingBuilding.transform.position.y, currentlyPlacingBuilding.transform.position.y);

            lastPlacementSquare.gameObject.SetActive(false);

            builtObjects.Add(currentlyPlacingBuilding.GetComponent<BuildingObject>());

            currentlyBuildingTag = null;
            currentlyPlacingBuilding = null;
        }
    }

    public bool CanBuildHere()
    {
        Collider2D col = currentlyPlacingBuilding.transform.Find("BuildingCollider").GetComponent<Collider2D>();

        Vector2 colPosition = col.transform.position;
        if (colPosition.x > 8 || colPosition.x < -5 || colPosition.y > 4.5 || colPosition.y < -4.5)
        {
            return false;
        }

        return !MaximUtils.DoIOverlapTag2D(col, TAG_BUILDING_PLACEMENT);
        /*
        List<Collider2D> overlapped = new List<Collider2D>();
        Physics2D.OverlapCollider(col, new ContactFilter2D().NoFilter(), overlapped);
        
        if (overlapped.Count > 0)
        {
            return false;
        }

        return true;
        */
    }

    public void CancelBuilding()
    {
        canDrag = true;
        canBuild = true;

        Destroy(currentlyPlacingBuilding);

        currentlyBuildingTag = null;
        currentlyPlacingBuilding = null;
    }


    void Update()
    {
        Vector2 mousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (draggedObject != null)
        {
            draggedObject.transform.position = new Vector3(mousePosition.x, mousePosition.y, -5);
        }

        if (currentlyPlacingBuilding != null)
        {
            currentlyPlacingBuilding.transform.position = new Vector3(mousePosition.x, mousePosition.y, -5);
            if (CanBuildHere())
            {
                lastPlacementSquare.color = new Color(0, 1, 0, 0.2f);
            }
            else
            {
                lastPlacementSquare.color = new Color(1, 0, 0, 0.2f);
            }

            if (Input.GetMouseButtonDown(0))
            {
                BuildHere();
            }

            if (Input.GetMouseButtonDown(1))
            {
                CancelBuilding();
            }
        }

// Cheats
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Q))
        {
            timeScale = 10f;
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            timeScale = 1f;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            ChangeResource(Resource.ResourceType.cubes, 20);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            ChangeResource(Resource.ResourceType.blah, 20);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ChangeResource(Resource.ResourceType.bones, 20);
        }
        
#endif

    }



    public void ChangeResource(Resource.ResourceType type, int delta)
    {
        allResources[(int)type].value += delta;
        allResources[(int)type].te.text = $"{allResources[(int)type].value}";
    }


    public void CreateIconPopUp(Vector3 initialPosition, string text, Sprite icon)
    {

        GameObject inst = Instantiate(moreResourcePfb, initialPosition + new Vector3(-1.2f, 0.7f, 0), Quaternion.identity);
        TMPro.TMP_Text te = inst.GetComponent<TMPro.TMP_Text>();
        te.text = text;
        SpriteRenderer sr = inst.transform.GetChild(0).GetComponent<SpriteRenderer>();
        sr.sprite = icon;
        DOTween.Sequence()
            .Append(inst.transform.DOJump(inst.transform.position + new Vector3(Random.Range(-0.4f, 0.4f), 0.15f, 0), Random.Range(0.3f, 0.6f), 1, 1f))
            .Join(inst.transform.DOScale(0.12f, 1f))
            .Join(sr.DOFade(0, 1.5f))
            .Join(te.DOFade(0, 1.5f));
        Destroy(inst, 1.5f);
    }

    // --- Upgrades sections --- 


    public void BuyUpgrade(BuildingObject bo)
    {

    }

    public void ShowUpgrades(List<UpgradeType> upgradeTypes, BuildingObject buildingToUpgrade)
    {
        const float BUTTON_OFFSET = 0.1F;
        HideUpgrades();
        // Outlining the building
        selectedBuilding = buildingToUpgrade;
        selectedBuilding.outline.SetActive(true);
        // Showing buttons
        upgradeButtons = new List<GameObject>();
        for (int i = 0; i < upgradeTypes.Count; ++i)
        {
            // Setting the correct params 
            // need to do that before the creating so that the button correctly reflects its state on start
            var ub = upgradeButtonPfb.GetComponent<UpgradeButton>();
            ub.bObj = buildingToUpgrade;
            ub.myType = upgradeTypes[i];

            // Instantiating
            GameObject buttonInst = Instantiate(upgradeButtonPfb, Vector3.zero, Quaternion.identity, upgradesContainer);
            buttonInst.transform.localPosition = new Vector3(0, i * (buttonInst.transform.localScale.y + BUTTON_OFFSET), 0);
            
            upgradeButtons.Add(buttonInst);
        }
    }

    public void HideUpgrades()
    {
        if (upgradeButtons != null)
        {
            if (selectedBuilding != null)
            {
                selectedBuilding.outline.SetActive(false);
                selectedBuilding = null;
            }
            for (int i = 0; i < upgradeButtons.Count; ++i)
            {
                Destroy(upgradeButtons[i]);
            }
        }
        upgradeButtons = null;
    }



}
