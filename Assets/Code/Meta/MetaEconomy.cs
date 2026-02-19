using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaEconomy : MonoBehaviour
{
    [SerializeField]
    TMPro.TMP_Text bonesTe;

    public static MetaEconomy inst = null;
    public int bones = 0;
    public int record = 0;
    public int lastRound = 0;

    void Start()
    {
        if (inst != null)
        {
            Debug.LogError("MetaEconomy.cs is singleton!");
            Destroy(this);
        }
        inst = this;

        if (PlayerPrefs.HasKey("totalBones"))
        {
            bones = PlayerPrefs.GetInt("totalBones");
        }
        if (PlayerPrefs.HasKey("recordBones"))
        {
            record = PlayerPrefs.GetInt("recordBones");
        }
        if (PlayerPrefs.HasKey("currentBones"))
        {
            lastRound = PlayerPrefs.GetInt("currentBones");
        }
        bonesTe.text = $"BONES: {bones}";
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            UpdateBones(10);
        }
    }

    public bool UpdateBones(int delta)
    {
        if (bones + delta < 0)
        {
            return false;
        }

        bones += delta;
        bonesTe.text = $"BONES: {bones}";

        PlayerPrefs.SetInt("totalBones", bones);
        PlayerPrefs.Save();

        return true;
    }
}
