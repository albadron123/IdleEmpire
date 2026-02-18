using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum UpgradeType
{
    productionTime,
    productionAmount,

    projectileDamage,
    projectileSize,
    shootingSpeed,
}

public class UpgradeButton : MonoBehaviour
{
    [SerializeField]
    TMPro.TMP_Text titleTe;
    [SerializeField]
    TMPro.TMP_Text priceTe;

    

    public BuildingObject bObj;
    public UpgradeType myType;


    void Start()
    {
        ReflectState();
    }

    
    void Update()
    {
        
    }

    public void Upgrade()
    {
        int lvl   = GetLevel();
        int price = GetPrice();

        if (lvl < 4 && price <= CoreGame.inst.allResources[(int)Resource.ResourceType.bones].value)
        {
            // Aquire upgrade
            CoreGame.inst.ChangeResource(Resource.ResourceType.bones, -price);
            // Apply Effect
            switch (myType)
            {
                case UpgradeType.productionAmount:
                    ++bObj.productionAmountLevel;
                    break;
                case UpgradeType.productionTime:
                    ++bObj.productionTimeLevel;
                    break;
                case UpgradeType.shootingSpeed:
                    ++bObj.shootingSpeedLevel;
                    break;
                case UpgradeType.projectileDamage:
                    ++bObj.projectileDamageLevel;
                    break;
                case UpgradeType.projectileSize:
                    ++bObj.projectileSizeLevel;
                    break;
            }
            // Reflect change on the button
            ReflectState();
        }
    }

    public int GetLevel()
    {
        switch (myType)
        {
            case UpgradeType.productionAmount:
                return bObj.productionAmountLevel;
            case UpgradeType.productionTime:
                return bObj.productionTimeLevel;
            case UpgradeType.shootingSpeed:
                return bObj.shootingSpeedLevel;
            case UpgradeType.projectileDamage:
                return bObj.projectileDamageLevel;
            case UpgradeType.projectileSize:
                return bObj.projectileSizeLevel;
        }
        return -1;
    }

    public int GetPrice()
    {
        switch (myType)
        {
            case UpgradeType.productionAmount:
                return (int)(2 + (bObj.productionAmountLevel + 1) * 1.6f);
            case UpgradeType.productionTime:
                return (int)(0 + (bObj.productionTimeLevel + 1) * 1.4f);
            case UpgradeType.shootingSpeed:
                return (int)(1 + (bObj.shootingSpeedLevel + 1) * 1.4f);
            case UpgradeType.projectileDamage:
                return (int)(2 + (bObj.projectileDamageLevel + 1) * 1.4f);
            case UpgradeType.projectileSize:
                return (int)(0 + (bObj.projectileSizeLevel + 1) * 1.4f);
        }
        return 0;
    }

    public void ReflectState()
    {
        priceTe.text = GetPrice().ToString();
        switch (myType)
        {
            case UpgradeType.productionAmount:
                if (bObj.productionAmountLevel == 4)
                {
                    titleTe.text = $"MAX PRODUCT AMOUNT\n (LVL {bObj.productionAmountLevel + 1}/5)";
                    priceTe.text = "";
                }
                else
                {
                    titleTe.text = $"More product.\n(LVL {bObj.productionAmountLevel + 1}/5)";
                }
                break;
            case UpgradeType.productionTime:
                if (bObj.productionTimeLevel == 4)
                {
                    titleTe.text = $"MAX PRODUCTION SPEED\n (LVL {bObj.productionTimeLevel + 1}/5)";
                    priceTe.text = "";
                }
                else
                {
                    titleTe.text = $"faster production.\n(LVL {bObj.productionTimeLevel + 1}/5)";
                }
                break;
            case UpgradeType.shootingSpeed:
                if (bObj.shootingSpeedLevel == 4)
                {
                    titleTe.text = $"MAX SHOOTING SPEED\n (LVL {bObj.shootingSpeedLevel + 1}/5)";
                    priceTe.text = "";
                }
                else
                {
                    titleTe.text = $"faster shooting reload.\n(LVL {bObj.shootingSpeedLevel + 1}/5)";
                }
                break;
            case UpgradeType.projectileDamage:
                if (bObj.projectileDamageLevel == 4)
                {
                    titleTe.text = $"MAX DAMAGE\n (LVL {bObj.projectileDamageLevel + 1}/5)";
                    priceTe.text = "";
                }
                else
                {
                    titleTe.text = $"more damage.\n(LVL {bObj.projectileDamageLevel + 1}/5)";
                }
                break;
            case UpgradeType.projectileSize:
                if (bObj.projectileSizeLevel == 4)
                {
                    titleTe.text = $"MAX PROJECTILE SIZE\n (LVL {bObj.projectileSizeLevel + 1}/5)";
                    priceTe.text = "";
                }
                else
                {
                    titleTe.text = $"Bigger projectiles.\n(LVL {bObj.projectileSizeLevel + 1}/5)";
                }
                break;
        }

    }
}
