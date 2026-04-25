using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCreature : Creature
{
    [SerializeField]
    int rewardBones;
    [SerializeField]
    int rewardBonesCritical;
    [SerializeField]
    float criticalChance = 0.15f;

    [SerializeField]
    EnemyHandle myHandle;

    DestructableObject dObj;

    protected override void Start()
    {
        base.Start();
        rewardBones = DataStorage.allEnemies[(int)myHandle].bonesReward;
        rewardBonesCritical = DataStorage.allEnemies[(int)myHandle].bonesRewardCritial;
        myDamage = DataStorage.allEnemies[(int)myHandle].simpleDamage;
        dObj = GetComponent<DestructableObject>();
        dObj.maxHealth = DataStorage.allEnemies[(int)myHandle].maxHealth;
        //as we've changed the basic health, we re-init it (probably multiple times, though it's okey, but it is nesessary to do so to apply the maxHealth change
        dObj.InitHealth();
    }

    protected override void Update()
    {
        base.Update();

        List<Collider2D> cols = new List<Collider2D>();
        Physics2D.OverlapCollider(GetComponent<Collider2D>(), new ContactFilter2D().NoFilter(), cols);
        foreach (Collider2D col in cols)
        {
            if (col.gameObject.tag == CoreGame.TAG_PROJECTILE)
            {
                int damage = col.gameObject.GetComponent<Projectile>().damage;
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

    public override void ChangeHealth(int damage)
    {
        base.ChangeHealth(damage);
        if (damage < 0)
        {
            CoreGame.inst.CreateIconPopUp((Vector2)t.position + new Vector2(0.5f, 0f), $"{damage} hp".Color("red").Bold().Size(40), null, 0.7f);
        }
    }

    public override void Die()
    {
        base.Die();
        int reward = Mathf.CeilToInt(rewardBones * CoreGame.bonesBonusMultiplier);
        if (Random.value < criticalChance)
        {
            reward = Mathf.CeilToInt(rewardBonesCritical * CoreGame.bonesBonusMultiplier);
            CoreGame.inst.CreateIconPopUp(t.position, $"{"CRITICAL!".Size(50)}\n+{reward}", CoreGame.inst.allResources[2].icon);
        }
        else
        {
            CoreGame.inst.CreateIconPopUp(t.position, $"+{reward}", CoreGame.inst.allResources[2].icon);
        }
        CoreGame.inst.ChangeResource(Resource.ResourceType.Bones, reward);
    }

    public override void StartSimulation()
    {
        
        GameObject targetObj = CoreGame.inst.builtObjects.Find(x => x.b.myType == Building.BuildingType.HutkaGrande).gameObject;
        simulation = StartCoroutine(MoveToAttackTarget(targetObj));
        
    }
}
