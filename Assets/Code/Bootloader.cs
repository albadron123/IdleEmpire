using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[DefaultExecutionOrder(-9999)]
public static class Bootloader
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InitializeGame()
    {
        Debug.Log("---Init game---");

        PlayerPrefs.DeleteAll();
        DOTween.Init(recycleAllByDefault: true, useSafeMode: true).SetCapacity(2000, 200);


        //Initializing global services
        GameObject staticContainer = new GameObject("===Global Managers===");
        Object.DontDestroyOnLoad(staticContainer);

        GameObject soundManagerGO = new GameObject("Sound Manager");
        soundManagerGO.transform.parent = staticContainer.transform;
        soundManagerGO.AddComponent<SoundManager>();

        GameObject dataStorageGO = new GameObject("Data Storage");
        dataStorageGO.transform.parent = staticContainer.transform;
        DataStorage ds = dataStorageGO.AddComponent<DataStorage>();

        GameObject G_GO = new GameObject("G");
        G_GO.transform.parent = staticContainer.transform;
        G_GO.AddComponent<G>();

        //Load all the saved stuff & essential data
        DataStorage.LoadSound();
        DataStorage.LoadBuildings();
        DataStorage.LoadUpgrades();
        DataStorage.LoadEnemies();
        /*
        if (!DataStorage.DeserializeAll())
        {
            DataStorage.SerializeAll();
        }
        */

        G.equippedBuildingsCapacity = 6;
        G.bonesSpent = 0;
        G.InitBuildingStates();
        G.InitUpgradeStates();
        G.LoadEquipmentListFromPlayerPrefs();

    }
    

}
