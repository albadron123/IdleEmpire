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

    SpawnMoreCubo,
    SpawnMoreBubil,
    MoreBones,
    
    ClickDamage,

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

    public static DataStorage inst = null;

    public static BuildingData[] allBuildings;
    public static UpgradeData[] allUpgrades;

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

        allUpgrades[(int)UpgradeHandle.SpawnMoreCubo] = new UpgradeData()
        {
            title = "Spawn More Cubo",
            maxLvls = 1,
            pricePerLevel = new int[1] { 10 },
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

        allUpgrades[(int)UpgradeHandle.SpawnMoreBubil] = new UpgradeData()
        {
            title = "Spawn More Bubil",
            maxLvls = 1,
            pricePerLevel = new int[1] { 10 },
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

        allUpgrades[(int)UpgradeHandle.MoreBones] = new UpgradeData()
        {
            title = "More Bones",
            maxLvls = 1,
            pricePerLevel = new int[1] { 10 },
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

        allUpgrades[(int)UpgradeHandle.ClickDamage] = new UpgradeData()
        {
            title = "Click Damage",
            maxLvls = 1,
            pricePerLevel = new int[1] { 10 },
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

        allUpgrades[(int)UpgradeHandle.BombikDamage] = new UpgradeData()
        {
            title = "+ Bombik Dmg",
            maxLvls = 1,
            pricePerLevel = new int[1] { 10 },
            isBuildingUpdrade = false,
            buildingHandle = null,
        };

        allUpgrades[(int)UpgradeHandle.BombikRange] = new UpgradeData()
        {
            title = "+ Bombik Range",
            maxLvls = 1,
            pricePerLevel = new int[1] { 10 },
            isBuildingUpdrade = false,
            buildingHandle = null,
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
