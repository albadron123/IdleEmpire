using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BuildingObject : MonoBehaviour, IDestructable
{
    [SerializeField]
    Transform zDivider;

    [SerializeField]
    TMPro.TMP_Text healthText;


    [SerializeField]
    Sprite destroyedBuildingSpr;


    private Transform t;
    private DestructableObject dObj;
    public SpriteRenderer sr;


    public Building b;


    public List<GameObject> blobPlaces;

    private Coroutine[] processes;
    private GameObject[] sliders;
    private Blob[] blobs;



    

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
        dObj = GetComponent<DestructableObject>();
        
        dObj.maxHealth = DataStorage.allBuildings[(int)b.myType].maxHealthPerLevel[(int)b.myLvl];
        dObj.health = dObj.maxHealth;
        dObj.InitHealth();
         
        RegisterBuilding();

        t = transform;
        if (zDivider != null)
        {
            t.position = new Vector3(t.position.x, t.position.y, zDivider.position.y);
            zDivider.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"A building object of type '{DataStorage.allBuildings[(int)b.myType].title}' has no zDivider");
            t.position = new Vector3(t.position.x, t.position.y, t.position.y);
        }

        processes = new Coroutine[blobPlaces.Count];
        sliders = new GameObject[blobPlaces.Count];
        blobs = new Blob[blobPlaces.Count];

        towerAnglePerPlace = new float[blobPlaces.Count];
        for (int i = 0; i < towerAnglePerPlace.Length; ++i)
        {
            towerAnglePerPlace[i] = 0;
        }


        if (b.myType == Building.BuildingType.Custik)
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

        if (healthText != null)
        {
            healthText.text = $"{dObj.health}hp";
        }

        if (damage < 0)
        {
            CoreGame.inst.CreateIconPopUp((Vector2)t.position + new Vector2(0.5f, 0f), $"{damage} hp".Bold().Size(50), null, 0.7f);
        }
        else
        {
            CoreGame.inst.CreateIconPopUp((Vector2)t.position + new Vector2(0.5f, 0f), $"+{damage} hp".Bold().Color("#559F52").Size(40), null, 0.7f);
        }

        if (b.myType == Building.BuildingType.Cacti)
        {    
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
        RemoveAllBlobs();

        StopAllCoroutines();

        GameObject ruinInst = Instantiate(CoreGame.inst.ruinPfb, t.position, Quaternion.identity);
        ruinInst.GetComponent<SpriteRenderer>().sprite = destroyedBuildingSpr;
        GameObject fx = Instantiate(CoreGame.inst.destructionEffect, t.position, Quaternion.identity);
        Destroy(fx, 0.8f);

        CoreGame.inst.builtObjects.Remove(this);
        if (b.myType == Building.BuildingType.HutkaGrande)
        {
            CoreGame.inst.EndRun();
        }
    }

    public void AddBlob(Blob blob, GameObject blobPlace)
    {
        int processId = blobPlaces.LastIndexOf(blobPlace);
        blobs[processId] = blob;
        blob.transform.parent = t;

        
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
        blob.transform.parent = null;

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

    public void RemoveAllBlobs()
    {
        for (int processId = 0; processId < blobs.Length; ++processId)
        {
            if (blobs[processId] == null)
            {
                continue;
            }

            Blob b = blobs[processId];
            b.transform.parent = null;

            DOTween.Sequence()
                .Append(b.transform.DOJump(t.position - 0.3f * Vector3.up + (Vector3)MaximUtils.RandomVector2(0.4f), Random.Range(0.7f, 0.9f), 1, 0.6f))
                .AppendCallback(() => { b.GetComponent<Creature>().StartSimulation(); Debug.Log("b is simulated"); });

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
    }

    public IEnumerator FunctionCoroutine(int processId)
    {
        if (b.myType == Building.BuildingType.CuboProduction)
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

                SoundManager.inst.PlaySfx(DataStorage.SFX_PRODUCE_CUBO);
                CoreGame.inst.ChangeResource(Resource.ResourceType.Cubo, productionAmount);
                CoreGame.inst.CreateIconPopUp(blobs[processId].transform.position, $"+{productionAmount}", CoreGame.inst.allResources[0].icon);
            }
        }
        else if (b.myType == Building.BuildingType.BubilProduction)
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


                SoundManager.inst.PlaySfx(DataStorage.SFX_PRODUCE_BUBIL);
                CoreGame.inst.ChangeResource(Resource.ResourceType.Bubil, productionAmount);
                CoreGame.inst.CreateIconPopUp(blobs[processId].transform.position, $"+{productionAmount}", CoreGame.inst.allResources[1].icon);
            }
        }
        else if (b.myType == Building.BuildingType.Tawa || b.myType == Building.BuildingType.Flawa)
        {
            GameObject myProjectilePfb = null;
            if (b.myType == Building.BuildingType.Tawa)
            {
                myProjectilePfb = CoreGame.inst.projectilePfb;
            }
            else
            {
                myProjectilePfb = CoreGame.inst.healingProjectilePfb;
            }
            while (true)
            {
                float shootingSpeed = GetShootingSpeed(); 
                Vector3 direction = towerAnglePerPlace[processId] switch
                {
                    0 => Vector3.right,
                    1 => Vector3.up,
                    2 => Vector3.left,
                    _ => Vector3.down
                };

                ShootProjectile(projectilePfb: myProjectilePfb,
                                projectilePosition: (Vector3)(Vector2)blobs[processId].transform.position + new Vector3(0, 0, -9),
                                direction: direction,
                                destroyTime: 2.1f,
                                rotation: Quaternion.identity,
                                doAffectBlobs: b.myType != Building.BuildingType.Flawa);
                yield return new WaitForSeconds(shootingSpeed);
            }
        }
        else if (b.myType == Building.BuildingType.Tumbo)
        {
            while (true)
            {
                float shootingSpeed = GetShootingSpeed();

                float projectileSize = GetProjectileSize();
                int damage = GetDamage();


                GameObject sliderInst = Instantiate(CoreGame.inst.sliderPfb, blobs[processId].transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                sliderInst.transform.GetChild(0).transform.DOLocalMove(new Vector3(0, 0, 0), shootingSpeed);
                sliders[processId] = sliderInst;
                yield return new WaitForSeconds(shootingSpeed);


                GameObject nearestEnemy = MaximUtils.GetNearestWithTag(t.position, CoreGame.TAG_ENEMY);

                bool waiting = false;
                while (nearestEnemy == null)
                {
                    if (!waiting)
                    {
                        sliderInst.transform.GetChild(0).GetComponent<SpriteRenderer>().color = CoreGame.YELLOW_COLOR;
                        waiting = true;
                    }
                    sliderInst.transform.DOShakePosition(shootingSpeed, 0.05f, 100, 90, false, false, ShakeRandomnessMode.Harmonic);
                    yield return new WaitForSeconds(shootingSpeed);
                    nearestEnemy = MaximUtils.GetNearestWithTag(t.position, CoreGame.TAG_ENEMY);
                }

                Destroy(sliderInst);

                Collider2D enemyCollider = nearestEnemy.GetComponent<Collider2D>();
                if (enemyCollider == null)
                {
                    Debug.LogError("EnemyDoesnt have a collider");
                }
                Vector2 enemyShootingPosition = enemyCollider.offset + (Vector2)nearestEnemy.transform.position;
                Vector3 position = (Vector3)(Vector2)blobs[processId].transform.position + new Vector3(0, 0, -9);
                Vector3 direction = (enemyShootingPosition - (Vector2)position).normalized;
                Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
                ShootProjectile(projectilePfb: CoreGame.inst.arrowProjectilePfb,
                                projectilePosition: (Vector3)(Vector2)blobs[processId].transform.position + new Vector3(0, 0, -9),
                                destroyTime: 0.75f,
                                direction: direction,
                                rotation: rotation,
                                doAffectBlobs: b.myType != Building.BuildingType.Flawa);
            }
        }
        else if (b.myType == Building.BuildingType.Custik)
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


                SoundManager.inst.PlaySfx(DataStorage.SFX_PRODUCE_BUBIL);
                CoreGame.inst.ChangeResource(Resource.ResourceType.Bubil, productionAmount);
                CoreGame.inst.CreateIconPopUp(transform.position + new Vector3(0, 1.4f, 0), $"+{productionAmount}", CoreGame.inst.allResources[1].icon);
            }
        }
        else if (b.myType == Building.BuildingType.Magno)
        {

        }
        else if (b.myType == Building.BuildingType.Bombo)
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


                SoundManager.inst.PlaySfx(DataStorage.SFX_PRODUCE_BUBIL);
                Instantiate(CoreGame.inst.bombPfb, specialPurposeCol.gameObject.transform.position + Vector3.back, Quaternion.identity);
            }
        }
    }

    private void OnMouseDown()
    {
        if (CoreGame.inst.currentCursor == CoreGame.FunctionalCursor.Basic)
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
    }

    private void ShootProjectile(GameObject projectilePfb, Vector3 projectilePosition, Vector3 direction, float destroyTime, Quaternion rotation, bool doAffectBlobs = true)
    {
        float projectileSize = GetProjectileSize();
        int damage = GetDamage();

        SoundManager.inst.PlaySfx(DataStorage.SFX_SHOOT, minPitch: 0.95f, maxPitch: 1.05f);
        GameObject inst = Instantiate(projectilePfb, projectilePosition, rotation);
        //inst.transform.localScale = new Vector3(projectileSize, projectileSize, 1);
        Projectile pr = inst.GetComponent<Projectile>();
        pr.damage = damage;
        pr.ignoreList.Add(gameObject);
        pr.size = projectileSize;
        pr.doAffectBlobs = doAffectBlobs;
        pr.direction = direction;

        pr.StartCoroutine(pr.ProjectileLifeCycle(destroyTime));
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
        if (b.myType == Building.BuildingType.Tawa)
        {
            return baseShootingSpeed - shootingSpeedLevel * 0.15f;
        }
        if (b.myType == Building.BuildingType.Tumbo)
        {
            return baseShootingSpeed - shootingSpeedLevel * 0.15f;
        }
        if (b.myType == Building.BuildingType.Flawa)
        {
            return baseShootingSpeed - shootingSpeedLevel * 0.15f;
        }
        //Unreachable
        Debug.LogError("Unreachable area of code!");
        return 0;
    }

    public int GetDamage()
    {
        if (b.myType == Building.BuildingType.Tawa)
        {
            return (projectileDamageLevel + 1) * 10;
        }
        if (b.myType == Building.BuildingType.Tumbo)
        {
            return (projectileDamageLevel + 1) * 10;
        }
        if (b.myType == Building.BuildingType.Flawa)
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
