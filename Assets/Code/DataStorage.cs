using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[System.Serializable]
public struct BuildingData
{
    public string title;
    public Sprite icon;
    public int maxLevels;
    public int[] initialPricePerLevel;
    public float[] priceMultiplierPerLevel;
    public int[] maxHealthPerLevel;
    public GameObject[] pfbs;
};

public enum UpgradeHandle
{
    //Building upgrades
    Tawa,
    Cubo,
    Bubil,
    HutkaGrande,
    Tumbo,
    Flawa,
    Magno,
    Custik,
    Bombo,
    Cacti,

    AutoResourceGather,

    BobbyAttackDamage,
    BobbyMovementVelocity,

    SpawnMoreCubo,
    SpawnMoreBubil,
    MoreBones,
    
    ClickDamage,
    ClickAttackRange, 

    ClickHeal,
    ClickHealRange,

    BombikDamage,
    BombikRange,

    MoreSlots,

    Count
};

public struct UpgradeData
{
    public string title;
    public int maxLvls;
    public int[] pricePerLevel;

    public bool isBuildingUpdrade;
    public Building.BuildingType? buildingHandle;
};

public class DataStorage : MonoBehaviour
{

    [HideInInspector]
    public static AudioClip SFX_PRODUCE_CUBO;
    [HideInInspector]
    public static AudioClip SFX_PRODUCE_BUBIL;
    [HideInInspector]
    public static AudioClip SFX_SHOOT;
    [HideInInspector]
    public static AudioClip SFX_CLICK_UPGRADE;
    [HideInInspector]
    public static AudioClip SFX_CLICK_UPGRADE_BUTTON;
    [HideInInspector]
    public static AudioClip SFX_OVER_UPGRADE_BUTTON;


    public static AudioClip SOUND_BROWN_NOISE;
    public static AudioClip SOUND_FIREPLACE;
    public static AudioClip SOUND_FIREPLACE_MUSIC;

    public static DataStorage inst = null;

    public static BuildingData[] allBuildings;
    public static UpgradeData[] allUpgrades;

    //Special upgrade data structures
    public static int[] bobbyDmgPerLevel;
    public static float[] bobbyVelocityPerLevel;

    public static int[] clickBaseAttackPerLevel;
    public static float[] clickAttackRadiusPerLevel;

    public static int[] clickHealAmountPerLevel;
    public static float[] clickHealRadiusPerLevel;

    public static float[] cuboSpawnTimePerLevel;
    public static float[] bubilSpawnTimePerLevel;

    public static float[] bonesMultiplierPerLevel;

    public static float[] bombRadiusPerLevel;
    public static int[] bombDamagePerLevel;

    void Start()
    {
        if (inst != null)
        {
            Debug.LogError("More then 1 DataStorage on the scene");
            Destroy(gameObject);
        }
        inst = this;
    }

    public static void LoadSound()
    {
        SFX_PRODUCE_BUBIL = Resources.Load<AudioClip>("Sound/SFX/ProduceBubil");
        SFX_PRODUCE_CUBO = Resources.Load<AudioClip>("Sound/SFX/ProduceCubo");
        SFX_SHOOT = Resources.Load<AudioClip>("Sound/SFX/Shoot");
        SFX_CLICK_UPGRADE = Resources.Load<AudioClip>("Sound/SFX/ClickUpgrade");
        SFX_CLICK_UPGRADE_BUTTON = Resources.Load<AudioClip>("Sound/SFX/ClickUpgradeButton");
        SFX_OVER_UPGRADE_BUTTON = Resources.Load<AudioClip>("Sound/SFX/OverUpgradeButton");
        SOUND_BROWN_NOISE = Resources.Load<AudioClip>("Sound/BrownNoise");
        SOUND_FIREPLACE = Resources.Load<AudioClip>("Sound/Fireplace");
        SOUND_FIREPLACE_MUSIC = Resources.Load<AudioClip>("Sound/FireplaceMusic");
}

