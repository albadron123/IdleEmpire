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
        Buffo,
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
    public static string TAG_BUILDING_AREA = "BuildingArea";


    public static string BUBIL_ICON_STR = "<sprite=1>";
    public static string CUBO_ICON_STR = "<sprite=4>";
    public static string BONES_ICON_STR = "<sprite=3>";
    public static string BLOB_ICON_STR = "<sprite=0>";
    public static string ENEMY_ICON_STR = "<sprite=1";

    public static Color BLOBPLACE_DEFAULT_COLOR = new Color(0.27f, 0.71f, 0.79f);
    public static Color YELLOW_COLOR = new Color(0.99f, 0.73f, 0);
    public static Color CREAMY_YELLOW_COLOR = new Color(1f, 0.8f, 0.39f);


    // === EVENTS SECTION

    [HideInInspector] public System.Action OnBlobAquired = null;
    [HideInInspector] public System.Action[] OnResourceChanged = new System.Action[(int)Resource.ResourceType.Count];

    public List<Building> allBuidlings = new List<Building>();

    public Resource[] allResources;
    public float runStartedAt = 0;

    [SerializeField] BuildingObject mainTower;
    public List<BuildingObject> builtObjects;

    public static CoreGame inst;


    public DragObject draggedObject = null;

    public GameObject sliderPfb;
    public GameObject moreResourcePfb;
    public GameObject ruinPfb;
    public GameObject fakeHutkaPfb;
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
    [SerializeField] Rect playableRect;

    float attackTimer = 0;
    float attackCount = 0;
    float attackTimeScale = 1;

    [SerializeField] TMPro.TMP_Text attackTe;
    [SerializeField] GameObject attackTePanel;
    Vector3 attackTePanelInitialPosition;
    [SerializeField] TMPro.TMP_Text attantionTe;

    [Header("Stuff")]
    public Material allWhiteMaterial;
    public Material spriteDefaultMaterial;
    [SerializeField] SpriteRenderer overlaySr;
    [Header("Projectiles")]
    public GameObject projectilePfb;
    public GameObject healingProjectilePfb;
    public GameObject arrowProjectilePfb;
    public GameObject bombPfb;
    public GameObject projectileDeathPlacePfb;


    //[SerializeField] TMPro.TMP_Text personPriceTe;
    [SerializeField] GameObject[] blobPfbs;

    [SerializeField] GameObject clickableBlockPfb;
    [SerializeField] GameObject clickableBlobPfb;


    [Header("Building Upgrades")]
    [SerializeField] GameObject upgradeGroup;
    [SerializeField] TMPro.TMP_Text upgradeGroupTe;
    [SerializeField] GameObject upgradeButton;
    [SerializeField] GameObject boostButton;
    [SerializeField] SpriteRenderer upgradeButtonSr;
    [SerializeField] SpriteRenderer boostButtonSr;
    [SerializeField] TMPro.TMP_Text upgradeButtonTe;
    [SerializeField] TMPro.TMP_Text boostButtonTe;
    [SerializeField] TMPro.TMP_Text upgradePrice;
    [SerializeField] TMPro.TMP_Text boostPrice;


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

    [Header("VFX")]
    [SerializeField] ParticleSystem contactEffect;
    public ParticleSystem upgradeEffectPfb;
    ParticleSystem[] contactEffectsPool = new ParticleSystem[10];


    [Header("Day-night cycle")]
    [SerializeField]
    SpriteRenderer dayNightBaseColorSr;
    [SerializeField]
    SpriteRenderer dayNightOutlinedSr;
    [SerializeField]
    SpriteRenderer lightingsSr;

    [Header("Level-patterns")]
    [SerializeField] List<GameObject> grassPatterns;
    [SerializeField] List<Transform> towerpoints;




    //===========RUN CONFIG PARAMS==============
    public static float bonesBonusMultiplier;

    public static float bubilGenerationTime;
    public static float cuboGenerationTime;

    public static int clickDamage;
    public static float clickAttackRadius;

    public static int clickHeal;
    public static float clickHealRadius;

    public static bool autoGatherResource;

    public static int karlDmg;
    public static float karlVelocity;

    public static int joniTechnique;
    public static float joniVelocity;

    public static float bobbyMultiplier;

    public static float bombRadius;
    public static int bombDamage;

    Sequence weatherTween = null;

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

        karlDmg = DataStorage.karlDamagePerLevel[G.GetUpgradeLvl(UpgradeHandle.AttackerBlobDmg)];
        karlVelocity = DataStorage.karlVelocityPerLevel[G.GetUpgradeLvl(UpgradeHandle.AttackerBlobVelocity)];

        joniTechnique = DataStorage.joniTechniquePerLevel[G.GetUpgradeLvl(UpgradeHandle.CollectorBlobTechnique)];
        joniVelocity = DataStorage.joniVelocityPerLevel[G.GetUpgradeLvl(UpgradeHandle.CollectorBlobVelocity)];

        bobbyMultiplier = DataStorage.basicBlobMultiplierPerLevel[G.GetUpgradeLvl(UpgradeHandle.BasicBlobMultiplier)];

        bombRadius = DataStorage.bombRadiusPerLevel[G.GetUpgradeLvl(UpgradeHandle.BombikRange)];
        bombDamage = DataStorage.bombDamagePerLevel[G.GetUpgradeLvl(UpgradeHandle.BombikDamage)];

        autoGatherResource = (G.GetUpgradeLvl(UpgradeHandle.AutoResourceGather) > 0);
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


    private void InitLevelWithGrassPattern()
    {
        int patternId = Random.Range(0, grassPatterns.Count);
        for (int i = 0; i < grassPatterns.Count; ++i)
        {
            grassPatterns[i].SetActive(false);
        }
        grassPatterns[patternId].SetActive(true);
        mainTower.transform.position = new Vector3(towerpoints[patternId].position.x, towerpoints[patternId].position.y, towerpoints[patternId].position.y);
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

        attackTePanelInitialPosition = attackTePanel.transform.position;
    }

    

    List<GameObject> easyEnemyGroups = new List<GameObject>();
    List<GameObject> mediumEnemyGroups = new List<GameObject>();
    List<GameObject> easyEnemyGroupsResource = new List<GameObject>();
    List<GameObject> mediumEnemyGroupsResource = new List<GameObject>();
    [SerializeField] GameObject bossGroup;
    [SerializeField] GameObject enemyHutPfb;
    [SerializeField] GameObject enemyHutResourcePfb;
    [SerializeField] GameObject shootingTowerPfb;
    [SerializeField] GameObject enemyTowerPfb;
    [SerializeField] List<GameObject> specialPlacings;


    void InitEnemyGroups()
    {
        easyEnemyGroups = new List<GameObject>();
        easyEnemyGroups.AddRange(Resources.LoadAll<GameObject>("Prefabs/Groups/Easy"));
        mediumEnemyGroups = new List<GameObject>();
        mediumEnemyGroups.AddRange(easyEnemyGroups);
        mediumEnemyGroups.AddRange(Resources.LoadAll<GameObject>("Prefabs/Groups/Medium"));

        easyEnemyGroupsResource = new List<GameObject>();
        easyEnemyGroupsResource.AddRange(Resources.LoadAll<GameObject>("Prefabs/Groups/EasyResource"));
        mediumEnemyGroupsResource = new List<GameObject>();
        mediumEnemyGroupsResource.AddRange(easyEnemyGroupsResource);
        mediumEnemyGroupsResource.AddRange(Resources.LoadAll<GameObject>("Prefabs/Groups/MediumResource"));

        specialPlacings = new List<GameObject>();
        specialPlacings.AddRange(Resources.LoadAll<GameObject>("Prefabs/Groups/Special"));
    }



    void Start()
    {
        InitEnemyGroups();
        InitLevelWithGrassPattern();
        InitRunFromUpgrades();
        G.InitBuildingStates();
        G.InitBlobStates();

        StartCoroutine(WaitForAttack());
        StartCoroutine(ResourceGenerationLogic(Resource.ResourceType.Cubo));
        StartCoroutine(ResourceGenerationLogic(Resource.ResourceType.Bubil));

        InitializeBuildingButtons();

        specialLrs = MaximUtils.CreateLineRendererBatch("_CIRCLE LR (generated)_", 17, specialLrColor, specialLrMaterial, specialLrThikness);
        specialLrs2 = MaximUtils.CreateLineRendererBatch("_CIRCLE LR 2 (generated)_", 17, specialLrColor, specialLrMaterial, specialLrThikness);


        CreatePopupPool();
        CreateEffectPool(contactEffect, contactEffectsPool);

        upgradeGroup.SetActive(false);
        upgradeGroup.SetActive(false);
        overlaySr.gameObject.SetActive(true);
        overlaySr.DOFade(0, 1);

        runStartedAt = Time.time;
    }

    IEnumerator DayFading(int dayTime)
    {
        weatherTween?.Kill(true);
        weatherTween = null;
        yield return dayNightBaseColorSr.DOFade(0, dayTime / 2f).WaitForCompletion();
        yield return dayNightBaseColorSr.DOFade(0.1f, dayTime / 2f).WaitForCompletion();
    }

    IEnumerator TransitionToNight()
    {
        weatherTween?.Kill(true);
        weatherTween = DOTween.Sequence()
            .Append(dayNightBaseColorSr.DOFade(0.5f, 1f))
            .Append(lightingsSr.material.DOFade(0.1f, 0))
            .Append(dayNightOutlinedSr.material.DOFade(0.88f, 1.5f))
            .Join(dayNightBaseColorSr.DOFade(0.5f, 1.5f));
        yield return weatherTween.WaitForCompletion();
    }

    IEnumerator TransitionToDay()
    {
        weatherTween?.Kill(true);
        weatherTween = DOTween.Sequence()
            .Append(lightingsSr.material.DOFade(0, 0))
            .Append(dayNightBaseColorSr.DOFade(0.2f, 1.5f))
            .Join(dayNightOutlinedSr.material.DOFade(0f, 1.5f))
            .Append(dayNightBaseColorSr.DOFade(0.1f, 1f));
        yield return weatherTween.WaitForCompletion();
    }

    IEnumerator WaitForAttack()
    {

        StartCoroutine(TransitionToDay());
        
        attackTimer = 30;
        attackTe.text = $"Next attack in {attackTimer} seconds";
        attackTePanel.transform.position = attackTePanelInitialPosition + Vector3.up * 2;
        attackTePanel.transform.DOMove(attackTePanelInitialPosition, 0.8f);

        StartCoroutine(DayFading(30));
        do
        {
            attackTe.text = $"Next attack in {attackTimer} seconds";
            yield return new WaitForSeconds(1f / attackTimeScale);
            --attackTimer;
        } while (attackTimer >= 0);

        yield return attackTePanel.transform.DOMove(attackTePanelInitialPosition+2*Vector3.up, 0.5f).WaitForCompletion();

        StartCoroutine(StartAttack());
    }

    IEnumerator StartAttack()
    {
        attackTe.text = "";

        int groupsCount = Mathf.Clamp(Mathf.CeilToInt(Mathf.Pow(1.7f, attackCount)), 1, 100);
        float waitingTime = Mathf.Clamp(0.5f - 0.06f * attackCount, 0.1f, 0.5f);
        List<GameObject> correctPool = (attackCount > 2) ? mediumEnemyGroups : easyEnemyGroups;
        List<GameObject> correctPoolResources = (attackCount > 2) ? mediumEnemyGroupsResource : easyEnemyGroupsResource;
        attackTePanel.transform.DOMove(attackTePanelInitialPosition, 0.4f);
        // boss 
        if (attackCount == 5)
        {
            DOTween.Sequence()
                .AppendCallback(()=>StartCoroutine(MaximUtils.AppearAndClearWavyText(attackTe, "THE BLOB-BOSS COMES!", 0.05f, 1, 0.5f)))
                .AppendInterval(1.75f)
                .Append(attackTePanel.transform.DOMove(attackTePanelInitialPosition+2*Vector3.up, 0.4f));

            float angle = 0;
            Instantiate(bossGroup,
                mainTower.transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * 6f,
                Quaternion.identity);
        }
        else
        {
            DOTween.Sequence()
                .AppendCallback(() => StartCoroutine(MaximUtils.AppearAndClearWavyText(attackTe, $"NIGHT {attackCount + 1} STARTS!", 0.05f, 1, 0.5f)))
                .AppendInterval(1.75f)
                .Append(attackTePanel.transform.DOMove(attackTePanelInitialPosition + 2 * Vector3.up, 0.4f));
        }

        yield return StartCoroutine(TransitionToNight());

        // attempt to place a special group
        float specialGroupChange = 1;
        
        if (Random.value <= specialGroupChange)
        {
            GameObject specialGroup = specialPlacings[Random.Range(0, specialPlacings.Count)];
            int specialEnemyGroupSize = specialGroup.GetComponent<SpecialEnemyGroup>().groupSize;
            
            if(specialEnemyGroupSize <= groupsCount)
            {
                if (CheckIfGroupCanBePlaced(specialGroup))
                { 
                    GameObject instGroup = Instantiate(specialGroup, specialGroup.transform.position, Quaternion.identity);
                    InitializeGroupWithEnemies(instGroup, correctPool, correctPoolResources);
                    groupsCount -= specialEnemyGroupSize;
                }
            }
        }

        // fill with other groups
        List<GameObject> buildingOptions = new List<GameObject> 
        {
            enemyHutPfb,
            enemyHutResourcePfb,
            enemyTowerPfb,
            shootingTowerPfb
        };
        List<float> chances = new List<float>
        {
            0.3f, 
            0.3f,
            0.2f,
            0.2f
        };
        for (int i = 0; i < groupsCount; ++i)
        {
            GameObject prefabToPlace;
            if (i == 0 && attackCount == 0)
            {
                prefabToPlace = enemyHutPfb;
            }
            else
            {
                prefabToPlace = buildingOptions[MaximUtils.RandomNonUniforIndex(chances)];
            }
            
            GameObject enemyHutInstance = Instantiate(prefabToPlace, new Vector3(1000,1000), Quaternion.identity);

            InitializeEnemyBuilding(enemyHutInstance.GetComponent<EnemyHut>(), correctPool, correctPoolResources);

            Transform enemyHutT = enemyHutInstance.transform;
            BoxCollider2D enemyHutCol = enemyHutInstance.GetComponent<BoxCollider2D>();

            Vector2 randomPos = GetPositionToPlaceHut(enemyHutCol);
            enemyHutT.position = new Vector3(randomPos.x, randomPos.y, randomPos.y);

            yield return new WaitForSeconds(waitingTime);
        }
        ++attackCount;
        StartCoroutine(PrepareTheNextAttack());
    }

    private void InitializeGroupWithEnemies(GameObject group, List<GameObject> pool, List<GameObject> poolResources)
    {
        EnemyHut[] huts = group.GetComponentsInChildren<EnemyHut>();
        foreach (var hut in huts)
        {
            InitializeEnemyBuilding(hut, pool, poolResources);
        }
    }

    private void InitializeEnemyBuilding(EnemyHut hut, List<GameObject> pool, List<GameObject> resourcePool)
    {
        if (hut.type == EnemyBuildingType.Tower && attackCount < 3)
        {
            //will use the default enemy group
            return;
        }
        switch (hut.spawnerType)
        {
            case EnemyBuildingSpawnerType.Classic:
                hut.enemyGroup = pool[Random.Range(0, pool.Count)];
                return;
            case EnemyBuildingSpawnerType.Resource:
                hut.enemyGroup = resourcePool[Random.Range(0, resourcePool.Count)];
                return;
            case EnemyBuildingSpawnerType.Mixed:
                if (Random.value > 0.5f)
                {
                    hut.enemyGroup = pool[Random.Range(0, pool.Count)];
                }
                else
                {
                    hut.enemyGroup = resourcePool[Random.Range(0, resourcePool.Count)];
                }
                return;
        }
    }

    private bool CheckIfGroupCanBePlaced(GameObject specialGroup)
    {
        Collider2D[] cols = specialGroup.GetComponentsInChildren<Collider2D>();
        foreach (var col in cols)
        {
            if (!CheckIfEnemyBuildingCanBePlaced(col))
            {
                return false;
            }
        }
        return true;
    }

    private bool CheckIfEnemyBuildingCanBePlaced(Collider2D col)
    {
        Vector2 position = col.transform.position;
        return !MaximUtils.DoIOverlapAndMatch(col, x => (x.CompareTag(TAG_BUILDING_AREA) || (x.CompareTag(TAG_ENEMY) && x != col))) && playableRect.Contains(position);
    }

    private bool CheckIfEnemyBuildingCanBePlaced(Vector2 position, float radius)
    {
        return !MaximUtils.CircleOverlapAndMatch(position, radius, x => (x.CompareTag(TAG_BUILDING_AREA) || (x.CompareTag(TAG_ENEMY)))) && playableRect.Contains(position);
    }

    private Vector2 GetPositionToPlaceHut(BoxCollider2D enemyHutCol)
    {
        Vector2 randomPos;
        bool cantBePlaced;
        int attempts = 0;
        do
        {
            randomPos = new Vector2(Random.Range(playableRect.x, playableRect.x + playableRect.width),
                                    Random.Range(playableRect.y, playableRect.y + playableRect.height));
            cantBePlaced = !CheckIfEnemyBuildingCanBePlaced(randomPos, 0.25f);
            ++attempts;
        } while (cantBePlaced && attempts < 500);
        return randomPos;
    }

    private void OnDrawGizmos()
    {
        //draw outer radius
        Vector3[] PlayableField = new Vector3[4];
        PlayableField[0] = new Vector3(playableRect.x, playableRect.y, -9);
        PlayableField[1] = new Vector3(playableRect.x + playableRect.width, playableRect.y, -9);
        PlayableField[2] = new Vector3(playableRect.x + playableRect.width, playableRect.y + playableRect.height, -9);
        PlayableField[3] = new Vector3(playableRect.x, playableRect.y + playableRect.height, -9);
        Gizmos.DrawLineStrip(PlayableField,
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
            buyBuildingButtonPlaceholders[i].SetActive(true);
        }
    }

    public void PressBuildingButton(BuildingButton bb)
    {
        int currentPrice = DataStorage.CalculateBuildingPrice(bb.type);
        if (currentPrice <= allResources[(int)Resource.ResourceType.Cubo].value)
        {
            StartBuilding(bb);
        }
        else
        {
            bb.PerformCancelAction();
        }
    }





    public void PressBuyPersonButton(BlobButton bb)
    {
        int blobPrice = DataStorage.CalculateBlobPrice();
        if (blobPrice <= allResources[(int)Resource.ResourceType.Bubil].value)
        {
            // Pay for the person
            ChangeResource(Resource.ResourceType.Bubil, -blobPrice);
            //Create person
            Instantiate(blobPfbs[(int)bb.handle],
                mainTower.transform.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0).normalized *
                Random.Range(-1, 1),
                Quaternion.identity);
            //Inflate price
            ++G.blobPurchasedCount;

            OnBlobAquired?.Invoke();
            //personPriceTe.text = BLOB_ICON_STR + ((int)blobPrice).ToString();
        }
        else
        {
            bb.PerformCancelAction();
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
        return !MaximUtils.DoIOverlapTag2D(col, TAG_BUILDING_PLACEMENT) && MaximUtils.DoIOverlapTag2D(col, TAG_BUILDING_AREA);
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

        if (Input.GetMouseButtonDown(0) && !MaximUtils.DoSquareOverlapAny(mousePosition, new Vector2(0.05f, 0.05f)))
        {
            DeselectBuilding();
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
        OnResourceChanged[(int)type]?.Invoke();
    }

    struct PooledPopup
    {
        public Transform t;
        public TMPro.TMP_Text te;
        public SpriteRenderer sr;
        public Sequence seq;
    };
    const int POOL_CAPACITY = 100;
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
            current.seq = null;
            popupPool[i] = current;
        }
        nextPoolId = POOL_CAPACITY - 1;
    }



    public void CreateIconPopUp(Vector2 initialPosition, string text, Sprite icon, float fading = 1.5f)
    {
        if (popupPool[nextPoolId].seq != null && popupPool[nextPoolId].seq.active)
        {
            DOTween.Kill(popupPool[nextPoolId].seq, true);
        }
        popupPool[nextPoolId].t.gameObject.SetActive(true);

        popupPool[nextPoolId].t.position = (Vector3)initialPosition + new Vector3(-1.2f, 0.7f, -9);
        popupPool[nextPoolId].te.text = text;
        popupPool[nextPoolId].sr.sprite = icon;
        popupPool[nextPoolId].sr.color = Color.white;
        popupPool[nextPoolId].te.color = Color.white;


        int id = nextPoolId;
        popupPool[id].seq = 
            DOTween.Sequence()
            .Append(popupPool[id].t.DOJump(popupPool[id].t.position + new Vector3(Random.Range(-0.4f, 0.4f), 0.15f, 0), Random.Range(0.3f, 0.6f), 1, fading * 0.66f))
            .Join(popupPool[id].t.DOScale(0.12f, fading * 0.66f))
            .Join(
                DOTween.Sequence()
                .AppendInterval(0.5f * fading)
                .Join(popupPool[id].sr.DOFade(0, 0.5f * fading).SetEase(Ease.InCirc))
                .Join(popupPool[id].te.DOFade(0, 0.5f * fading).SetEase(Ease.InCirc))
            )
            .Join(popupPool[id].t.DOMoveZ(1, fading))
            /*
            .JoinCallback(() => popupPool[id].t.gameObject.SetActive(false))*/
            .OnComplete(() => popupPool[id].t.gameObject.SetActive(false))
            .OnKill(() => popupPool[id].t.gameObject.SetActive(false));
        //.SetRecyclable(true);
        nextPoolId = (nextPoolId + 1) % POOL_CAPACITY;
    }

    // --- Upgrades sections --- 
    public void DecorateBuildingAsSelected(GameObject b)
    {
        selectedBuilding.sr.material.SetFloat("_Strength", 0.0035f);
        selectedBuilding.sr.material.SetColor("_Color", YELLOW_COLOR);

        foreach (var blobPlace in selectedBuilding.blobPlaces)
        {
            if (blobPlace != null)
            {
                blobPlace.GetComponent<SpriteRenderer>().color = YELLOW_COLOR;
            }
        }
    }

    public void SelectBuilding(Building.BuildingType type, int currentLvl, bool isBoosted, BuildingObject buildingToUpgrade)
    {
        DeselectBuilding();


        upgradeGroup.transform.DOKill();
        DOTween.Sequence()
            .Append(upgradeGroup.transform.DOMoveY(-8, 0))
            .Append(upgradeGroup.transform.DOMoveY(-4.5f, 0.45f).SetEase(Ease.OutCubic));

        // Outlining the building (cosmetic styling)
        selectedBuilding = buildingToUpgrade;
        DecorateBuildingAsSelected(buildingToUpgrade.gameObject);

        // Showing UI
        upgradeGroup.SetActive(true);

        OnResourceChanged[(int)Resource.ResourceType.Cubo] += ColorUpgradeButton;
        OnResourceChanged[(int)Resource.ResourceType.Bubil] += ColorBoostButton;

        ViewSelectedTitle();
        ViewUpgradeButton();
        ViewBoostButton(isBoosted);
    }

    private void ColorUpgradeButton()
    {
        upgradeButtonSr.color = (selectedBuilding != null && BuidingCanBeUpgraded(selectedBuilding.b) && BuildingUpgradeCanBePurchased(selectedBuilding.b)) ? YELLOW_COLOR : Color.white;
    }

    private void ColorBoostButton()
    {
    }

    private void ViewUpgradeButton()
    {
        if (BuidingCanBeUpgraded(selectedBuilding.b))
        {
            upgradeButtonTe.text = $"Upgrade";
            upgradePrice.text = $"{DataStorage.CalculateBuildingPrice(selectedBuilding.b.myType, selectedBuilding.b.myLvl + 1)} <size=45>{CUBO_ICON_STR}</size>";
        }
        else
        {
            upgradeButtonTe.text = $"Max level";
            upgradePrice.text = "";
        }
        ColorUpgradeButton();
    }

    private void ViewSelectedTitle() 
    {
        upgradeGroupTe.text = $"{DataStorage.allBuildings[(int)selectedBuilding.b.myType].title}\n<size=30>lvl {selectedBuilding.b.myLvl + 1}</size>";
    }

    private void ViewBoostButton(bool isBoosed)
    {

        if (isBoosed)
        {
            boostButtonTe.text = "Boosted (00:10)";
            boostPrice.text = "";
        }
        else
        {
            boostButtonTe.text = "Boost";
            boostPrice.text = $"10 <size=65>{BUBIL_ICON_STR}</size>";
        }
    }

    public void DeselectBuilding()
    {

        if (selectedBuilding != null)
        {
            selectedBuilding.sr.material.SetFloat("_Strength", 0f);
            selectedBuilding.sr.material.SetColor("_Color", new Color(0, 0, 0, 0));
            foreach (var blobPlace in selectedBuilding.blobPlaces)
            {
                blobPlace.GetComponent<SpriteRenderer>().color = BLOBPLACE_DEFAULT_COLOR;
            }
            selectedBuilding = null;
        }

        OnResourceChanged[(int)Resource.ResourceType.Cubo] -= ColorUpgradeButton;
        OnResourceChanged[(int)Resource.ResourceType.Bubil] -= ColorBoostButton;
        upgradeGroup.SetActive(false);
    }

    public bool BuidingCanBeUpgraded(Building b)
    {
        return G.buildingStates[(int)b.myType].upgradeLvlUnlocked > b.myLvl;
    }

    public bool BuildingUpgradeCanBePurchased(Building b)
    {
        return DataStorage.CalculateBuildingPrice(b.myType, b.myLvl + 1) <= allResources[(int)Resource.ResourceType.Cubo].value;
    }

    public void UpgradeSelectedBuilding(Interactable i)
    {
        
        if (!BuidingCanBeUpgraded(selectedBuilding.b) || !BuildingUpgradeCanBePurchased(selectedBuilding.b))
        {
            i.PerformCancelAction();
            return;
        }
        // Pay & Inflate the prices
        ChangeResource(Resource.ResourceType.Cubo, -DataStorage.CalculateBuildingPrice(selectedBuilding.b.myType, selectedBuilding.b.myLvl + 1));
        G.buildingStates[(int)selectedBuilding.b.myType].purchasedCount[selectedBuilding.b.myLvl+1]++;

        GameObject newPrefab = DataStorage.allBuildings[(int)selectedBuilding.b.myType].pfbs[selectedBuilding.b.myLvl+1];
        selectedBuilding = selectedBuilding.UpgradeInto(newPrefab);

        // * vfx  & sfx


        // Update UI
        ViewSelectedTitle();
        ViewUpgradeButton();
    }


    public void EndRun()
    {
        int bones = allResources[2].value;

        G.SaveRunInfo(bones, (int)(Time.time - runStartedAt));

        StartCoroutine(EndRunCoroutine());

        //SceneManager.LoadScene("End");
    }

    
    IEnumerator EndRunCoroutine()
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(G.SCENE_INTERMEDIATE, LoadSceneMode.Additive);
        ao.allowSceneActivation = false;

        Camera.main.DOShakePosition(3, 1, 50, 90, false);
        yield return new WaitForSeconds(1);
        Debug.Log(ao.progress);
        

        while (ao.progress < 0.9f)
            yield return null;

        overlaySr.DOFade(1, 1);

        yield return new WaitForSeconds(1);

        ao.allowSceneActivation = true;

        yield return ao;

        Scene oldScene = SceneManager.GetActiveScene();

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(G.SCENE_INTERMEDIATE));

        // Unload old scene asynchronously (no freeze)
        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(oldScene);

        // Optional: Force garbage collection after unload
        System.GC.Collect();

    }


    private int contactEffectId = 0;
    private int upgradeEffectId = 0;
    private void CreateEffectPool(ParticleSystem effect, ParticleSystem[] storage)
    {
        for (int i = 0; i < storage.Length; ++i)
        {
            storage[i] = Instantiate(effect, new Vector3(100, 100), Quaternion.identity);
        }
    }

    public void PlayContactEffect(Vector3 pos)
    {
        PlayEffect(pos, contactEffectsPool, ref contactEffectId);
    }

    private void PlayEffect(Vector3 pos, ParticleSystem[] pool, ref int effectId)
    {
        if (pool[effectId] != null)
        {
            pool[effectId].gameObject.transform.position = pos;
            pool[effectId].Play();
            effectId = (effectId + 1) % pool.Length;
        }
    }

    public void ShootProjectile(GameObject projectilePfb, Vector3 projectilePosition, Vector3 direction, float destroyTime, Quaternion rotation, int damage, float projectileSize, bool doAffectBlobs = true)
    {

        SoundManager.inst.PlaySfx(DataStorage.SFX_SHOOT, minPitch: 0.95f, maxPitch: 1.05f);
        GameObject inst = Instantiate(projectilePfb, projectilePosition, rotation);
        //inst.transform.localScale = new Vector3(projectileSize, projectileSize, 1);
        Projectile pr = inst.GetComponent<Projectile>();
        pr.damage = damage;
        pr.ignoreList.Add(gameObject);
        pr.size = projectileSize;
        pr.doAffectBlobs = doAffectBlobs;
        pr.direction = direction;

        pr.StartCoroutine(pr.ProjectileLifeCycle(destroyTime));
    }
}
