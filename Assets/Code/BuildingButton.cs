using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BuildingButton : MonoBehaviour
{
    public BuildingTag myBuildingTag;
    [SerializeField]
    Color[] levelColors;
    [SerializeField]
    GameObject[] changeButtons;
    [SerializeField]
    SpriteRenderer backgroundSr;


    const float minLevelButtonX = 1.55f;
    const float maxLevelButtonX = 1.7f;

    void Start()
    {
        
    }

    public void Init(Building.BuildingType buildingType)
    {
        myBuildingTag.titleTe.text = DataStorage.allBuildings[(int)buildingType].title;
        myBuildingTag.sr.sprite = DataStorage.allBuildings[(int)buildingType].icon;
    }

    public void SelectLevel(int level)
    {
        if (myBuildingTag.buidlingLvl != level)
        {
            int prevLevel = myBuildingTag.buidlingLvl;
            myBuildingTag.buidlingLvl = level;
            myBuildingTag.priceTe.text = myBuildingTag.prices[level].ToString();
            backgroundSr.color = levelColors[level];
            myBuildingTag.sr.sprite = myBuildingTag.sprites[level];

            for (int i = 0; i < changeButtons.Length; ++i)
            {
                changeButtons[i].transform.localPosition = new Vector3(changeButtons[i].transform.localPosition.x, changeButtons[i].transform.localPosition.y, 1);
            }
            changeButtons[level].transform.localPosition = new Vector3(changeButtons[level].transform.localPosition.x, changeButtons[level].transform.localPosition.y, -1);
            changeButtons[level].transform.DOLocalMoveX(maxLevelButtonX, 0.2f);
            changeButtons[prevLevel].transform.DOLocalMoveX(minLevelButtonX, 0.2f);
        }
    }
}
