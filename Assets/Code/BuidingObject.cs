using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class BuildingObject : MonoBehaviour, IDestructable
{
    [SerializeField]
    List<Blob> blobs;


    List<GameObject> sliders = new List<GameObject>();

    Transform t;

    Vector3 initilaScale;

    public Building b;

    private Coroutine process;


    // tower variables
    float towerAngle = 0;

    public float baseProjectileSize = 1.15f;
    public float baseShootingSpeed = 0.9f;
    
    public int projectileDamageLevel = 0;
    public int shootingSpeedLevel = 0;
    public int projectileSizeLevel = 0;

    // bubil & cubo variables
    public float baseProductionTime = 2;

    public int productionTimeLevel = 0;
    public int productionAmountLevel = 0;

    [SerializeField]
    List<UpgradeType> upgradeTypes;


    void Start()
    {
        t = transform;
        initilaScale = t.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ChangeHealth(int damage)
    {
        if (damage < 0)
        {
            CoreGame.inst.CreateIconPopUp(t.position, $"{damage} hp", null);
        }
    }

    public void Die()
    {
        if (b.myType == Building.BuildingType.MajorTower)
        {
            int lastRecord = 0;
            int bones = CoreGame.inst.allResources[2].value;
            if (PlayerPrefs.HasKey("recordBones"))
            {
                lastRecord = PlayerPrefs.GetInt("recordBones");
            }
            PlayerPrefs.SetInt("currentBones", bones);
            if (bones > lastRecord)
            {
                PlayerPrefs.SetInt("recordBones", bones);
            }
            SceneManager.LoadScene("End");
        }
    }

    public void StartFunctioning()
    {
        process = StartCoroutine(FunctionCoroutine());
    }

    public void StopFunctioning()
    {
        if (process != null)
        {
            StopCoroutine(process);
            process = null;
        }
    }

    public IEnumerator FunctionCoroutine()
    {
        if (b.myType == Building.BuildingType.CubeProduction)
        {

            while (true)
            {
                float productionTime = GetProductionTime();
                int productionAmount = GetProductionAmount();

                DOTween.Sequence()
                    .Append(t.DOScale(new Vector3(1.1f, 1.1f, 1), 0.25f*productionTime))
                    .Append(t.DOScale(new Vector3(1f, 1f, 1), 0.25f*productionTime)).SetLoops(2);

                for (int i = 0; i < blobs.Count; ++i)
                {
                    GameObject inst = Instantiate(CoreGame.inst.sliderPfb, blobs[i].transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                    inst.transform.GetChild(0).transform.DOLocalMove(new Vector3(0, 0, 0), productionTime);
                    Destroy(inst, productionTime);
                }


                yield return new WaitForSeconds(productionTime);
                CoreGame.inst.ChangeResource(Resource.ResourceType.cubes, productionAmount);

                //Create popups
                for (int i = 0; i < blobs.Count; ++i)
                {
                    CoreGame.inst.CreateIconPopUp(blobs[i].transform.position, $"+{productionAmount}", CoreGame.inst.allResources[0].icon);
                }
            }
        }
        else if (b.myType == Building.BuildingType.BlahProduction)
        {
            
            while (true)
            {
                float productionTime = GetProductionTime();
                int productionAmount = GetProductionAmount();

                DOTween.Sequence()
                    .Append(t.DOScale(new Vector3(1.1f, 1.1f, 1), 0.25f * productionTime))
                    .Append(t.DOScale(new Vector3(1f, 1f, 1), 0.25f * productionTime)).SetLoops(2);

                for (int i = 0; i < blobs.Count; ++i)
                {
                    GameObject inst = Instantiate(CoreGame.inst.sliderPfb, blobs[i].transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                    inst.transform.GetChild(0).transform.DOLocalMove(new Vector3(0, 0, 0), productionTime);
                    Destroy(inst, productionTime);
                }


                yield return new WaitForSeconds(productionTime);
                CoreGame.inst.ChangeResource(Resource.ResourceType.blah, productionAmount);

                for (int i = 0; i < blobs.Count; ++i)
                {
                    CoreGame.inst.CreateIconPopUp(blobs[i].transform.position, $"+{productionAmount}", CoreGame.inst.allResources[1].icon);
                }
            }
        }
        else if (b.myType == Building.BuildingType.Tower)
        {
            while (true)
            {
                //REDOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
                float shootingSpeed = GetShootingSpeed();
                float projectileSize = GetProjectileSize();
                int damage = GetProjectileDamage();
                    
                GameObject inst = Instantiate(CoreGame.inst.projectilePfb, blobs[0].transform.position, Quaternion.identity);
                inst.transform.localScale = new Vector3(projectileSize, projectileSize, 1);
                Projectile pr = inst.GetComponent<Projectile>();
                pr.damage = damage;
                if (towerAngle == 0)
                {
                    pr.direction = Vector3.right;
                }
                else if (towerAngle == 1)
                {
                    pr.direction = Vector3.up;
                }
                else if (towerAngle == 2)
                {
                    pr.direction = Vector3.left;
                }
                else 
                {
                    pr.direction = Vector3.down;
                }
                Destroy(inst, 2.1f);
                yield return new WaitForSeconds(shootingSpeed);
            }
        }
    }


    private void OnMouseDown()
    {
        //Show upgrade possibilities
        CoreGame.inst.ShowUpgrades(upgradeTypes, this);
    }

    public int GetProductionAmount()
    {
        return productionAmountLevel + 1;
    }

    public float GetProductionTime()
    {
        return baseProductionTime - productionTimeLevel * 0.3f;
    }

    public float GetShootingSpeed()
    {
        return baseShootingSpeed - shootingSpeedLevel * 0.15f;
    }

    public int GetProjectileDamage()
    {
        return projectileDamageLevel + 1;
    }

    public float GetProjectileSize()
    {
        return baseProjectileSize + projectileSizeLevel * 0.1f;   
    }
    
    


    public void AddBlob(Blob b)
    {
        towerAngle++;
        if (towerAngle > 3)
        {
            towerAngle = 0;
        }
        blobs.Add(b);
        if (blobs.Count == 1)
        {
            StartFunctioning();
        }
    }

    public void RemoveBlob(Blob b)
    {
        blobs.Remove(b);
        if (blobs.Count == 0)
        {
            StopFunctioning();
        }
    }





    /*
    private void OnMouseDown()
    {
        CoreGame.inst.ChangeResource(Resource.ResourceType.Wood, 1);
        DOTween.Sequence()
            .Append(t.DOScale(0.9f * initilaScale, 0.05f))
            .Append(t.DOScale(1.1f * initilaScale, 0.05f));
    }

    private void OnMouseEnter()
    {
        t.DOScale(1.1f * initilaScale, 0.1f);
    }

    private void OnMouseExit()
    {
        t.DOScale(initilaScale, 0.1f);
    }
    */
}
