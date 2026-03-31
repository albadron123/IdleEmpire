using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingButton : MonoBehaviour
{
    public BuildingTag myBuildingTag;
    [SerializeField]
    Color[] levelColors;
    [SerializeField]
    GameObject[] changeButtons;
    [SerializeField]
    SpriteRenderer backgroundSr;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectLevel(int level)
    {
        if (myBuildingTag.buidlingLvl != level)
        {
            myBuildingTag.buidlingLvl = level;
            myBuildingTag.priceTe.text = myBuildingTag.prices[level].ToString();
            backgroundSr.color = levelColors[level];
            myBuildingTag.sr.sprite = myBuildingTag.sprites[level];

            for (int i = 0; i < changeButtons.Length; ++i)
            {
                changeButtons[i].transform.localPosition = new Vector3(changeButtons[i].transform.localPosition.x, changeButtons[i].transform.localPosition.y, 1);
            }
            changeButtons[level].transform.localPosition = new Vector3(changeButtons[level].transform.localPosition.x, changeButtons[level].transform.localPosition.y, -1);
        }
    }
}