    public static void LoadBuildings()
    {
        allBuildings = new BuildingData[(int)Building.BuildingType.Count];

        List<Sprite> sprites = (Resources.LoadAll<Sprite>("Art/Icons")).ToList<Sprite>();
        allBuildings[(int)Building.BuildingType.Tawa] = new BuildingData() {
            title = "Tawa",
            icon = sprites[(int)Building.BuildingType.Tawa],
            maxLevels = 3,
            initialPricePerLevel = new int[3] { 5, 10, 15 },
            priceMultiplierPerLevel = new float[3] { 2, 2, 2 },
            maxHealthPerLevel = new int[3] { 15, 15, 15 },

            pfbs = new GameObject[] {
                Resources.Load<GameObject>("Prefabs/Buildings/Tawa_1"),
                Resources.Load<GameObject>("Prefabs/Buildings/Tawa_2"),
                Resources.Load<GameObject>("Prefabs/Buildings/Tawa_3"),
            }
        };
        allBuildings[(int)Building.BuildingType.CuboProduction] = new BuildingData() { 
            title = "Cubo",
            icon = sprites[(int)Building.BuildingType.CuboProduction],
            maxLevels = 3,
            initialPricePerLevel = new int[3] { 5, 10, 15 },
            priceMultiplierPerLevel = new float[3] { 2, 2, 2 },
            maxHealthPerLevel = new int[3] { 15, 15, 15 },

            pfbs = new GameObject[] {
                Resources.Load<GameObject>("Prefabs/Buildings/Cubo_1"),
                Resources.Load<GameObject>("Prefabs/Buildings/Cubo_2"),
                Resources.Load<GameObject>("Prefabs/Buildings/Cubo_3"),
            }
        };
        allBuildings[(int)Building.BuildingType.BubilProduction] = new BuildingData() { 
            title = "Bubil",
            icon = sprites[(int)Building.BuildingType.BubilProduction],
            maxLevels = 3,
            initialPricePerLevel = new int[3] { 5, 10, 15 },
            priceMultiplierPerLevel = new float[3] { 2, 2, 2 },
            maxHealthPerLevel = new int[3] { 15, 15, 15 },

            pfbs = new GameObject[] {
                Resources.Load<GameObject>("Prefabs/Buildings/Bubil_1"),
                Resources.Load<GameObject>("Prefabs/Buildings/Bubil_2"),
                Resources.Load<GameObject>("Prefabs/Buildings/Bubil_3"),
            }
        };
        allBuildings[(int)Building.BuildingType.HutkaGrande] = new BuildingData() { 
            title = "Hutka Grande",
            icon = sprites[(int)Building.BuildingType.HutkaGrande],
            maxLevels = 1,
            initialPricePerLevel = new int[1] {15},
            priceMultiplierPerLevel = new float[1] {2},
            maxHealthPerLevel = new int[1] { 50 },

            pfbs = new GameObject[] {
                Resources.Load<GameObject>("Prefabs/Buildings/MainTower"),
            }
        };
        allBuildings[(int)Building.BuildingType.Tumbo] = new BuildingData() { 
            title = "Tumbo",
            icon = sprites[(int)Building.BuildingType.Tumbo],
            maxLevels = 1,
            initialPricePerLevel = new int[1] { 15 },
            priceMultiplierPerLevel = new float[1] { 2 },
            maxHealthPerLevel = new int[1] { 15 },

            pfbs = new GameObject[] {
                Resources.Load<GameObject>("Prefabs/Buildings/Tumbo_1"),
            }
        };
        allBuildings[(int)Building.BuildingType.Flawa] = new BuildingData() { 
            title = "Flawa",
            icon = sprites[(int)Building.BuildingType.Flawa],
            maxLevels = 1,
            initialPricePerLevel = new int[1] { 15 },
            priceMultiplierPerLevel = new float[1] { 2 },
            maxHealthPerLevel = new int[1] { 15 },

            pfbs = new GameObject[] {
                Resources.Load<GameObject>("Prefabs/Buildings/Flawa_1"),
            }
        };
        allBuildings[(int)Building.BuildingType.Magno] = new BuildingData() {
            title = "Magno",
            icon = sprites[(int)Building.BuildingType.Magno],
            maxLevels = 1,
            initialPricePerLevel = new int[1] { 15 },
            priceMultiplierPerLevel = new float[1] { 2 },
            maxHealthPerLevel = new int[1] { 15 },

            pfbs = new GameObject[] {
                Resources.Load<GameObject>("Prefabs/Buildings/Magno_1"),
            }
        };
        allBuildings[(int)Building.BuildingType.Custik] = new BuildingData() { 
            title = "Custik",
            icon = sprites[(int)Building.BuildingType.Custik],
            maxLevels = 1,
            initialPricePerLevel = new int[1] { 15 },
            priceMultiplierPerLevel = new float[1] { 2 },
            maxHealthPerLevel = new int[1] { 15 },

            pfbs = new GameObject[] {
                Resources.Load<GameObject>("Prefabs/Buildings/Custik_1"),
            }
        };
        allBuildings[(int)Building.BuildingType.Bombo] = new BuildingData() { 
            title = "Bombo",
            icon = sprites[(int)Building.BuildingType.Bombo],
            maxLevels = 1,
            initialPricePerLevel = new int[1] { 15 },
            priceMultiplierPerLevel = new float[1] { 2 },
            maxHealthPerLevel = new int[1] { 15 },

            pfbs = new GameObject[] {
                Resources.Load<GameObject>("Prefabs/Buildings/Bombo_1"),
            }
        };
        allBuildings[(int)Building.BuildingType.Cacti] = new BuildingData() { 
            title = "Cacti",
            icon = sprites[(int)Building.BuildingType.Cacti],
            maxLevels = 1,
            initialPricePerLevel = new int[1] { 15 },
            priceMultiplierPerLevel = new float[1] { 2 },
            maxHealthPerLevel = new int[1] { 15 },

            pfbs = new GameObject[] {
                Resources.Load<GameObject>("Prefabs/Buildings/Cacti_1"),
            }
        };
    }


