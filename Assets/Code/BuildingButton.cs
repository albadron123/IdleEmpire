using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BuildingButton : MonoBehaviour
{
    
    [HideInInspector]
    public Building.BuildingType type;    


    [Header("View")]

    [SerializeField] TMPro.TMP_Text titleTe;
    [SerializeField] TMPro.TMP_Text priceTe;
    [SerializeField] TMPro.TMP_Text[] otherPricesTe;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] SpriteRenderer backgroundSr;

    [SerializeField] GameObject[] changeButtons;


    [Header("Extra")]

    [SerializeField] Color[] levelColors;
    const float minLevelButtonX = 1.55f;
    const float maxLevelButtonX = 1.7f;

    void Start()
    {
        
    }

    public void Init(Building.BuildingType buildingType)
    {
        type = buildingType;
        titleTe.text = DataStorage.allBuildings[(int)type].title;
        sr.sprite = DataStorage.allBuildings[(int)type].icon;
    }

    public void SelectLevel(int lvl)
    {
        if (G.buildingStates[(int)type].currentLvl != lvl)
        {
            G.buildingStates[(int)type].currentLvl = lvl;

            int prevLevel = G.buildingStates[(int)type].currentLvl;
            priceTe.text = DataStorage.CalculateBuildingPrice(type).ToString();
            backgroundSr.color = levelColors[lvl];
            sr.sprite = DataStorage.allBuildings[(int)type].icon;

            for (int i = 0; i < changeButtons.Length; ++i)
            {
                changeButtons[i].transform.localPosition = new Vector3(changeButtons[i].transform.localPosition.x, changeButtons[i].transform.localPosition.y, 1);
            }

            changeButtons[lvl].transform.localPosition = new Vector3(changeButtons[lvl].transform.localPosition.x, changeButtons[lvl].transform.localPosition.y, -1);
            changeButtons[lvl].transform.DOLocalMoveX(maxLevelButtonX, 0.2f);
            changeButtons[prevLevel].transform.DOLocalMoveX(minLevelButtonX, 0.2f);
        }
    }


    public void UpdatePrices()
    {
        int newPrice = DataStorage.CalculateBuildingPrice(type);

        otherPricesTe[G.buildingStates[(int)type].currentLvl].text = $"{G.buildingStates[(int)type].currentLvl + 1}<size=2.2>(={newPrice}cubo)</size>";
        priceTe.text = newPrice.ToString();
    }
}
