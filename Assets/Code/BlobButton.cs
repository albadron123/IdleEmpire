using UnityEngine;
using DG.Tweening;

public class BlobButton : MonoBehaviour
{
    public BlobHandle handle;
    [SerializeField] UpgradeHandle upgradeNeeded;
    [SerializeField] GameObject placeholderButton;

    private Interactable interactable;
    [SerializeField] TMPro.TMP_Text priceTe;

    [SerializeField] SpriteRenderer backgroundSr;


    private void Start()
    {
        if (handle == BlobHandle.Basic || G.upgradeStates[(int)upgradeNeeded].upgradeLvl > 0)
        {
            interactable = GetComponent<Interactable>();
            interactable.getDescriptionEvent.AddListener(GetDescription);
            placeholderButton.SetActive(false);
            CoreGame.inst.OnBlobAquired += UpdatePrices;
            CoreGame.inst.OnResourceChanged[(int)Resource.ResourceType.Bubil] += ReactOnResourceAndCostChange;
            UpdatePrices();
        }
        else
        {
            placeholderButton.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public void PerformCancelAction()
    {
        interactable.PerformCancelAction();
    }

    public void GetDescription(StringContainer strContainer)
    {
        strContainer.str = DataStorage.blobDescriptions[(int)handle];
    }

    public void UpdatePrices()
    {
        int newPrice = DataStorage.CalculateBlobPrice();
        priceTe.text = CoreGame.BUBIL_ICON_STR + newPrice.ToString();
        ReactOnResourceAndCostChange();
    }

    private void ReactOnResourceAndCostChange()
    {
        bool doEnable = DataStorage.CalculateBlobPrice() <= CoreGame.inst.allResources[(int)Resource.ResourceType.Bubil].value;
        backgroundSr.DOKill();
        backgroundSr.DOColor(doEnable ? CoreGame.CREAMY_YELLOW_COLOR : Color.white, 0.2f);
    }
}