    public static void LoadUpgrades()
    {
        allUpgrades = new UpgradeData[(int)UpgradeHandle.Count];

        allUpgrades[(int)UpgradeHandle.Tawa] = new UpgradeData()
        {
            title = "Tawa",
            maxLvls = 3,
            pricePerLevel = new int[3] { 10, 20, 50},
            isBuildingUpdrade = true,
            buildingHandle = Building.BuildingType.Tawa,
        };

        allUpgrades[(int)UpgradeHandle.Cubo] = new UpgradeData()
        {
            title = "Cubo",
            maxLvls = 3,
            pricePerLevel = new int[3] { 10, 20, 50 },
            isBuildingUpdrade = true,
            buildingHandle = Building.BuildingType.CuboProduction,
        };

        allUpgrades[(int)UpgradeHandle.Bubil] = new UpgradeData()
        {
            title = "Bubil",
            maxLvls = 3,
            pricePerLevel = new int[3] { 10, 20, 50 },
            isBuildingUpdrade = true,
            buildingHandle = Building.BuildingType.BubilProduction,
        };

        allUpgrades[(int)UpgradeHandle.HutkaGrande] = new UpgradeData()
        {
            title = "Hutka Grande",
            maxLvls = 1,
            pricePerLevel = new int[1] { 10 },
            isBuildingUpdrade = true,
            buildingHandle = Building.BuildingType.HutkaGrande,
        };

        allUpgrades[(int)UpgradeHandle.Tumbo] = new UpgradeData()
        {
            title = "Tumbo",
            maxLvls = 1,
            pricePerLevel = new int[1] { 10 },
            isBuildingUpdrade = true,
            buildingHandle = Building.BuildingType.Tumbo,
        };

        allUpgrades[(int)UpgradeHandle.Flawa] = new UpgradeData()
        {
            title = "Flawa",
            maxLvls = 1,
            pricePerLevel = new int[1] { 10 },
            isBuildingUpdrade = true,
            buildingHandle = Building.BuildingType.Flawa,
        };

        allUpgrades[(int)UpgradeHandle.Magno] = new UpgradeData()
        {
            title = "Magno",
            maxLvls = 1,
            pricePerLevel = new int[1] { 10 },
            isBuildingUpdrade = true,
            buildingHandle = Building.BuildingType.Magno,
        };

        allUpgrades[(int)UpgradeHandle.Custik] = new UpgradeData()
        {
            title = "Custik",
            maxLvls = 1,
            pricePerLevel = new int[1] { 10 },
            isBuildingUpdrade = true,
            buildingHandle = Building.BuildingType.Custik,
        };

        allUpgrades[(int)UpgradeHandle.Bombo] = new UpgradeData()
        {
            title = "Bombo",
            maxLvls = 1,
            pricePerLevel = new int[1] { 10 },
            isBuildingUpdrade = true,
            buildingHandle = Building.BuildingType.Bombo,
        };

        allUpgrades[(int)UpgradeHandle.Cacti] = new UpgradeData()
        {
            title = "Cacti",
            maxLvls = 1,
            pricePerLevel = new int[1] { 10 },
            isBuildingUpdrade = true,
            buildingHandle = Building.BuildingType.Cacti,
        };

        
        allUpgrades[(int)UpgradeHandle.AutoResourceGather] = new UpgradeData()
        {
            title = "Gather Resources without clicking",
            maxLvls = 1,
            pricePerLevel = new int[1] { 75 },
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

        allUpgrades[(int)UpgradeHandle.BobbyAttackDamage] = new UpgradeData()
        {
            title = "Bobby killin damaga",
            maxLvls = 5,
            pricePerLevel = new int[5] {10, 20, 40, 60, 100},
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

        bobbyDmgPerLevel = new int[5] { 3, 10, 20, 60, 150};

        allUpgrades[(int)UpgradeHandle.BobbyMovementVelocity] = new UpgradeData()
        {
            title = "Bobby spiddin",
            maxLvls = 3,
            pricePerLevel = new int[3] { 20, 40, 60 },
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

        bobbyVelocityPerLevel = new float[3] { 2, 3.5f, 5.5f };

        allUpgrades[(int)UpgradeHandle.SpawnMoreCubo] = new UpgradeData()
        {
            title = "Spawn More Cubo",
            maxLvls = 4,
            pricePerLevel = new int[4] { 10, 20, 40, 100 },
            isBuildingUpdrade = false,
            buildingHandle = null,
        };
        
        cuboSpawnTimePerLevel = new float[4] {5, 4.25f, 3.75f, 2};

        allUpgrades[(int)UpgradeHandle.SpawnMoreBubil] = new UpgradeData()
        {
            title = "Spawn More Bubil",
            maxLvls = 4,
            pricePerLevel = new int[4] { 10, 20, 40, 100 },
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

        bubilSpawnTimePerLevel = new float[4] { 5, 4.25f, 3.75f, 2 };

        allUpgrades[(int)UpgradeHandle.MoreBones] = new UpgradeData()
        {
            title = "More Bones",
            maxLvls = 4,
            pricePerLevel = new int[4] { 10, 20, 30, 40 },
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

        bonesMultiplierPerLevel = new float[4] { 1, 2, 3, 5 };

        allUpgrades[(int)UpgradeHandle.ClickDamage] = new UpgradeData()
        {
            title = "Click Attack Dmg",
            maxLvls = 5,
            pricePerLevel = new int[5] { 20, 40, 60, 85 , 100},
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

        clickBaseAttackPerLevel = new int[5] { 1, 2, 5, 10, 20 };

        allUpgrades[(int)UpgradeHandle.ClickAttackRange] = new UpgradeData()
        {
            title = "Click Attack Range",
            maxLvls = 5,
            pricePerLevel = new int[5] { 20, 40, 60, 85, 100 },
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

        clickAttackRadiusPerLevel = new float[5] { 0.3f, 0.45f, 0.6f, 0.75f, 1f };


        allUpgrades[(int)UpgradeHandle.ClickHeal] = new UpgradeData()
        {
            title = "Click Heal Amount",
            maxLvls = 5,
            pricePerLevel = new int[5] { 20, 40, 60, 85, 100 },
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

        clickHealAmountPerLevel = new int[5] { 1, 2, 5, 10, 20 };

        allUpgrades[(int)UpgradeHandle.ClickHealRange] = new UpgradeData()
        {
            title = "Click Heal Range",
            maxLvls = 5,
            pricePerLevel = new int[5] { 20, 40, 60, 85, 100 },
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

        clickHealRadiusPerLevel = new float[5] { 0.5f, 0.7f, 0.9f, 1.25f, 2f };

        allUpgrades[(int)UpgradeHandle.BombikDamage] = new UpgradeData()
        {
            title = "+ Bombik Dmg",
            maxLvls = 3,
            pricePerLevel = new int[3] {50, 100, 200},
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

        bombDamagePerLevel = new int[3] {10, 30, 100};

        allUpgrades[(int)UpgradeHandle.BombikRange] = new UpgradeData()
        {
            title = "+ Bombik Range",
            maxLvls = 3,
            pricePerLevel = new int[3] { 50, 100, 200},
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

        bombRadiusPerLevel = new float[3] { 1, 1.5f, 2};
    }

    
    void Update()
    {
        
    }

    //=== HERE WE HAVE FORMULAS FOR OUR BALANCE COMPUTATIONS

    public static int CalculateBuildingPrice(Building.BuildingType type)
    {
        int currentLvl = G.buildingStates[(int)type].currentLvl;
        return CalculateBuildingPrice(type, currentLvl);
    }

    public static int CalculateBuildingPrice(Building.BuildingType type, int lvl)
    {
        int purchased = G.buildingStates[(int)type].purchasedCount[lvl];
        return (int)(allBuildings[(int)type].initialPricePerLevel[lvl] * Mathf.Pow(allBuildings[(int)type].priceMultiplierPerLevel[lvl], purchased));
    }

    public static int CalculateUpgradePrice(UpgradeHandle handle)
    {
        int lvl = G.upgradeStates[(int)handle].upgradeLvl;
        return allUpgrades[(int)handle].pricePerLevel[lvl];
    }
}
