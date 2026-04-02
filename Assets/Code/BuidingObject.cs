using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BuildingObject : MonoBehaviour, IDestructable
{

    Transform t;

    public Building b;


    public List<GameObject> blobPlaces;

    private Coroutine[] processes;
    private GameObject[] sliders;
    private Blob[] blobs;

    private SpriteRenderer sr;


    public GameObject outline;

    public GameObject rotationPart = null;

    
    [Header("Attack Variables")]

    float[] towerAnglePerPlace;

    public float baseProjectileSize = 1.15f;
    public float baseShootingSpeed = 0.9f;
    
    public int projectileDamageLevel = 0;
    public int shootingSpeedLevel = 0;
    public int projectileSizeLevel = 0;

    [Header("Production Variables")]
    
    public float baseProductionTime = 2;

    public int productionTimeLevel = 0;
    public int productionAmountLevel = 0;


    [Header("Cacti Variables")]

    [SerializeField] Sprite cacti1Spr;
    [SerializeField] Sprite cacti2Spr;
    [SerializeField] Sprite cacti3Spr;
    [SerializeField] Collider2D specialPurposeCol;
    List<GameObject> inCollision = new List<GameObject>();

    [SerializeField]
    List<UpgradeType> upgradeTypes;

    void RegisterBuilding()
    {
        if (CoreGame.inst.builtObjects == null)
        {
            CoreGame.inst.builtObjects = new List<BuildingObject>();
        }
        CoreGame.inst.builtObjects.Add(this);
    }

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        RegisterBuilding();

        t = transform;

        processes = new Coroutine[blobPlaces.Count];
        sliders = new GameObject[blobPlaces.Count];
        blobs = new Blob[blobPlaces.Count];

        towerAnglePerPlace = new float[blobPlaces.Count];
        for (int i = 0; i < towerAnglePerPlace.Length; ++i)
        {
            towerAnglePerPlace[i] = 0;
        }


        if (b.myType == Building.BuildingType.Tree)
        {
            StartCoroutine(FunctionCoroutine(0));
        }
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Here is some code duplication to be fixed later (also in friend and enemy creature updates)
        //Collision with projectiles
        {
            List<Collider2D> cols = new List<Collider2D>();
            Physics2D.OverlapCollider(GetComponent<Collider2D>(), new ContactFilter2D().NoFilter(), cols);
            foreach (Collider2D col in cols)
            {
                if (col.gameObject.tag == CoreGame.TAG_ENEMY_PROJECTILE)
                {
                    Projectile proj = col.gameObject.GetComponent<Projectile>();
                    if (!proj.ignoreList.Contains(gameObject))
                    {
                        int damage = proj.damage;
                        Destroy(col.gameObject);
                        DestructableObject dObj = GetComponent<DestructableObject>();
                        dObj.ChangeHealth(-damage);
                        if (dObj.health <= 0)
                        {
                            break;
                        }
                    }
                }
            }
        }

        if(b.myType == Building.BuildingType.Cacti)
        {
            List<Collider2D> cols = new List<Collider2D>();
            Physics2D.OverlapCollider(specialPurposeCol, new ContactFilter2D().NoFilter(), cols);
            foreach (Collider2D col in cols)
            {
                if (col.gameObject.tag == CoreGame.TAG_ENEMY)
                {
                    int myDamage = GetDamage();
                    if (col.gameObject.tag == CoreGame.TAG_ENEMY && !inCollision.Contains(col.gameObject))
                    {
                        inCollision.Add(col.gameObject);
                        StartCoroutine(CactiReload(col.gameObject));
                        
                        col.gameObject.GetComponent<DestructableObject>().ChangeHealth(-myDamage);
                        DestructableObject dObj = GetComponent<DestructableObject>();
                        dObj.ChangeHealth(-3);

                    }
                }
            }

        }
    }

    IEnumerator CactiReload(GameObject o)
    {
        yield return new WaitForSeconds(1);
        inCollision.Remove(o);
    }


    public void ChangeHealth(int damage)
    {
        if (damage < 0)
        {
            CoreGame.inst.CreateIconPopUp((Vector2)t.position + new Vector2(0.5f, 0f), $"{damage} hp".Bold(), null);
        }
        else
        {
            CoreGame.inst.CreateIconPopUp((Vector2)t.position + new Vector2(0.5f, 0f), $"+{damage} hp".Bold().Color("#559F52"), null);
        }

        if (b.myType == Building.BuildingType.Cacti)
        {
            DestructableObject dObj = GetComponent<DestructableObject>();
            float healthPortion = (float)dObj.health / dObj.maxHealth;
            if (healthPortion >= 0.67f)
            {
                sr.sprite = cacti1Spr;
            }
            if (healthPortion < 0.67f)
            {
                sr.sprite = cacti2Spr;
            }
            if (healthPortion < 0.2f)
            {
                sr.sprite = cacti3Spr;
            }
        }
    }

    public void Die()
    {
        CoreGame.inst.builtObjects.Remove(this);
        if (b.myType == Building.BuildingType.MajorTower)
        {
            CoreGame.inst.EndRun();
        }
    }

    public void AddBlob(Blob blob, GameObject blobPlace)
    {
        int processId = blobPlaces.LastIndexOf(blobPlace);
        blobs[processId] = blob;

        
        //TOWER SPECIFIC
        towerAnglePerPlace[processId]++;
        if (towerAnglePerPlace[processId] > 3)
        {
            towerAnglePerPlace[processId] = 0;
            
        }

        if (rotationPart != null)
        {
            rotationPart.transform.rotation = Quaternion.Euler(0, 0, 90 * towerAnglePerPlace[processId]);
        }
        //

        processes[processId] = StartCoroutine(FunctionCoroutine(processId));
    }

    public void RemoveBlob(Blob blob, GameObject blobPlace)
    {
        int processId = blobPlaces.LastIndexOf(blobPlace);
        blobs[processId] = null;

        if (processes[processId] != null)
        {   
            StopCoroutine(processes[processId]);
            processes[processId] = null;
        }
        if (sliders[processId] != null)
        {
            Destroy(sliders[processId]);
            sliders[processId] = null;
        }
    }

    public IEnumerator FunctionCoroutine(int processId)
    {
        if (b.myType == Building.BuildingType.CubeProduction)
        {

            while (true)
            {
                float productionTime = GetProductionTime();
                int productionAmount = GetProductionAmount();

                DOTween.Sequence()
                    .Append(t.DOScale(new Vector3(1.1f, 1.1f, 1), 0.25f * productionTime))
                    .Append(t.DOScale(new Vector3(1f, 1f, 1), 0.25f * productionTime)).SetLoops(2);

                GameObject sliderInst = Instantiate(CoreGame.inst.sliderPfb, blobs[processId].transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                sliderInst.transform.GetChild(0).transform.DOLocalMove(new Vector3(0, 0, 0), productionTime);
                sliders[processId] = sliderInst;

                yield return new WaitForSeconds(productionTime);

                Destroy(sliderInst);
                sliders[processId] = null;

                SoundManager.inst.PlaySfx(SoundManager.inst.SFX_PRODUCE_CUBO);
                CoreGame.inst.ChangeResource(Resource.ResourceType.cubes, productionAmount);
                CoreGame.inst.CreateIconPopUp(blobs[processId].transform.position, $"+{productionAmount}", CoreGame.inst.allResources[0].icon);
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

                GameObject sliderInst = Instantiate(CoreGame.inst.sliderPfb, blobs[processId].transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                sliderInst.transform.GetChild(0).transform.DOLocalMove(new Vector3(0, 0, 0), productionTime);
                sliders[processId] = sliderInst;

                yield return new WaitForSeconds(productionTime);

                Destroy(sliderInst);
                sliders[processId] = null;


                SoundManager.inst.PlaySfx(SoundManager.inst.SFX_PRODUCE_BUBIL);
                CoreGame.inst.ChangeResource(Resource.ResourceType.blah, productionAmount);
                CoreGame.inst.CreateIconPopUp(blobs[processId].transform.position, $"+{productionAmount}", CoreGame.inst.allResources[1].icon);
            }
        }
        else if (b.myType == Building.BuildingType.Tower || b.myType == Building.BuildingType.Flower)
        {
            GameObject myProjectilePfb = null;
            if (b.myType == Building.BuildingType.Tower)
            {
                myProjectilePfb = CoreGame.inst.projectilePfb;
            }
            else
            {
                myProjectilePfb = CoreGame.inst.healingProjectilePfb;
            }
            while (true)
            {
                //REDOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
                float shootingSpeed = GetShootingSpeed();
                float projectileSize = GetProjectileSize();
                int damage = GetDamage();

                SoundManager.inst.PlaySfx(SoundManager.inst.SFX_SHOOT, minPitch: 0.95f, maxPitch: 1.05f);
                GameObject inst = Instantiate(myProjectilePfb, (Vector3)(Vector2)blobs[processId].transform.position + new Vector3(0, 0, -9), Quaternion.identity);
                inst.transform.localScale = new Vector3(projectileSize, projectileSize, 1);
                Projectile pr = inst.GetComponent<Projectile>();
                pr.damage = damage;
                pr.ignoreList.Add(gameObject);
                if (b.myType == Building.BuildingType.Flower)
                {
                    pr.doAffectBlobs = false;    
                }
                else
                {
                    pr.doAffectBlobs = true;
                }

                if (towerAnglePerPlace[processId] == 0)
                {
                    pr.direction = Vector3.right;
                }
                else if (towerAnglePerPlace[processId] == 1)
                {
                    pr.direction = Vector3.up;
                }
                else if (towerAnglePerPlace[processId] == 2)
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
        else if (b.myType == Building.BuildingType.Archery)
        {
            while (true)
            {
                float shootingSpeed = GetShootingSpeed();
                float projectileSize = GetProjectileSize();
                int damage = GetDamage();


                GameObject sliderInst = Instantiate(CoreGame.inst.sliderPfb, blobs[processId].transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                sliderInst.transform.GetChild(0).transform.DOLocalMove(new Vector3(0, 0, 0), shootingSpeed);
                yield return new WaitForSeconds(shootingSpeed);


                GameObject nearestEnemy = MaximUtils.GetNearestWithTag(t.position, CoreGame.TAG_ENEMY);
                if (nearestEnemy == null)
                {
                    yield return new WaitForSeconds(shootingSpeed);
                    continue;
                }

                Destroy(sliderInst);


                SoundManager.inst.PlaySfx(SoundManager.inst.SFX_SHOOT, minPitch: 0.95f, maxPitch: 1.05f);
                GameObject inst = Instantiate(CoreGame.inst.arrowProjectilePfb, (Vector3)(Vector2)blobs[processId].transform.position + new Vector3(0, 0, -9), Quaternion.identity);
                inst.transform.localScale = new Vector3(projectileSize, projectileSize, 1);
                Projectile pr = inst.GetComponent<Projectile>();
                pr.damage = damage;
                pr.direction = ((Vector2)(nearestEnemy.transform.position - t.position)).normalized;
                inst.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(pr.direction.y, pr.direction.x)*Mathf.Rad2Deg);
                Destroy(inst, 0.75f);
            }
        }
        else if (b.myType == Building.BuildingType.Tree)
        {
            while (true)
            {
                float productionTime = baseProductionTime;
                int productionAmount = GetProductionAmount();

                GameObject sliderInst = Instantiate(CoreGame.inst.sliderPfb, transform.position + new Vector3(0, 1.4f, 0), Quaternion.identity);
                sliderInst.transform.GetChild(0).transform.DOLocalMove(new Vector3(0, 0, 0), productionTime);
                sliders[processId] = sliderInst;

                yield return new WaitForSeconds(productionTime);

                Destroy(sliderInst);
                sliders[processId] = null;


                SoundManager.inst.PlaySfx(SoundManager.inst.SFX_PRODUCE_BUBIL);
                CoreGame.inst.ChangeResource(Resource.ResourceType.blah, productionAmount);
                CoreGame.inst.CreateIconPopUp(transform.position + new Vector3(0, 1.4f, 0), $"+{productionAmount}", CoreGame.inst.allResources[1].icon);
            }
        }
        else if (b.myType == Building.BuildingType.Magnet)
        {

        }
        else if (b.myType == Building.BuildingType.BombProduction)
        {
            while (true)
            {
                while(MaximUtils.GetAnyOverlappedWithTag2D(specialPurposeCol, CoreGame.TAG_BOMB) != null) {
                    //wait for the bomb to be taken
                    yield return new WaitForSeconds(0.2f);
                }

                float productionTime = GetProductionTime();

                DOTween.Sequence()
                    .Append(t.DOScale(new Vector3(1.1f, 1.1f, 1), 0.25f * productionTime))
                    .Append(t.DOScale(new Vector3(1f, 1f, 1), 0.25f * productionTime)).SetLoops(2);

                GameObject sliderInst = Instantiate(CoreGame.inst.sliderPfb, blobs[processId].transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                sliderInst.transform.GetChild(0).transform.DOLocalMove(new Vector3(0, 0, 0), productionTime);
                sliders[processId] = sliderInst;

                yield return new WaitForSeconds(productionTime);

                Destroy(sliderInst);
                sliders[processId] = null;


                SoundManager.inst.PlaySfx(SoundManager.inst.SFX_PRODUCE_BUBIL);
                Instantiate(CoreGame.inst.bombPfb, specialPurposeCol.gameObject.transform.position + Vector3.back, Quaternion.identity);
            }
        }
    }

    private void OnMouseDown()
    {
        if (CoreGame.inst.selectedBuilding == this)
        {
            CoreGame.inst.HideUpgrades();
        }
        else
        {
            CoreGame.inst.ShowUpgrades(upgradeTypes, this);
        }
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
        if (b.myType == Building.BuildingType.Tower)
        {
            return baseShootingSpeed - shootingSpeedLevel * 0.15f;
        }
        if (b.myType == Building.BuildingType.Archery)
        {
            return baseShootingSpeed - shootingSpeedLevel * 0.15f;
        }
        if (b.myType == Building.BuildingType.Flower)
        {
            return baseShootingSpeed - shootingSpeedLevel * 0.15f;
        }
        //Unreachable
        Debug.LogError("Unreachable area of code!");
        return 0;
    }

    public int GetDamage()
    {
        if (b.myType == Building.BuildingType.Tower)
        {
            return (projectileDamageLevel + 1) * 10;
        }
        if (b.myType == Building.BuildingType.Archery)
        {
            return (projectileDamageLevel + 1) * 10;
        }
        if (b.myType == Building.BuildingType.Flower)
        {
            return (projectileDamageLevel + 1) * (-1);
        }
        if (b.myType == Building.BuildingType.Cacti)
        {
            return (projectileDamageLevel + 1)*3;
        }
        //Unreachable
        Debug.LogError("Unreachable area of code!");
        return 0;
    }

    public float GetProjectileSize()
    {
        return baseProjectileSize + projectileSizeLevel * 0.1f;   
    }    


}
