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
        Cubo,
        Bubil,
        Bones,
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

    float specialLrThikness = 0.1f;
    Color specialLrColor = Color.white;
    public Material specialLrMaterial;
    LineRenderer[] specialLrs;
    public LineRenderer[] specialLrs2;



    public BuildingObject selectedBuilding = null;


    public bool canDrag = true;
    public bool canBuild = true;

    GameObject currentlyPlacingBuilding = null;
    BuildingButton currentlyBuildingButton = null;
    LineRenderer currentlyPlacingLr;
    Vector2 currentlyPlacingSize;
    Vector2 currentlyPlacingOffset;

    Vector2 mousePosition;

    [Header("Attack params")]
    [SerializeField] Rect outerAttackRect;
    [SerializeField] Rect innerAttackRect;

    float attackTimer = 0;
    float attackCount = 0;
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
    float personPrice = 5;
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
    [SerializeField] Sprite swordCursorSpr;
    [SerializeField] Sprite wandCursorSpr;

    public enum FunctionalCursor { Basic, Sward, Wand, Count };

    public FunctionalCursor currentCursor;

    int unlockedCursorCount;

    [SerializeField] GameObject[] cursorButtons = new GameObject[3];

    [SerializeField] Color selectColor;


    [Header("Left-Side UI")]
    [SerializeField] List<GameObject> buyBuildingButtons;
    [SerializeField] List<GameObject> buyBuildingButtonPlaceholders;


    //===========RUN CONFIG PARAMS==============
    public static float bonesBonusMultiplier;

    public static float bubilGenerationTime;
    public static float cuboGenerationTime;

    public static int clickDamage;
    public static float clickAttackRadius;

    public static int clickHeal;
    public static float clickHealRadius;

    public static bool autoGatherResource;

    public static int bobbyDmg;
    public static float bobbyVelocity;

    public static float bombRadius;
    public static int bombDamage;

    void InitRunFromUpgrades()
    {
        InitCursorButtons();

        bonesBonusMultiplier = DataStorage.bonesMultiplierPerLevel[G.GetUpgradeLvl(UpgradeHandle.MoreBones)];

        cuboGenerationTime = DataStorage.cuboSpawnTimePerLevel[G.GetUpgradeLvl(UpgradeHandle.SpawnMoreCubo)];
        bubilGenerationTime = DataStorage.bubilSpawnTimePerLevel[G.GetUpgradeLvl(UpgradeHandle.SpawnMoreBubil)];

        clickDamage = DataStorage.clickBaseAttackPerLevel[G.GetUpgradeLvl(UpgradeHandle.ClickDamage)];
        clickAttackRadius = DataStorage.clickAttackRadiusPerLevel[G.GetUpgradeLvl(UpgradeHandle.ClickAttackRange)];

        clickHeal = DataStorage.clickHealAmountPerLevel[G.GetUpgradeLvl(UpgradeHandle.ClickHeal)];
        clickHealRadius = DataStorage.clickHealRadiusPerLevel[G.GetUpgradeLvl(UpgradeHandle.ClickHealRange)];

        bobbyDmg = DataStorage.bobbyDmgPerLevel[G.GetUpgradeLvl(UpgradeHandle.BobbyAttackDamage)];
        bobbyVelocity = DataStorage.bobbyVelocityPerLevel[G.GetUpgradeLvl(UpgradeHandle.BobbyMovementVelocity)];

        bombRadius = DataStorage.bombRadiusPerLevel[G.GetUpgradeLvl(UpgradeHandle.BombikRange)];
        bombDamage = DataStorage.bombDamagePerLevel[G.GetUpgradeLvl(UpgradeHandle.BombikDamage)];

        autoGatherResource = (G.GetUpgradeLvl(UpgradeHandle.AutoResourceGather) > 0);

        //REMOVE THIS LATER
        //BOBBY BUY PRICE
        personPrice = 5;
        personPriceTe.text = personPrice.ToString();
    }

    void InitCursorButtons()
    {
        bool hasAttackCursor = false;
        bool hasHealCursor = false;

        if (G.upgradeStates[(int)UpgradeHandle.ClickDamage].upgradeLvl > 0)
        {
            hasAttackCursor = true;
        }
        if (G.upgradeStates[(int)UpgradeHandle.ClickHeal].upgradeLvl > 0)
        {
            hasHealCursor = true;
        }

        currentCursor = FunctionalCursor.Basic;

        //Disable Cursor buttons for now
        for (int i = 0; i < cursorButtons.Length; ++i)
        {
            cursorButtons[i].SetActive(false);
        }

        if (!hasAttackCursor)
        {
            //No cursor is unlocked, so only 1 (basic) is available
            unlockedCursorCount = 1;
            return;
        }

        if (!hasHealCursor)
        {
            unlockedCursorCount = 2;
        }
        else
        {
            unlockedCursorCount = 3;
        }

        cursorButtons[0].GetComponent<SpriteRenderer>().color = selectColor;

        for (int i = 0; i < unlockedCursorCount; ++i)
        {
            cursorButtons[i].SetActive(true);
        }
    }


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



    List<GameObject> easyEnemyGroups = new List<GameObject>();
    List<GameObject> mediumEnemyGroups = new List<GameObject>();
    [SerializeField] GameObject bossGroup;
    [SerializeField] GameObject enemyHutPfb;


    void InitEnemyGroups()
    {
        easyEnemyGroups = new List<GameObject>();
        easyEnemyGroups.AddRange(Resources.LoadAll<GameObject>("Prefabs/Groups/Easy"));
        mediumEnemyGroups = new List<GameObject>();
        mediumEnemyGroups.AddRange(easyEnemyGroups);
        mediumEnemyGroups.AddRange(Resources.LoadAll<GameObject>("Prefabs/Groups/Medium"));
    }

    void Start()
    {

        InitEnemyGroups();
        InitRunFromUpgrades();
        G.InitBuildingStates();

        StartCoroutine(WaitForAttack());
        StartCoroutine(ResourceGenerationLogic(Resource.ResourceType.Cubo));
        StartCoroutine(ResourceGenerationLogic(Resource.ResourceType.Bubil));

        InitializeBuildingButtons();

        specialLrs = MaximUtils.CreateLineRendererBatch("_CIRCLE LR (generated)_", 17, specialLrColor, specialLrMaterial, specialLrThikness);
        specialLrs2 = MaximUtils.CreateLineRendererBatch("_CIRCLE LR 2 (generated)_", 17, specialLrColor, specialLrMaterial, specialLrThikness);


        CreatePopupPool();
    }







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
        attackTe.text = "";   

        int groupsCount = Mathf.Clamp(Mathf.CeilToInt(Mathf.Pow(1.7f, attackCount)), 1, 200);
        float waitingTime = Mathf.Clamp(0.5f - 0.06f * attackCount, 0.1f, 0.5f);
        List<GameObject> correctPool = (attackCount > 3) ? mediumEnemyGroups : easyEnemyGroups;
        // boss 
        if (attackCount == 5)
        {
            StartCoroutine(MaximUtils.AppearAndClearWavyText(attantionTe, "THE BLOB-BOSS COMES!!!", 0.05f, 1, 0.5f));

            float angle = 0;
            Instantiate(bossGroup,
                mainTower.transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * 6f,
                Quaternion.identity);   
        }
        else
        {
            StartCoroutine(MaximUtils.AppearAndClearWavyText(attantionTe, $"WAVE {attackCount + 1} BEGINS!!!", 0.05f, 1, 0.5f));
        }

        
        for (int i = 0; i < groupsCount; ++i)
        {
            Vector2 randomPos = Vector2.zero;
        
            GameObject enemyHutInstance = Instantiate(enemyHutPfb, new Vector3(randomPos.x, randomPos.y, randomPos.y), Quaternion.identity);
            enemyHutInstance.GetComponent<EnemyHut>().enemyGroup = correctPool[Random.Range(0, correctPool.Count)];
        
            Transform enemyHutT = enemyHutInstance.transform;
            BoxCollider2D enemyHutCol = enemyHutInstance.GetComponent<BoxCollider2D>();

            bool overlapped = false;
            int attempts = 0;
            do
            {
                randomPos = MaximUtils.RandomPositionInsideFrame(innerAttackRect, outerAttackRect);
                overlapped = MaximUtils.DoSquareOverlapAny(randomPos - enemyHutCol.offset, enemyHutCol.size);
                ++attempts;
            } while(overlapped && attempts < 500);

            enemyHutT.position = new Vector3(randomPos.x, randomPos.y, randomPos.y);
            
            
            yield return new WaitForSeconds(waitingTime);
        }
        ++attackCount;
        StartCoroutine(PrepareTheNextAttack());
    }

    private void OnDrawGizmos()
    {
        //draw outer radius
        Vector3[] outerAttackRadius = new Vector3[4];
        outerAttackRadius[0] = new Vector3(outerAttackRect.x, outerAttackRect.y, -9);
        outerAttackRadius[1] = new Vector3(outerAttackRect.x+outerAttackRect.width, outerAttackRect.y, -9);
        outerAttackRadius[2] = new Vector3(outerAttackRect.x+outerAttackRect.width, outerAttackRect.y+outerAttackRect.height, -9);
        outerAttackRadius[3] = new Vector3(outerAttackRect.x, outerAttackRect.y+outerAttackRect.height, -9);
        Gizmos.DrawLineStrip(outerAttackRadius,
                             true);
        //draw inner radius
        Vector3[] innerAttackRadius = new Vector3[4];
        innerAttackRadius[0] = new Vector3(innerAttackRect.x, innerAttackRect.y, -9);
        innerAttackRadius[1] = new Vector3(innerAttackRect.x + innerAttackRect.width, innerAttackRect.y, -9);
        innerAttackRadius[2] = new Vector3(innerAttackRect.x + innerAttackRect.width, innerAttackRect.y + innerAttackRect.height, -9);
        innerAttackRadius[3] = new Vector3(innerAttackRect.x, innerAttackRect.y + innerAttackRect.height, -9);
        Gizmos.DrawLineStrip(innerAttackRadius,
                             true);
    }

    IEnumerator PrepareTheNextAttack()
    {
        const int EXTRA_TIME_FOR_DEFENCE_SEC = 30;
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

    IEnumerator ResourceGenerationLogic(Resource.ResourceType res)
    {
        float baseGenerationTime = (res == Resource.ResourceType.Cubo) ? cuboGenerationTime : bubilGenerationTime;
        GameObject genPrefab = (res == Resource.ResourceType.Cubo) ? clickableBlockPfb : clickableBlobPfb;
        while (true)
        {
            //Step 1: Wait
            yield return new WaitForSeconds(baseGenerationTime + Random.Range(-baseGenerationTime / 3f, baseGenerationTime / 3f));
            //Step 2: Generate resource in free place
            GameObject resInst = Instantiate(genPrefab, new Vector3(Random.Range(-5f, 8f), Random.Range(-4.5f, 4.5f), 50),
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
        if (currentPrice <= allResources[(int)Resource.ResourceType.Cubo].value)
        {
            StartBuilding(bb);
        }
    }





    public void PressBuyPersonButton()
    {
        if ((int)personPrice <= allResources[(int)Resource.ResourceType.Bubil].value)
        {
            // Pay for the person
            ChangeResource(Resource.ResourceType.Bubil, -(int)personPrice);
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
        CoreGame.inst.SetCurrentCursor((int)FunctionalCursor.Basic);

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
            ChangeResource(Resource.ResourceType.Cubo, -DataStorage.CalculateBuildingPrice(currentlyBuildingButton.type));
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


    public void SetCurrentCursor(int fc)
    {
        currentCursor = (FunctionalCursor)fc;
        switch (currentCursor)
        {
            case FunctionalCursor.Basic:
                G.SetCursor(basicCursorSpr);
                break;
            case FunctionalCursor.Sward:
                G.SetCursor(swordCursorSpr);
                break;
            case FunctionalCursor.Wand:
                G.SetCursor(wandCursorSpr);
                break;
        }
        for (int i = 0; i < cursorButtons.Length; ++i)
        {
            if (i != (int)currentCursor)
            {
                cursorButtons[i].GetComponent<SpriteRenderer>().color = Color.white;
            }
            else
            {
                cursorButtons[i].GetComponent<SpriteRenderer>().color = selectColor;
            }
        }
    }

    void Update()
    {
        mousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (draggedObject == null && currentlyBuildingButton == null)
        {
            if (Input.mouseScrollDelta.y < 0)
            {
                SetCurrentCursor(((int)currentCursor + 1 + unlockedCursorCount) % unlockedCursorCount);
            }
            if (Input.mouseScrollDelta.y > 0)
            {
                SetCurrentCursor(((int)currentCursor - 1 + unlockedCursorCount) % unlockedCursorCount);
            }
        }

        if (currentCursor == FunctionalCursor.Basic)
        {
            specialLrs[0].transform.parent.gameObject.SetActive(false);

            if (draggedObject != null)
            {
                G.SetCursor(handCursorSpr);
                draggedObject.transform.position = new Vector3(mousePosition.x, mousePosition.y, -5);
            }
            else
            {
                G.SetCursor(basicCursorSpr);
            }
        }
        else
        {
            // visualize cursor range (radius)
            float radius = 5;
            if (currentCursor == FunctionalCursor.Sward)
            {
                radius = clickAttackRadius;
            }
            else if (currentCursor == FunctionalCursor.Wand)
            {
                radius = clickHealRadius;
            }
            specialLrs[0].transform.parent.gameObject.SetActive(true);
            MaximUtils.RenderDashedCircle(specialLrs, mousePosition, radius, Time.time, 16);
        }

        if (currentlyPlacingBuilding != null)
        {
            currentlyPlacingBuilding.transform.position = new Vector3(mousePosition.x, mousePosition.y, -5);
            DrawBuildingRect(currentlyPlacingBuilding.transform.position + 10 * Vector3.forward, currentlyPlacingOffset, currentlyPlacingSize);
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

        if (currentlyPlacingBuilding == null && draggedObject == null)
        {


            if (Input.GetMouseButtonDown(0))
            {
                // attack enemies
                if (currentCursor == FunctionalCursor.Sward)
                {
                    List<Collider2D> enemyCols = MaximUtils.GetAllOverlappedWithTag2D(mousePosition, clickAttackRadius, TAG_ENEMY);
                    for (int i = 0; i < enemyCols.Count; ++i)
                    {
                        enemyCols[i].GetComponent<DestructableObject>().ChangeHealth(-clickDamage);
                    }
                }

                //heal towers
                if (currentCursor == FunctionalCursor.Wand)
                {
                    List<Collider2D> buildingCols = MaximUtils.GetAllOverlappedWithTag2D(mousePosition, clickHealRadius, TAG_BUILDING);
                    for (int i = 0; i < buildingCols.Count; ++i)
                    {
                        buildingCols[i].GetComponent<DestructableObject>().ChangeHealth(clickHeal);
                    }
                }

                // gather resources
                Collider2D resourseCol = MaximUtils.GetNearestOverlappedWithTag2D(mousePosition, 0.1f, TAG_CLICKABLE_RESOURCE);
                if (resourseCol != null)
                {
                    resourseCol.GetComponent<ClickableResource>().Click();
                }
            }

            if (autoGatherResource)
            {
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
            ChangeResource(Resource.ResourceType.Cubo, 20);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            ChangeResource(Resource.ResourceType.Bubil, 20);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ChangeResource(Resource.ResourceType.Bones, 20);
        }

#endif

    }



    public void ChangeResource(Resource.ResourceType type, int delta)
    {
        allResources[(int)type].value += delta;
        allResources[(int)type].te.text = $"{allResources[(int)type].value}";
    }

    struct PooledPopup
    {
        public Transform t;
        public TMPro.TMP_Text te;
        public SpriteRenderer sr;
    };
    const int POOL_CAPACITY = 500;
    PooledPopup[] popupPool;
    int nextPoolId = 0;
    private void CreatePopupPool()
    {
        popupPool = new PooledPopup[POOL_CAPACITY];
        for (int i = 0; i < POOL_CAPACITY; ++i)
        {
            GameObject inst = Instantiate(moreResourcePfb);
            TMPro.TMP_Text te = inst.GetComponent<TMPro.TMP_Text>();
            SpriteRenderer sr = inst.transform.GetChild(0).GetComponent<SpriteRenderer>();
            inst.SetActive(false);
            PooledPopup current = new PooledPopup();
            current.t = inst.transform;
            current.te = te;
            current.sr = sr;
            popupPool[i] = current;
        }
    }



    public void CreateIconPopUp(Vector2 initialPosition, string text, Sprite icon, float fading = 1.5f, bool doPool = true)
    {
        if (doPool)
        {
            DOTween.Kill(popupPool[nextPoolId].t);
            popupPool[nextPoolId].t.gameObject.SetActive(true);

            popupPool[nextPoolId].t.position = (Vector3)initialPosition + new Vector3(-1.2f, 0.7f, -9);
            popupPool[nextPoolId].te.text = text;
            popupPool[nextPoolId].sr.sprite = icon;
            popupPool[nextPoolId].sr.color = Color.white;
            popupPool[nextPoolId].te.color = Color.white;

            DOTween.Sequence()
               .Append(popupPool[nextPoolId].t.DOJump(popupPool[nextPoolId].t.position + new Vector3(Random.Range(-0.4f, 0.4f), 0.15f, 0), Random.Range(0.3f, 0.6f), 1, fading * 0.66f))
               .Join(popupPool[nextPoolId].t.DOScale(0.12f, fading * 0.66f))
               .Join(
                   DOTween.Sequence()
                   .AppendInterval(0.5f * fading)
                   .Join(popupPool[nextPoolId].sr.DOFade(0, 0.5f * fading).SetEase(Ease.InCirc))
                   .Join(popupPool[nextPoolId].te.DOFade(0, 0.5f * fading).SetEase(Ease.InCirc))
               )
               .Join(popupPool[nextPoolId].t.DOMoveZ(1, fading))
               .JoinCallback(() => popupPool[nextPoolId].t.gameObject.SetActive(false))
               .OnComplete(() => popupPool[nextPoolId].t.gameObject.SetActive(false))
               .OnKill(() => popupPool[nextPoolId].t.gameObject.SetActive(false));
               //.SetRecyclable(true);
            nextPoolId = (nextPoolId + 1) % POOL_CAPACITY;
        }
        /*
        else
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
                .Join(inst.transform.DOMoveZ(1, fading))
                .SetRecyclable(true);

            Destroy(inst, fading);
        }
        */
        
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
