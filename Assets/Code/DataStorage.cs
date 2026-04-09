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


public class DataStorage : MonoBehaviour
{

    [HideInInspector]
    public AudioClip SFX_PRODUCE_CUBO;
    [HideInInspector]
    public AudioClip SFX_PRODUCE_BUBIL;
    [HideInInspector]
    public AudioClip SFX_SHOOT;

    public static DataStorage inst = null;

    public static BuildingData[] allBuildings;

    void Start()
    {
        if (inst != null)
        {
            Debug.LogError("More then 1 DataStorage on the scene");
            Destroy(gameObject);
        }
        inst = this;

        LoadSound();
        LoadBuildings();
    }

    void LoadSound()
    {
        SFX_PRODUCE_BUBIL = Resources.Load<AudioClip>("Sound/SFX/ProduceBubil");
        SFX_PRODUCE_CUBO = Resources.Load<AudioClip>("Sound/SFX/ProduceCubo");
        SFX_SHOOT = Resources.Load<AudioClip>("Sound/SFX/Shoot");
    }

    void LoadBuildings()
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


    
    void Update()
    {
        
    }

    //=== HERE WE HAVE FORMULAS FOR OUR BALANCE COMPUTATIONS

    public static int CalculateBuildingPrice(Building.BuildingType type)
    {
        int currentLvl = G.buildingStates[(int)type].currentLvl;
        int purchased = G.buildingStates[(int)type].purchasedCount[currentLvl];
        return (int)(allBuildings[(int)type].initialPricePerLevel[currentLvl] * Mathf.Pow(allBuildings[(int)type].priceMultiplierPerLevel[currentLvl], purchased));
    }
}
