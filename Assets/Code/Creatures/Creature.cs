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
    public enum CreatureType
    {
        normal,
        tinyEnemy
    };


    protected Animator a;
    protected Transform t;
    protected Coroutine simulation = null;


    [SerializeField]
    protected CreatureType myType;
    [SerializeField]
    protected float idleVelocity = 1f;
    [SerializeField]
    protected float activeVelocity = 1.5f;
    [SerializeField]
    protected int myDamage = 1;

    protected Vector3 destination;
    protected DestructableObject target;


    protected virtual void Start()
    {
        t = transform;
        a = GetComponent<Animator>();
        Invoke("StartSimulation", 0.05f);
    }

    
    protected virtual void Update()
    {
        //Right nothing
    }

    

    public virtual void ChangeHealth(int damage)
    {
        //nothing
    }


    public virtual void Die()
    {
        //nothing
    }

    public virtual void StartSimulation()
    {
        // kind of abstract 
    }

    public virtual void StopSimulation()
    {
        if (simulation != null)
        {
            StopAllCoroutines();
            
            simulation = null;
            a.SetBool("walk", false);
            a.SetBool("attack", false);
        }  
    }
    
    protected virtual IEnumerator IdleWalking()
    {
        //Classic variation

        while (true)
        {
            destination = t.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0).normalized * Random.Range(-1.5f, 1.5f);
            destination = new Vector3(Mathf.Clamp(destination.x, -5, 8), Mathf.Clamp(destination.y, -4.5f, 4.5f), destination.z);
            a.SetBool("walk", true);

            do
            {
                t.position = Vector3.MoveTowards(t.position, destination, Time.fixedDeltaTime * idleVelocity);
                t.position = new Vector3(t.position.x, t.position.y, t.position.y);
                destination.z = t.position.y;
                yield return new WaitForFixedUpdate();
            } 
            while (Vector3.Distance(t.position, destination) >= 0.05f);

            a.SetBool("walk", false);
            yield return new WaitForSeconds(Random.Range(0.9f, 2f));

        }
            
    }

    protected virtual IEnumerator MoveToAttackTarget(GameObject targetObj)
    {
        //Classic variation
        if (targetObj == null)
        {
            StartCoroutine(IdleWalking());
            yield break;
        }


        a.SetBool("walk", true);
        
        target = targetObj.GetComponent<DestructableObject>();
        Vector3 destinationDelta = MaximUtils.RandomVector2(0.5f);

        do
        {
            yield return new WaitForFixedUpdate();

            if (targetObj == null)
            {
                StartCoroutine(IdleWalking());
                yield break;
            }

            destination = targetObj.transform.position + destinationDelta;
            destination.z = t.position.y;

            t.position = Vector3.MoveTowards(t.position, destination, Time.fixedDeltaTime * idleVelocity);
            t.position = new Vector3(t.position.x, t.position.y, t.position.y);

        } while (Vector3.Distance(t.position, destination) >= 0.25f);
        a.SetBool("walk", false);

        StartCoroutine(AttackTarget());
        yield break;

    }

    protected virtual IEnumerator AttackTarget()
    {
        // Classic variation
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
            
        target.ChangeHealth(-myDamage);

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
