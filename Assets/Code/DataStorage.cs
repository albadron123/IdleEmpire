using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using System.IO;
using System;

[System.Serializable]
public struct BuildingData
{
    public string title;
    public string description;
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

[System.Serializable]
public struct UpgradeData
{
    public string title;
    public Sprite icon;
    public int maxLvls;
    public int[] pricePerLevel;

    public bool isBuildingUpdrade;
    public Building.BuildingType? buildingHandle;
};

public enum EnemyHandle
{
    TinyBlob,
    NormalBlob,

    Archer,
    Summmoner,
    Boss,

    EnemyHut,

    Count
};

[System.Serializable]
public struct EnemyData
{
    public int bonesReward;
    public int bonesRewardCritial;

    public int maxHealth;

    public int simpleDamage;
};

public class DataStorage : MonoBehaviour
{
    [System.Serializable]
    private class SerializeData
    {
        public BuildingData[] allBuildings;
        public UpgradeData[] allUpgrades;
        public EnemyData[] allEnemies;
        public int[] bobbyDmgPerLevel;
        public float[] bobbyVelocityPerLevel;
        public int[] clickBaseAttackPerLevel;
        public float[] clickAttackRadiusPerLevel;
        public int[] clickHealAmountPerLevel;
        public float[] clickHealRadiusPerLevel;
        public float[] cuboSpawnTimePerLevel;
        public float[] bubilSpawnTimePerLevel;
        public float[] bonesMultiplierPerLevel;
        public float[] bombRadiusPerLevel;
        public int[] bombDamagePerLevel;
        //public int[] hutsPerLevel;
    }

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
    public static EnemyData[] allEnemies;

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


    public static void SerializeAll()
    {
        SerializeData data = new SerializeData();
        data.allBuildings = allBuildings;
        data.allEnemies = allEnemies;
        data.allUpgrades = allUpgrades;
        data.bobbyDmgPerLevel = bobbyDmgPerLevel;
        data.bobbyVelocityPerLevel = bobbyVelocityPerLevel;
        data.clickBaseAttackPerLevel = clickBaseAttackPerLevel;
        data.clickAttackRadiusPerLevel = clickAttackRadiusPerLevel;
        data.clickHealAmountPerLevel = clickHealAmountPerLevel;
        data.clickHealRadiusPerLevel = clickHealRadiusPerLevel;
        data.cuboSpawnTimePerLevel = cuboSpawnTimePerLevel;
        data.bubilSpawnTimePerLevel = bubilSpawnTimePerLevel;
        data.bonesMultiplierPerLevel = bonesMultiplierPerLevel;
        data.bombRadiusPerLevel = bombRadiusPerLevel;
        data.bombDamagePerLevel = bombDamagePerLevel;
        string dataStr = JsonUtility.ToJson(data, true);
        File.WriteAllText($"{Application.dataPath}/balance.json", dataStr);
    }

    public static bool DeserializeAll()
    {
        SerializeData data = null;
        if (!File.Exists($"{Application.dataPath}/balance.json"))
        {
            Debug.Log("balance.json not found!");
            return false;
        }
        string dataStr = File.ReadAllText($"{Application.dataPath}/balance.json");
        if (string.IsNullOrEmpty(dataStr))
        {
            Debug.Log("Json string is empty");
            return false;
        }
        try
        {
            data = JsonUtility.FromJson<SerializeData>(dataStr);
        }
        catch (Exception e)
        {
            Debug.Log("Deserialization failed");
            return false;
        }
        if (data == null)
        {
            Debug.Log("Deserialized object is null");
            return false;
        }
        allBuildings = data.allBuildings;
        allUpgrades = data.allUpgrades;
        allEnemies = data.allEnemies;
        bobbyDmgPerLevel = data.bobbyDmgPerLevel;
        bobbyVelocityPerLevel = data.bobbyVelocityPerLevel;
        clickBaseAttackPerLevel = data.clickBaseAttackPerLevel;
        clickAttackRadiusPerLevel = data.clickAttackRadiusPerLevel;
        clickHealAmountPerLevel = data.clickHealAmountPerLevel;
        clickHealRadiusPerLevel = data.clickHealRadiusPerLevel;
        cuboSpawnTimePerLevel = data.cuboSpawnTimePerLevel;
        bubilSpawnTimePerLevel = data.bubilSpawnTimePerLevel;
        bonesMultiplierPerLevel = data.bonesMultiplierPerLevel;
        bombRadiusPerLevel = data.bombRadiusPerLevel;
        bombDamagePerLevel = data.bombDamagePerLevel;
        return true;
    }

