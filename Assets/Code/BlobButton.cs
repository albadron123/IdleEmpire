using UnityEngine;

public class BlobButton : MonoBehaviour
{
    public BlobHandle handle;
    [SerializeField] UpgradeHandle upgradeNeeded;
    [SerializeField] GameObject placeholderButton;

    private Interactable interactable;
    [SerializeField] TMPro.TMP_Text priceTe;


    private void Start()
    {
        if (handle == BlobHandle.Basic || G.upgradeStates[(int)upgradeNeeded].upgradeLvl > 0)
        {
            interactable = GetComponent<Interactable>();
            placeholderButton.SetActive(false);
            CoreGame.inst.OnBlobAquired += UpdatePrices;
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
        strContainer.str = "This is a blob!";//DataStorage.allBuildings[(int)type].description;
    }

    public void UpdatePrices()
    {
        int newPrice = DataStorage.CalculateBlobPrice();
        priceTe.text = CoreGame.BLOB_ICON_STR + newPrice.ToString();
    }
}

