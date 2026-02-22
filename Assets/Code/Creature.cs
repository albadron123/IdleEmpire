using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDestructable
{
    
    public void ChangeHealth(int damage);
    public void Die();
}

public class Creature : MonoBehaviour, IDestructable
{
    Animator a;
    Transform t;
    Coroutine simulation = null;

    enum CreatureType
    {
        normal,
        tinyEnemy
    };

    [SerializeField]
    CreatureType myType;
    
    
    void Start()
    {
        t = transform;
        a = GetComponent<Animator>();
        Invoke("StartSimulation", 0.1f);
    }

    
    void Update()
    {
        if (myType == CreatureType.tinyEnemy)
        {
            //Hit by projectiles
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
    }

    

    public void ChangeHealth(int damage)
    {
        if (myType == CreatureType.tinyEnemy)
        {
            if (damage < 0)
            {
                CoreGame.inst.CreateIconPopUp((Vector2)t.position + new Vector2(0.5f, 0f), $"{damage} hp".Color("red").Bold(), null);
            }
        }
    }


    public void Die()
    {
        if (myType == CreatureType.tinyEnemy)
        {
            CoreGame.inst.CreateIconPopUp(t.position, "+1", CoreGame.inst.allResources[2].icon);
            CoreGame.inst.ChangeResource(Resource.ResourceType.bones, 1);
        }
    }

    public void StartSimulation()
    {
        if (myType == CreatureType.tinyEnemy)
        {
            GameObject targetObj = CoreGame.inst.builtObjects.Find(x => x.b.myType == Building.BuildingType.MajorTower).gameObject;
            simulation = StartCoroutine(MoveToAttackTarget(targetObj));
        }
        else
        {
            simulation = StartCoroutine(IdleWalking());
        }
    }

    public void StopSimulation()
    {
        if (simulation != null)
        {
            StopAllCoroutines();
            
            simulation = null;
            a.SetBool("walk", false);
            a.SetBool("attack", false);
        }  
    }


    float idleVelocity = 1f;
    [SerializeField] 
    int damage = 1;
    Vector3 destination;
    DestructableObject target;
    IEnumerator IdleWalking()
    {
        while (true)
        {
            destination = t.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0).normalized * Random.Range(-1.5f, 1.5f);
            destination = new Vector3(Mathf.Clamp(destination.x, -5, 8), Mathf.Clamp(destination.y, -4.5f, 4.5f), destination.z);
            a.SetBool("walk", true);
            do
            {
                yield return new WaitForFixedUpdate();
                t.position = Vector3.MoveTowards(t.position, destination, Time.fixedDeltaTime * idleVelocity);
                t.position = new Vector3(t.position.x, t.position.y, t.position.y);
                destination.z = t.position.y;
                // Check for collision

                // Check for nearestEnemies to exit
                if (myType == CreatureType.normal)
                {
                    GameObject target = null;
                    Collider2D[] cols = Physics2D.OverlapCircleAll(t.position, 3f);
                    foreach (Collider2D col in cols)
                    {
                        if (col.gameObject.tag == CoreGame.TAG_ENEMY)
                        {
                            target = col.gameObject;
                            break;
                        }
                    }
                    if (target != null)
                    {
                        StartCoroutine(MoveToAttackTarget(target));
                        yield break;
                    }
                }
                
            } while (Vector3.Distance(t.position, destination) >= 0.05f);
            a.SetBool("walk", false);
            yield return new WaitForSeconds(Random.Range(0.9f, 2f));
        }
            
    }

    IEnumerator MoveToAttackTarget(GameObject targetObj)
    {
        yield return new WaitForSeconds(0.1f);

        if (targetObj == null)
        {
            StartCoroutine(IdleWalking());
            yield break;
        }

        target = targetObj.GetComponent<DestructableObject>();
        //destination = new Vector3(Mathf.Clamp(destination.x, -5, 8), Mathf.Clamp(destination.y, -4.5f, 4.5f), destination.z);
        a.SetBool("walk", true);
        do
        {
            yield return new WaitForFixedUpdate();
            t.position = Vector3.MoveTowards(t.position, destination, Time.fixedDeltaTime * idleVelocity);
            t.position = new Vector3(t.position.x, t.position.y, t.position.y);
            if (targetObj != null)
            {
                destination = targetObj.transform.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0).normalized * Random.Range(-0.5f, 0.5f);
                destination.z = t.position.y;
            }
            else
            {
                StartCoroutine(IdleWalking());
                yield break;
            }
            // Check for collision

            // Check for nearestEnemies to exit
        } while (Vector3.Distance(t.position, destination) >= 0.25f);
        a.SetBool("walk", false);

        StartCoroutine(AttackTarget());
        yield break;

    }

    
    IEnumerator AttackTarget()
    {
            if (target == null)
            {
                StartCoroutine(IdleWalking());
                yield break;
            }

            a.SetBool("attack", true);
            a.SetBool("walk", false);

            yield return new WaitForSeconds(0.15f);

            if (target == null)
            {
                StartCoroutine(IdleWalking());
                yield break;
            }
            
            target.ChangeHealth(-damage);

            yield return new WaitForSeconds(1);
            
            a.SetBool("attack", false);

            if (target == null)
            {
                StartCoroutine(IdleWalking());
                yield break;
            
            }

            StartCoroutine(MoveToAttackTarget(target.gameObject));
            yield break;
    }



}
