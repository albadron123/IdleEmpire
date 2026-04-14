using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

[System.Serializable]
public class Building
{
    public enum BuildingType
    {
        Tawa, 
        CuboProduction,
        BubilProduction,
        HutkaGrande,
        Tumbo,
        Flawa,
        Magno,
        Custik,
        Bombo,
        Cacti,
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

public class CoreGame : MonoBehaviour
{
    public static string TAG_BLOB_PLACE = "BlobPlace";
    public static string TAG_ENEMY = "Enemy";
    public static string TAG_CLICKABLE_RESOURCE = "ClickableResource";
    public static string TAG_PROJECTILE = "Projectile";
    public static string TAG_ENEMY_PROJECTILE = "EnemyProjectile";
    public static string TAG_BUILDING_PLACEMENT = "BuildingPlacement";
    public static string TAG_BOMB = "Bomb";
    public static string TAG_BUILDING = "Building";

    //public static string[] BUILDING_NAMES = new string[(int)Building.BuildingType.Count] { "Tawa", "Cubo", "Bubil", "Major", "Tumba", "Flawa", "Magnik", "Plomo", "Bombik", "Cacti"};

    public List<Building> allBuidlings = new List<Building>();

    public Resource[] allResources;

    [SerializeField] BuildingObject mainTower;
    public List<BuildingObject> builtObjects;

    public static CoreGame inst;


    public DragObject draggedObject = null;

    public GameObject sliderPfb;
    public GameObject moreResourcePfb;
    public GameObject ruinPfb;
    public GameObject destructionEffect;

    public BuildingObject selectedBuilding = null;


    public bool canDrag = true;
    public bool canBuild = true;

    GameObject currentlyPlacingBuilding = null;
    BuildingButton currentlyBuildingButton = null;
    LineRenderer currentlyPlacingLr;
    Vector2 currentlyPlacingSize;
    Vector2 currentlyPlacingOffset;
    
    Vector2 mousePosition;

    float attackTimer = 0;
    float attackCount = 1;
    float attackTimeScale = 1;

    [SerializeField] TMPro.TMP_Text attackTe;
    [SerializeField] TMPro.TMP_Text attantionTe;

    [Header("Stuff")]
    public Material allWhiteMaterial;
    public Material spriteDefaultMaterial;
    [Header("Projectiles")]
    public GameObject projectilePfb;
    public GameObject healingProjectilePfb;
    public GameObject arrowProjectilePfb;
    public GameObject bombPfb;


    //Remove later
    float personPrice = 20;
    [SerializeField] TMPro.TMP_Text personPriceTe;
    [SerializeField] GameObject blobPfb;




    List<GameObject> upgradeButtons = null;
    [SerializeField] Transform upgradesContainer;
    [SerializeField] GameObject upgradeButtonPfb;

    [SerializeField] GameObject clickableBlockPfb;
    [SerializeField] GameObject clickableBlobPfb;

    [Header("Cursor")]
    [SerializeField] Sprite basicCursorSpr;
    [SerializeField] Sprite handCursorSpr;


    [Header("Left-Side UI")]
    [SerializeField] List<GameObject> buyBuildingButtons;
    [SerializeField] List<GameObject> buyBuildingButtonPlaceholders;





    private void Awake()
    {
        // We are booting the game here. 

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

        if (builtObjects == null)
        {
            builtObjects = new List<BuildingObject>();
        }
    }


    void Start()
    {
        G.InitBuildingStates();

        StartCoroutine(WaitForAttack());
        StartCoroutine(ResourceGenerationLogic());

        InitializeBuildingButtons();
    }

    [SerializeField] List<GameObject> enemyGroups = new List<GameObject>();

    


    IEnumerator WaitForAttack()
    {
        attackTimer = 30;
        do
        {
            attackTe.text = $"Next attack in {attackTimer} seconds";
            yield return new WaitForSeconds(1f / attackTimeScale);
            --attackTimer;
        } while (attackTimer >= 0);
        StartCoroutine(StartAttack());
    }

