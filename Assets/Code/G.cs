using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct BuildingState
{
    public int[] purchasedCount;
    public int currentLvl;
    public int upgradeLvlUnlocked;
}

public struct UpgradeState
{
    public int upgradeLvl;
    public bool visible;

}


public class G : MonoBehaviour
{
    public static G inst;

    // Equipped buildings
    public static List<Building.BuildingType> equippedBuildings;
    public static int equippedBuildingsSize;
    public static int equippedBuildingsCapacity;


    public static int bonesSpent;
    public static int maxBonesSpent = 100;


    public static BuildingState[] buildingStates;
    public static UpgradeState[] upgradeStates;

    static GameObject cursor;
    static SpriteRenderer cursorSr;

    void Start()
    {
        if (inst != null)
        {
            Debug.LogError("More then 1 G on the scene");
            Destroy(gameObject);
        }
        inst = this;

        LoadCursor();
    }


    void LoadCursor()
    {
        cursor = Instantiate(Resources.Load<GameObject>("Prefabs/Cursor"));
        Object.DontDestroyOnLoad(cursor);
        cursorSr = cursor.GetComponent<SpriteRenderer>();
        Cursor.visible = false;
    }

    private void OnApplicationFocus(bool focus)
    {
        Cursor.visible = false;   
    }


    public static void InitBuildingStates()
    {
        buildingStates = new BuildingState[(int)Building.BuildingType.Count];
        for (int i = 0; i < (int)Building.BuildingType.Count; ++i)
        {
            buildingStates[i] = new BuildingState { 
                purchasedCount = new int[DataStorage.allBuildings[i].maxLevels], 
                currentLvl = 0, 
                upgradeLvlUnlocked = -1,
            };
            if (PlayerPrefs.HasKey($"building_updrade_{DataStorage.allBuildings[(int)i].title}")) 
            {
                buildingStates[i].upgradeLvlUnlocked = PlayerPrefs.GetInt($"building_updrade_{DataStorage.allBuildings[(int)i].title}");
            }
        }
    }


    public static void InitUpgradeStates()
    {
        upgradeStates = new UpgradeState[(int)UpgradeHandle.Count];

        if (!PlayerPrefs.HasKey("===has upgrades==="))
        {
            //set up initial upgrades in memory
            PlayerPrefs.SetInt("===has upgrades===", 1);

            //Set reset every upgrade
            for (int upgradeHandle = 0; upgradeHandle < (int)UpgradeHandle.Count; ++upgradeHandle)
            {
                upgradeStates[upgradeHandle].visible = false;
                upgradeStates[upgradeHandle].upgradeLvl = 0;
            }

            //Set the default state correctly
            upgradeStates[(int)UpgradeHandle.Cubo].upgradeLvl = 1;
            upgradeStates[(int)UpgradeHandle.Cubo].visible = true;

            upgradeStates[(int)UpgradeHandle.Bubil].upgradeLvl = 1;
            upgradeStates[(int)UpgradeHandle.Bubil].visible = true;

            upgradeStates[(int)UpgradeHandle.Tawa].upgradeLvl = 1;
            upgradeStates[(int)UpgradeHandle.Tawa].visible = true;

            PlayerPrefs.SetInt($"{DataStorage.allUpgrades[(int)UpgradeHandle.Cubo].title}", 1);
            PlayerPrefs.SetInt($"{DataStorage.allUpgrades[(int)UpgradeHandle.Cubo].title}_visible", 1);
            
            PlayerPrefs.SetInt($"{DataStorage.allUpgrades[(int)UpgradeHandle.Bubil].title}", 1);
            PlayerPrefs.SetInt($"{DataStorage.allUpgrades[(int)UpgradeHandle.Bubil].title}_visible", 1);
            
            PlayerPrefs.SetInt($"{DataStorage.allUpgrades[(int)UpgradeHandle.Tawa].title}", 1);
            PlayerPrefs.SetInt($"{DataStorage.allUpgrades[(int)UpgradeHandle.Tawa].title}_visible", 1);
            


            buildingStates[(int)Building.BuildingType.CuboProduction].upgradeLvlUnlocked = 0;
            buildingStates[(int)Building.BuildingType.BubilProduction].upgradeLvlUnlocked = 0;
            buildingStates[(int)Building.BuildingType.Tawa].upgradeLvlUnlocked = 0;
            PlayerPrefs.SetInt($"building_updrade_{DataStorage.allBuildings[(int)Building.BuildingType.CuboProduction].title}", 0);
            PlayerPrefs.SetInt($"building_updrade_{DataStorage.allBuildings[(int)Building.BuildingType.BubilProduction].title}", 0);
            PlayerPrefs.SetInt($"building_updrade_{DataStorage.allBuildings[(int)Building.BuildingType.Tawa].title}", 0);
        }
        else
        {
            for (int upgradeHandle = 0; upgradeHandle < (int)UpgradeHandle.Count; ++upgradeHandle)
            {
                if (PlayerPrefs.HasKey($"{DataStorage.allUpgrades[upgradeHandle].title}_visible"))
                {
                    upgradeStates[upgradeHandle].visible = true;
                }
                if (PlayerPrefs.HasKey(DataStorage.allUpgrades[upgradeHandle].title))
                {
                    upgradeStates[upgradeHandle].upgradeLvl = PlayerPrefs.GetInt(DataStorage.allUpgrades[upgradeHandle].title);
                }
            }
            
        }
    }