    public static void LoadBuildings()
    {
        allBuildings = new BuildingData[(int)Building.BuildingType.Count];

        List<Sprite> sprites = (Resources.LoadAll<Sprite>("Art/Icons")).ToList<Sprite>();
        allBuildings[(int)Building.BuildingType.Tawa] = new BuildingData() {
            title = "Tawa",
            description = $"<align=\"center\"><b>Tawa</b></align>\nShoots bullets in 4 directions. Change direction by clicking on <sprite=0>.",
            icon = sprites[(int)Building.BuildingType.Tawa],
            maxLevels = 3,
            initialPricePerLevel = new int[3] { 5, 10, 15 },
            priceMultiplierPerLevel = new float[3] { 2, 2, 2 },
            maxHealthPerLevel = new int[3] { 100, 150, 200},

            pfbs = new GameObject[] {
                Resources.Load<GameObject>("Prefabs/Buildings/Tawa_1"),
                Resources.Load<GameObject>("Prefabs/Buildings/Tawa_2"),
                Resources.Load<GameObject>("Prefabs/Buildings/Tawa_3"),
            }
        };
        allBuildings[(int)Building.BuildingType.CuboProduction] = new BuildingData() { 
            title = "Cubo",
            description = $"<align=\"center\"><b>Cubo</b></align>\nProduces <sprite=4> that it used to build new <i>marvels</i>.",
            icon = sprites[(int)Building.BuildingType.CuboProduction],
            maxLevels = 3,
            initialPricePerLevel = new int[3] { 5, 10, 15 },
            priceMultiplierPerLevel = new float[3] { 2, 2, 2 },
            maxHealthPerLevel = new int[3] { 100, 150, 200 },

            pfbs = new GameObject[] {
                Resources.Load<GameObject>("Prefabs/Buildings/Cubo_1"),
                Resources.Load<GameObject>("Prefabs/Buildings/Cubo_2"),
                Resources.Load<GameObject>("Prefabs/Buildings/Cubo_3"),
            }
        };
        allBuildings[(int)Building.BuildingType.BubilProduction] = new BuildingData() { 
            title = "Bubil",
            description = $"<align=\"center\"><b>Bubil</b></align>\nProduces <sprite=1> that it used to create new <i>heroes</i>.",
            icon = sprites[(int)Building.BuildingType.BubilProduction],
            maxLevels = 3,
            initialPricePerLevel = new int[3] { 5, 10, 15 },
            priceMultiplierPerLevel = new float[3] { 2, 2, 2 },
            maxHealthPerLevel = new int[3] { 100, 150, 200 },

            pfbs = new GameObject[] {
                Resources.Load<GameObject>("Prefabs/Buildings/Bubil_1"),
                Resources.Load<GameObject>("Prefabs/Buildings/Bubil_2"),
                Resources.Load<GameObject>("Prefabs/Buildings/Bubil_3"),
            }
        };
        allBuildings[(int)Building.BuildingType.HutkaGrande] = new BuildingData() { 
            title = "Hutka Grande",
            description = $"<align=\"center\"><b>Hutka Grande</b></align>\nYou live in here, protect it.",
            icon = sprites[(int)Building.BuildingType.HutkaGrande],
            maxLevels = 1,
            initialPricePerLevel = new int[1] {15},
            priceMultiplierPerLevel = new float[1] {2},
            maxHealthPerLevel = new int[1] { 150 },

            pfbs = new GameObject[] {
                Resources.Load<GameObject>("Prefabs/Buildings/MainTower"),
            }
        };
        allBuildings[(int)Building.BuildingType.Tumbo] = new BuildingData() { 
            title = "Tumbo",
            description = $"<align=\"center\"><b>Tumbo</b></align>\nShoots bulets at the nearest enemies.",
            icon = sprites[(int)Building.BuildingType.Tumbo],
            maxLevels = 2,
            initialPricePerLevel = new int[2] { 5, 10 },
            priceMultiplierPerLevel = new float[2] {2, 2},
            maxHealthPerLevel = new int[2] { 100, 150 },

            pfbs = new GameObject[] {
                Resources.Load<GameObject>("Prefabs/Buildings/Tumbo_1"),
                Resources.Load<GameObject>("Prefabs/Buildings/Tumbo_2"),
            }
        };
        allBuildings[(int)Building.BuildingType.Flawa] = new BuildingData() { 
            title = "Flawa",
            description = $"<align=\"center\"><b>Flawa</b></align>\nShoots healing orbs in 1 of 4 directions.",
            icon = sprites[(int)Building.BuildingType.Flawa],
            maxLevels = 1,
            initialPricePerLevel = new int[1] { 15 },
            priceMultiplierPerLevel = new float[1] { 2 },
            maxHealthPerLevel = new int[1] { 100 },

            pfbs = new GameObject[] {
                Resources.Load<GameObject>("Prefabs/Buildings/Flawa_1"),
            }
        };
        allBuildings[(int)Building.BuildingType.Magno] = new BuildingData() {
            title = "Magno",
            description = $"<align=\"center\"><b>Magno</b></align>\nAttracts enemies in one of 4 directions.",
            icon = sprites[(int)Building.BuildingType.Magno],
            maxLevels = 1,
            initialPricePerLevel = new int[1] { 15 },
            priceMultiplierPerLevel = new float[1] { 2 },
            maxHealthPerLevel = new int[1] { 100 },

            pfbs = new GameObject[] {
                Resources.Load<GameObject>("Prefabs/Buildings/Magno_1"),
            }
        };
        allBuildings[(int)Building.BuildingType.Custik] = new BuildingData() { 
            title = "Custik",
            description = $"<align=\"center\"><b>Custik</b></align>\nAutogenerates <sprite=1> that it used to buy new <i>heroes</i>.",
            icon = sprites[(int)Building.BuildingType.Custik],
            maxLevels = 1,
            initialPricePerLevel = new int[1] { 15 },
            priceMultiplierPerLevel = new float[1] { 2 },
            maxHealthPerLevel = new int[1] { 100 },

            pfbs = new GameObject[] {
                Resources.Load<GameObject>("Prefabs/Buildings/Custik_1"),
            }
        };
        allBuildings[(int)Building.BuildingType.Bombo] = new BuildingData() { 
            title = "Bombo",
            description = $"<align=\"center\"><b>Bombo</b></align>\nProduces <i>bombik</i> that can be placed to destroy enemies.",
            icon = sprites[(int)Building.BuildingType.Bombo],
            maxLevels = 1,
            initialPricePerLevel = new int[1] { 15 },
            priceMultiplierPerLevel = new float[1] { 2 },
            maxHealthPerLevel = new int[1] { 100 },

            pfbs = new GameObject[] {
                Resources.Load<GameObject>("Prefabs/Buildings/Bombo_1"),
            }
        };
        allBuildings[(int)Building.BuildingType.Cacti] = new BuildingData()
        {
            title = "Cacti",
            description = $"<align=\"center\"><b>Cacti</b></align>\nIf <sprite=2> hit cacti, <sprite=2> lose health.",
            icon = sprites[(int)Building.BuildingType.Cacti],
            maxLevels = 1,
            initialPricePerLevel = new int[1] { 15 },
            priceMultiplierPerLevel = new float[1] { 2 },
            maxHealthPerLevel = new int[1] { 200 },

            pfbs = new GameObject[] {
                Resources.Load<GameObject>("Prefabs/Buildings/Cacti_1"),
            }
        };

        Debug.Log(JsonUtility.ToJson(allBuildings[0]));
    }


