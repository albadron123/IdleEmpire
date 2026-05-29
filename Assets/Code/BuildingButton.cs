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
    
    [SerializeField] SpriteRenderer sr;
    [SerializeField] SpriteRenderer backgroundSr;

    private Interactable interactable;

    void Start()
    {
        
    }

    public void PerformCancelAction()
    {
        interactable.PerformCancelAction();
    }

    public void Init(Building.BuildingType buildingType)
    {
        interactable = GetComponent<Interactable>();
        interactable.getDescriptionEvent.AddListener(GetDescription);
        CoreGame.inst.OnResourceChanged[(int)Resource.ResourceType.Cubo] += ReactOnResourceAndCostChange;

        type = buildingType;
        titleTe.text = DataStorage.allBuildings[(int)type].title;
        sr.sprite = DataStorage.allBuildings[(int)type].icon;
        UpdatePrices();
    }

    public void GetDescription(StringContainer strContainer)
    {
        strContainer.str = DataStorage.allBuildings[(int)type].description;
    }

    public void ReactOnResourceAndCostChange()
    {
        bool doEnable = DataStorage.CalculateBuildingPrice(type) <= CoreGame.inst.allResources[(int)Resource.ResourceType.Cubo].value;
        backgroundSr.DOKill();
        backgroundSr.DOColor(doEnable ? CoreGame.CREAMY_YELLOW_COLOR : Color.white, 0.2f);
    }

    public void UpdatePrices()
    {
        priceTe.text = DataStorage.CalculateBuildingPrice(type).ToString();
        ReactOnResourceAndCostChange();
    }
}
