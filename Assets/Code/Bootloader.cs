using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-9999)]
public static class Bootloader
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InitializeGame()
    {
        Debug.Log("---Init game---");

        //PlayerPrefs.DeleteAll();

        //Initializing global services
        GameObject staticContainer = new GameObject("===Global Managers===");
        Object.DontDestroyOnLoad(staticContainer);

        GameObject soundManagerGO = new GameObject("Sound Manager");
        soundManagerGO.transform.parent = staticContainer.transform;
        soundManagerGO.AddComponent<SoundManager>();

        GameObject dataStorageGO = new GameObject("Data Storage");
        dataStorageGO.transform.parent = staticContainer.transform;
        dataStorageGO.AddComponent<DataStorage>();

        GameObject G_GO = new GameObject("G");
        G_GO.transform.parent = staticContainer.transform;
        G_GO.AddComponent<G>();

    }
    

}