    public static void LoadUpgrades()
    {
        allUpgrades = new UpgradeData[(int)UpgradeHandle.Count];

        List<Sprite> sprites = (Resources.LoadAll<Sprite>("Art/UpgradeIcons")).ToList<Sprite>();

        allUpgrades[(int)UpgradeHandle.Tawa] = new UpgradeData()
        {
            title = "Tawa",
            icon = sprites[(int)UpgradeHandle.Tawa],
            maxLvls = 3,
            pricePerLevel = new int[3] { 10, 30, 400},
            isBuildingUpdrade = true,
            buildingHandle = Building.BuildingType.Tawa,
        };

        allUpgrades[(int)UpgradeHandle.Cubo] = new UpgradeData()
        {
            title = "Cubo",
            icon = sprites[(int)UpgradeHandle.Cubo],
            maxLvls = 3,
            pricePerLevel = new int[3] { 0, 5, 100},
            isBuildingUpdrade = true,
            buildingHandle = Building.BuildingType.CuboProduction,
        };

        allUpgrades[(int)UpgradeHandle.Bubil] = new UpgradeData()
        {
            title = "Bubil",
            icon = sprites[(int)UpgradeHandle.Bubil],
            maxLvls = 3,
            pricePerLevel = new int[3] { 0, 5, 100},
            isBuildingUpdrade = true,
            buildingHandle = Building.BuildingType.BubilProduction,
        };

        allUpgrades[(int)UpgradeHandle.HutkaGrande] = new UpgradeData()
        {
            title = "Hutka Grande",
            icon = sprites[(int)UpgradeHandle.HutkaGrande],
            maxLvls = 1,
            pricePerLevel = new int[1] { 10 },
            isBuildingUpdrade = true,
            buildingHandle = Building.BuildingType.HutkaGrande,
        };

        allUpgrades[(int)UpgradeHandle.Tumbo] = new UpgradeData()
        {
            title = "Tumbo",
            icon = sprites[(int)UpgradeHandle.Tumbo],
            maxLvls = 2,
            pricePerLevel = new int[2] { 0, 5},
            isBuildingUpdrade = true,
            buildingHandle = Building.BuildingType.Tumbo,
        };

        allUpgrades[(int)UpgradeHandle.Flawa] = new UpgradeData()
        {
            title = "Flawa",
            icon = sprites[(int)UpgradeHandle.Flawa],
            maxLvls = 1,
            pricePerLevel = new int[1] { 750 },
            isBuildingUpdrade = true,
            buildingHandle = Building.BuildingType.Flawa,
        };

        allUpgrades[(int)UpgradeHandle.Magno] = new UpgradeData()
        {
            title = "Magno",
            icon = sprites[(int)UpgradeHandle.Magno],
            maxLvls = 1,
            pricePerLevel = new int[1] { 1000 },
            isBuildingUpdrade = true,
            buildingHandle = Building.BuildingType.Magno,
        };

        allUpgrades[(int)UpgradeHandle.Custik] = new UpgradeData()
        {
            title = "Custik",
            icon = sprites[(int)UpgradeHandle.Custik],
            maxLvls = 1,
            pricePerLevel = new int[1] { 100 },
            isBuildingUpdrade = true,
            buildingHandle = Building.BuildingType.Custik,
        };

        allUpgrades[(int)UpgradeHandle.Bombo] = new UpgradeData()
        {
            title = "Bombo",
            icon = sprites[(int)UpgradeHandle.Bombo],
            maxLvls = 1,
            pricePerLevel = new int[1] { 1000 },
            isBuildingUpdrade = true,
            buildingHandle = Building.BuildingType.Bombo,
        };

        allUpgrades[(int)UpgradeHandle.Cacti] = new UpgradeData()
        {
            title = "Cacti",
            icon = sprites[(int)UpgradeHandle.Cacti],
            maxLvls = 1,
            pricePerLevel = new int[1] { 1000 },
            isBuildingUpdrade = true,
            buildingHandle = Building.BuildingType.Cacti,
        };


        /// ============EFFECT UPGRADES=========
        /// ============EFFECT UPGRADES=========
        /// ============EFFECT UPGRADES=========
        /// ============EFFECT UPGRADES=========

        
        allUpgrades[(int)UpgradeHandle.AutoResourceGather] = new UpgradeData()
        {
            title = "Gather Resources without clicking",
            icon = sprites[(int)UpgradeHandle.AutoResourceGather],
            maxLvls = 1,
            pricePerLevel = new int[1] { 20000 },
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

        allUpgrades[(int)UpgradeHandle.BobbyAttackDamage] = new UpgradeData()
        {
            title = "Bobby killin damaga",
            icon = sprites[(int)UpgradeHandle.BobbyAttackDamage],
            maxLvls = 5,
            pricePerLevel = new int[5] {10, 20, 100, 500, 10000},
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

        bobbyDmgPerLevel = new int[6] {1, 3, 10, 20, 60, 100};

        allUpgrades[(int)UpgradeHandle.BobbyMovementVelocity] = new UpgradeData()
        {
            title = "Bobby spiddin",
            icon = sprites[(int)UpgradeHandle.BobbyMovementVelocity],
            maxLvls = 3,
            pricePerLevel = new int[3] { 20, 40, 500 },
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

        bobbyVelocityPerLevel = new float[4] {2, 3f, 4.5f, 5.5f };

        allUpgrades[(int)UpgradeHandle.SpawnMoreCubo] = new UpgradeData()
        {
            title = "Spawn More Cubo",
            icon = sprites[(int)UpgradeHandle.SpawnMoreCubo],
            maxLvls = 4,
            pricePerLevel = new int[4] { 10, 20, 40, 100 },
            isBuildingUpdrade = false,
            buildingHandle = null,
        };
        
        cuboSpawnTimePerLevel = new float[5] { 7f, 5f, 3f, 1.5f, 0.5f };

        allUpgrades[(int)UpgradeHandle.SpawnMoreBubil] = new UpgradeData()
        {
            title = "Spawn More Bubil",
            icon = sprites[(int)UpgradeHandle.SpawnMoreBubil],
            maxLvls = 4,
            pricePerLevel = new int[4] { 10, 20, 40, 100 },
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

        bubilSpawnTimePerLevel = new float[5] { 7f, 5f, 3f, 1.5f, 0.5f };

        allUpgrades[(int)UpgradeHandle.MoreBones] = new UpgradeData()
        {
            title = "More Bones",
            icon = sprites[(int)UpgradeHandle.MoreBones],
            maxLvls = 4,
            pricePerLevel = new int[4] { 10, 20, 30, 40 },
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

        //IMPORTANT: if we have something without a level, then we use [0]
        bonesMultiplierPerLevel = new float[5] { 1, 2, 3, 5, 10 };

        allUpgrades[(int)UpgradeHandle.ClickDamage] = new UpgradeData()
        {
            title = "Click Attack Dmg",
            icon = sprites[(int)UpgradeHandle.ClickDamage],
            maxLvls = 5,
            pricePerLevel = new int[5] { 10, 25, 500, 20000, 100000},
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

        clickBaseAttackPerLevel = new int[6] { 0, 1, 2, 5, 10, 20};

        allUpgrades[(int)UpgradeHandle.ClickAttackRange] = new UpgradeData()
        {
            title = "Click Attack Range",
            icon = sprites[(int)UpgradeHandle.ClickAttackRange],
            maxLvls = 5,
            pricePerLevel = new int[5] { 20, 150, 7000, 15000, 50000},
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

        clickAttackRadiusPerLevel = new float[6] { 0.15f, 0.3f, 0.45f, 0.6f, 0.75f, 1f};


        allUpgrades[(int)UpgradeHandle.ClickHeal] = new UpgradeData()
        {
            title = "Click Heal Amount",
            icon = sprites[(int)UpgradeHandle.ClickHeal],
            maxLvls = 5,
            pricePerLevel = new int[5] { 2000, 5000, 20000, 50000, 100000},
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

        clickHealAmountPerLevel = new int[6] {0, 1, 2, 5, 10, 20 };

        allUpgrades[(int)UpgradeHandle.ClickHealRange] = new UpgradeData()
        {
            title = "Click Heal Range",
            icon = sprites[(int)UpgradeHandle.ClickHealRange],
            maxLvls = 5,
            pricePerLevel = new int[5] { 3000, 7000, 40000, 80000, 150000 },
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

        clickHealRadiusPerLevel = new float[6] {0.15f, 0.5f, 0.7f, 0.9f, 1.25f, 2f };

        allUpgrades[(int)UpgradeHandle.BombikDamage] = new UpgradeData()
        {
            title = "+ Bombik Dmg",
            icon = sprites[(int)UpgradeHandle.BombikDamage],
            maxLvls = 3,
            pricePerLevel = new int[3] {1000, 5000, 30000},
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

        bombDamagePerLevel = new int[4] {0, 20, 50, 100};

        allUpgrades[(int)UpgradeHandle.BombikRange] = new UpgradeData()
        {
            title = "+ Bombik Range",
            icon = sprites[(int)UpgradeHandle.BombikRange],
            maxLvls = 3,
            pricePerLevel = new int[3] { 1000, 10000, 100000},
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

        bombRadiusPerLevel = new float[4] {0.3f, 1, 1.5f, 2};

        
        allUpgrades[(int)UpgradeHandle.MoreSlots] = new UpgradeData()
        {
            title = "More building slots",
            icon = sprites[(int)UpgradeHandle.MoreSlots],
            maxLvls = 3,
            pricePerLevel = new int[3] { 25, 2500, 25000},
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

    }

    public static void LoadEnemies()
    {
        allEnemies = new EnemyData[(int)EnemyHandle.Count];

        allEnemies[(int)EnemyHandle.TinyBlob] = new EnemyData() 
        {
            bonesReward = 1, 
            bonesRewardCritial = 2, 
            maxHealth = 10, 
            simpleDamage = 1,
        };
        allEnemies[(int)EnemyHandle.NormalBlob] = new EnemyData()
        {
            bonesReward = 2,
            bonesRewardCritial = 3,
            maxHealth = 25,
            simpleDamage = 2,
        };
        allEnemies[(int)EnemyHandle.Archer] = new EnemyData()
        {
            bonesReward = 2,
            bonesRewardCritial = 3,
            maxHealth = 20,
            simpleDamage = 3,
        };
        allEnemies[(int)EnemyHandle.Summmoner] = new EnemyData()
        {
            bonesReward = 3,
            bonesRewardCritial = 5,
            maxHealth = 40,
            simpleDamage = 0,
        };
        allEnemies[(int)EnemyHandle.Boss] = new EnemyData() 
        {
            bonesReward = 100,
            bonesRewardCritial = 100,
            maxHealth = 200,
            simpleDamage = 5,
        };
        allEnemies[(int)EnemyHandle.EnemyHut] = new EnemyData()
        {
            bonesReward = 10,
            bonesRewardCritial = 15,
            maxHealth = 10,
            simpleDamage = 0,
        };
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
