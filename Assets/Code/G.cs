using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G : MonoBehaviour
{
    public static G inst;

    // Equipped buildings
    public static List<Building.BuildingType> equippedBuildings;
    public static int equippedBuildingsSize;
    public static int equippedBuildingsCapacity;


    public static int bonesSpent;
    public static int maxBonesSpent = 100;


    void Start()
    {
        if (inst != null)
        {
            Debug.LogError("More then 1 G on the scene");
            Destroy(gameObject);
        }
        inst = this;

        
        equippedBuildingsCapacity = 6;
        LoadEquipmentListFromPlayerPrefs();

        bonesSpent = 0;
    }


    public static void LoadEquipmentListFromPlayerPrefs()
    {
        equippedBuildingsSize = 3;
        equippedBuildings = new List<Building.BuildingType>();
        if (PlayerPrefs.HasKey("equippedBuildingsSize"))
        {
            equippedBuildingsSize = PlayerPrefs.GetInt("equippedBuildingsSize");
        }
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
        
    }
}
