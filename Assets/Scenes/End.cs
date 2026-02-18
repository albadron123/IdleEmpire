using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class End : MonoBehaviour
{
    [SerializeField]
    TMPro.TMP_Text te;
    // Start is called before the first frame update
    void Start()
    {
        int bones = 0;
        int record = 0;
        if (PlayerPrefs.HasKey("recordBones"))
        {
            record = PlayerPrefs.GetInt("recordBones");
        }
        if (PlayerPrefs.HasKey("currentBones"))
        {
            bones = PlayerPrefs.GetInt("currentBones");
        }
        te.text = $"Now you crushed {bones} bones. \nYour record is {record} bones.";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