    public static void AquireUpgrade(UpgradeHandle h, List<UpgradeHandle> connectedUpgrades)
    {
        for (int i = 0; i < connectedUpgrades.Count; ++i)
        {
            upgradeStates[(int)connectedUpgrades[i]].visible = true;
            PlayerPrefs.SetInt($"{DataStorage.allUpgrades[(int)connectedUpgrades[i]].title}_visible", 1);
        }
        ++upgradeStates[(int)h].upgradeLvl;
        PlayerPrefs.SetInt($"{DataStorage.allUpgrades[(int)h].title}", upgradeStates[(int)h].upgradeLvl);
        if (DataStorage.allUpgrades[(int)h].isBuildingUpdrade)
        {
            //aquire building upgrade :^)
            Building.BuildingType buildingHandle = DataStorage.allUpgrades[(int)h].buildingHandle.Value;
            ++buildingStates[(int)buildingHandle].upgradeLvlUnlocked;
            PlayerPrefs.SetInt($"building_updrade_{DataStorage.allBuildings[(int)buildingHandle].title}",
                               buildingStates[(int)buildingHandle].upgradeLvlUnlocked);
        }
    }

    public static void UpgradeEquipBuildingsSize()
    {
        ++equippedBuildingsSize;
        PlayerPrefs.SetInt("equippedBuildingsSize", equippedBuildingsSize);
    }

    public static void LoadEquipmentListFromPlayerPrefs()
    {
        
        equippedBuildings = new List<Building.BuildingType>();
        if (PlayerPrefs.HasKey("equippedBuildingsSize"))
        {
            equippedBuildingsSize = PlayerPrefs.GetInt("equippedBuildingsSize");
            for (int i = 0; i < equippedBuildingsSize; ++i)
            {
                if (PlayerPrefs.HasKey($"equipment_{i}"))
                {
                    int val = PlayerPrefs.GetInt($"equipment_{i}");
                    if (val >= 0)
                    {
                        equippedBuildings.Add((Building.BuildingType)val);
                    }
                }
            }
        }
        else
        {
            equippedBuildingsSize = 3;
            equippedBuildings.Add(Building.BuildingType.CuboProduction);
            equippedBuildings.Add(Building.BuildingType.BubilProduction);
            equippedBuildings.Add(Building.BuildingType.Tawa);
            PlayerPrefs.SetInt("equippedBuildingsSize", equippedBuildingsSize);
            PlayerPrefs.SetInt($"equipment_{0}", (int)Building.BuildingType.CuboProduction);
            PlayerPrefs.SetInt($"equipment_{1}", (int)Building.BuildingType.BubilProduction);
            PlayerPrefs.SetInt($"equipment_{2}", (int)Building.BuildingType.Tawa);
        }
        
    }

    public static void SaveEquipmentListToPlayerPrefs()
    {
        PlayerPrefs.SetInt("equippedBuildingsSize", equippedBuildingsSize);
        for (int i = 0; i < equippedBuildings.Count; ++i)
        {
            PlayerPrefs.SetInt($"equipment_{i}", (int)equippedBuildings[i]);
        }
        for (int i = equippedBuildings.Count; i < equippedBuildingsSize; ++i)
        {
            PlayerPrefs.SetInt($"equipment_{i}", -1);
        }
    }

    void Update()
    {
        Vector2 mousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursor.transform.position = new Vector3(mousePosition.x, mousePosition.y, -9.6f);

        if (Input.GetMouseButtonDown(0))
        {
            cursor.transform.localScale = new Vector3(0.85f, 0.85f, 1);
        }
        if (Input.GetMouseButtonUp(0))
        {
            cursor.transform.localScale = new Vector3(1f, 1f, 1);
            cursorSr.color = Color.white;
        }
    }

    public static void SetCursor(Sprite s)
    {
        cursorSr.sprite = s;
    }

    //Useful shortcuts:

    public static int GetUpgradeLvl(UpgradeHandle h)
    {
        return upgradeStates[(int)h].upgradeLvl;
    }
}
