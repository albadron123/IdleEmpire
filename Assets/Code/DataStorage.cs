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
            icon = sprites.Find(x=>x.name == "Tawa")
        };
        allBuildings[(int)Building.BuildingType.CuboProduction] = new BuildingData() { 
            title = "Cubo", 
            icon = sprites.Find(x => x.name == "Cubo") 
        };
        allBuildings[(int)Building.BuildingType.BubilProduction] = new BuildingData() { 
            title = "Bubil", 
            icon = sprites.Find(x => x.name == "Bubil") 
        };
        allBuildings[(int)Building.BuildingType.HutkaGrande] = new BuildingData() { 
            title = "Hutka Grande", 
            icon = sprites.Find(x => x.name == "MainTower") 
        };
        allBuildings[(int)Building.BuildingType.Tumbo] = new BuildingData() { 
            title = "Tumbo", 
            icon = sprites.Find(x => x.name == "Tumbo") 
        };
        allBuildings[(int)Building.BuildingType.Flawa] = new BuildingData() { 
            title = "Flawa", 
            icon = sprites.Find(x => x.name == "Flawa") 
        };
        allBuildings[(int)Building.BuildingType.Magno] = new BuildingData() {
            title = "Magno",
            icon = sprites.Find(x => x.name == "Magno")
        };
        allBuildings[(int)Building.BuildingType.Custik] = new BuildingData() { 
            title = "Custik",
            icon = sprites.Find(x => x.name == "Custik")
        };
        allBuildings[(int)Building.BuildingType.Bombo] = new BuildingData() { 
            title = "Bombo",
            icon = sprites.Find(x => x.name == "Bombo")
        };
        allBuildings[(int)Building.BuildingType.Cacti] = new BuildingData() { 
            title = "Cacti",
            icon = sprites.Find(x => x.name == "Cacti")
        };
    }


    
    void Update()
    {
        
    }
}