    IEnumerator StartAttack()
    {
        //Notify all
        attackTe.text = "";
        StartCoroutine(MaximUtils.AppearAndClearWavyText(attantionTe, "NEW WAVE BEGINS!!!", 0.05f, 1, 0.5f));

        for (int i = 0; i < attackCount; ++i)
        {
            float angle = Random.Range(0, Mathf.PI * 2);
            Instantiate(enemyGroups[Random.Range(0, enemyGroups.Count)],
                mainTower.transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * 6f,
                Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
        }
        ++attackCount;
        StartCoroutine(PrepareTheNextAttack());
    }

    IEnumerator PrepareTheNextAttack()
    {
        const int EXTRA_TIME_FOR_DEFENCE_SEC = 10;
        for (int i = 0; i < EXTRA_TIME_FOR_DEFENCE_SEC; ++i)
        {
            if (MaximUtils.CountGameObjectsWithTag(TAG_ENEMY) <= 0)
            {
                break;
            }
            yield return new WaitForSecondsRealtime(1);
        }
        StartCoroutine(WaitForAttack());
    }


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

    void InitializeBuildingButtons()
    {
        for (int i = 0; i < G.equippedBuildingsSize; ++i)
        {
            if (i < G.equippedBuildings.Count)
            {
                buyBuildingButtons[i].SetActive(true);
                buyBuildingButtons[i].GetComponent<BuildingButton>().Init(G.equippedBuildings[i]);
                buyBuildingButtonPlaceholders[i].SetActive(false);
            }
            else
            {
                buyBuildingButtons[i].SetActive(false);
                buyBuildingButtonPlaceholders[i].SetActive(true);
            }
        }
        for (int i = G.equippedBuildingsSize; i < G.equippedBuildingsCapacity; ++i)
        {
            buyBuildingButtons[i].SetActive(false);
            buyBuildingButtonPlaceholders[i].SetActive(false);
        }
    }

    public void PressBuildingButton(BuildingButton bb)
    {
        int currentPrice = DataStorage.CalculateBuildingPrice(bb.type);
        if (currentPrice <= allResources[(int)Resource.ResourceType.cubes].value)
        {
            StartBuilding(bb);
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

    public void StartBuilding(BuildingButton bb)
    {
        canDrag = false;
        canBuild = false;
        currentlyBuildingButton = bb;

        //Instantiate building

        int buildingLevel = G.buildingStates[(int)bb.type].currentLvl;

        currentlyPlacingBuilding = Instantiate(DataStorage.allBuildings[(int)bb.type].pfbs[buildingLevel],
                                               new Vector3(mousePosition.x, mousePosition.y, -5),
                                               Quaternion.identity);


        
        Transform cpT = currentlyPlacingBuilding.transform.Find("BuildingCollider");
        BoxCollider2D cpB2d = cpT.GetComponent<BoxCollider2D>();
        currentlyPlacingSize = cpB2d.size;
        currentlyPlacingOffset = (Vector2)cpT.localPosition + cpB2d.offset;
        currentlyPlacingLr = currentlyPlacingBuilding.GetComponent<LineRenderer>();
        currentlyPlacingLr.positionCount = 5;
    }


    public void DrawBuildingRect(Vector3 position, Vector2 center, Vector2 size)
    {
        currentlyPlacingLr.positionCount = 5;
        currentlyPlacingLr.SetPositions(new Vector3[] {position + (Vector3) (center + new Vector2(size.x / 2, size.y / 2)),
                                                       position + (Vector3) (center + new Vector2(-size.x / 2, size.y / 2)),
                                                       position + (Vector3) (center + new Vector2(-size.x / 2, -size.y / 2)),
                                                       position + (Vector3) (center + new Vector2(size.x / 2, -size.y / 2)),
                                                       position + (Vector3) (center + new Vector2(size.x / 2, size.y / 2))});
    }

    public void BuildHere()
    {


        bool canBuildHere = CanBuildHere();
        if (canBuildHere)
        {
            canDrag = true;
            canBuild = true;

            // Aquire
            ChangeResource(Resource.ResourceType.cubes, -DataStorage.CalculateBuildingPrice(currentlyBuildingButton.type));
            // Increase the stats of built objects
            G.buildingStates[(int)currentlyBuildingButton.type].purchasedCount[G.buildingStates[(int)currentlyBuildingButton.type].currentLvl]++;
            
            currentlyBuildingButton.UpdatePrices();
            
            // Build
            currentlyPlacingBuilding.transform.position = new Vector3(currentlyPlacingBuilding.transform.position.x,
                currentlyPlacingBuilding.transform.position.y, currentlyPlacingBuilding.transform.position.y);

            currentlyPlacingLr.positionCount = 0;
            currentlyPlacingLr = null;

            currentlyBuildingButton = null;
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

        currentlyBuildingButton = null;
        currentlyPlacingBuilding = null;
    }

    


    void Update()
    {
        mousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (draggedObject != null)
        {
            G.SetCursor(handCursorSpr);
            draggedObject.transform.position = new Vector3(mousePosition.x, mousePosition.y, -5);
        }
        else
        {
            G.SetCursor(basicCursorSpr);
        }

        if (currentlyPlacingBuilding != null)
        {
            currentlyPlacingBuilding.transform.position = new Vector3(mousePosition.x, mousePosition.y, -5);
            DrawBuildingRect(currentlyPlacingBuilding.transform.position + 10*Vector3.forward, currentlyPlacingOffset, currentlyPlacingSize);
            if (CanBuildHere())
            {
                currentlyPlacingLr.startColor = new Color(0.3f, 1, 0.3f, 1f);
                currentlyPlacingLr.endColor = new Color(0.3f, 1, 0.3f, 1f);
            }
            else
            {
                currentlyPlacingLr.startColor = new Color(1, 0.3f, 0.3f, 1f);
                currentlyPlacingLr.endColor = new Color(1, 0.3f, 0.3f, 1f);
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

        if (currentlyPlacingBuilding == null && draggedObject==null)
        {
            // perform clicking activities
            if(Input.GetMouseButtonDown(0))
            {
                Collider2D enemyCol = MaximUtils.GetNearestOverlappedWithTag2D(mousePosition, 0.1f, TAG_ENEMY);
                if (enemyCol != null)
                {
                    enemyCol.GetComponent<DestructableObject>().ChangeHealth(-1);
                }
                Collider2D resourseCol = MaximUtils.GetNearestOverlappedWithTag2D(mousePosition, 0.1f, TAG_CLICKABLE_RESOURCE);
                if (resourseCol != null)
                {
                    resourseCol.GetComponent<ClickableResource>().Click();
                }
            }
        }

// Cheats
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Q))
        {
            attackTimeScale = 10f;
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            attackTimeScale = 1f;
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


    public void CreateIconPopUp(Vector2 initialPosition, string text, Sprite icon, float fading = 1.5f)
    {

        GameObject inst = Instantiate(moreResourcePfb, (Vector3)initialPosition + new Vector3(-1.2f, 0.7f, -9), Quaternion.identity);
        TMPro.TMP_Text te = inst.GetComponent<TMPro.TMP_Text>();
        te.text = text;
        SpriteRenderer sr = inst.transform.GetChild(0).GetComponent<SpriteRenderer>();
        sr.sprite = icon;


        DOTween.Sequence()
            .Append(inst.transform.DOJump(inst.transform.position + new Vector3(Random.Range(-0.4f, 0.4f), 0.15f, 0), Random.Range(0.3f, 0.6f), 1, fading * 0.66f))
            .Join(inst.transform.DOScale(0.12f, fading * 0.66f))
            .Join(
                DOTween.Sequence()
                .AppendInterval(0.5f * fading)
                .Join(sr.DOFade(0, 0.5f * fading).SetEase(Ease.InCirc))
                .Join(te.DOFade(0, 0.5f * fading).SetEase(Ease.InCirc))
            )
            .Join(inst.transform.DOMoveZ(1, fading));
        Destroy(inst, fading);
    }

    // --- Upgrades sections --- 

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


    public void EndRun()
    {
        int lastRecord = 0;
        int bones = allResources[2].value;
        int totalBones = 0;

        if (PlayerPrefs.HasKey("recordBones"))
        {
            lastRecord = PlayerPrefs.GetInt("recordBones");
        }

        PlayerPrefs.SetInt("currentBones", bones);

        if (bones > lastRecord)
        {
            PlayerPrefs.SetInt("recordBones", bones);
        }

        if (PlayerPrefs.HasKey("totalBones"))
        {
            totalBones += PlayerPrefs.GetInt("totalBones");
        }
        totalBones += bones;
        PlayerPrefs.SetInt("totalBones", totalBones);

        PlayerPrefs.Save();

        SceneManager.LoadScene("End");
    }
}
