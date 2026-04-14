using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : EnemyCreature
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform shootingPlace;
    [SerializeField] float arrorVelocity;
    [SerializeField] GameObject groupToSummon;

    float shootingDistance = 3f;

    int bulletCount = 3;


    public override void StartSimulation()
    {
        if (Random.value > 0.5f)
        {
            GameObject targetObj = CoreGame.inst.builtObjects[Random.Range(0, CoreGame.inst.builtObjects.Count)].gameObject;
            simulation = StartCoroutine(GetOnDistanceFromTarget(targetObj));
        }
        else
        {
            GameObject targetObj = CoreGame.inst.builtObjects[Random.Range(0, CoreGame.inst.builtObjects.Count)].gameObject;
            simulation = StartCoroutine(MoveToAttackTarget(targetObj));
        }
    }

    protected override IEnumerator MoveToAttackTarget(GameObject targetObj)
    {
        //Classic variation
        if (targetObj == null)
        {
            StartSimulation();
            yield break;
        }


        a.SetBool("walk", true);

        target = targetObj.GetComponent<DestructableObject>();
        Vector3 destinationDelta = MaximUtils.RandomVector2RandomMagnitudeRange(0.3f, 0.4f);

        do
        {
            yield return new WaitForFixedUpdate();

            if (targetObj == null)
            {
                StartSimulation();
                yield break;
            }

            destination = targetObj.transform.position + destinationDelta;
            destination.z = t.position.y;

            t.position = Vector3.MoveTowards(t.position, destination, Time.fixedDeltaTime * idleVelocity);
            t.position = new Vector3(t.position.x, t.position.y, t.position.y);

        } while (Vector3.Distance(t.position, destination) >= 0.25f);

        a.SetBool("walk", false);

        StartCoroutine(AttackTarget());
    }

    protected override IEnumerator AttackTarget()
    {
        if (target == null)
        {
            StartSimulation();
            yield break;
        }

        a.SetBool("attack1", false);
        a.SetBool("attack2", true);
        a.SetBool("walk", false);

        yield return new WaitForSeconds(0.15f);

        if (target == null)
        {
            StartSimulation();
            a.SetBool("attack1", false);
            a.SetBool("attack2", false);
            yield break;
        }

        

        yield return new WaitForSeconds(0.5f);

        Instantiate(groupToSummon, t.position, Quaternion.identity);
        target.ChangeHealth(-myDamage / 2);
        yield return new WaitForSeconds(0.25f);
        target.ChangeHealth(-myDamage / 2);
        yield return new WaitForSeconds(0.25f);

        a.SetBool("attack1", false);
        a.SetBool("attack2", false);
        yield return new WaitForSeconds(2.5f);


        StartCoroutine(GetOnDistanceFromTarget(target.gameObject));
    }

    protected IEnumerator GetOnDistanceFromTarget(GameObject targetObj)
    {
        a.SetBool("walk", true);

        if (targetObj == null)
        {
            a.SetBool("walk", false);
            StartSimulation();
            yield break;
        }


        Vector2 targetPosition = (Vector2)targetObj.transform.position + MaximUtils.RandomVector2FixMagnitude(2f) + MaximUtils.RandomVector2(0.25f);
        //We are attemopting to get the correct target position
        int attempt = 0;
        while (MaximUtils.GetNearestOverlappedWithTag2D(targetPosition, 1f, CoreGame.TAG_BUILDING) != null && attempt < 100)
        {
            targetPosition = (Vector2)targetObj.transform.position + MaximUtils.RandomVector2FixMagnitude(2f) + MaximUtils.RandomVector2(0.25f);
            ++attempt;
        }
        if (attempt >= 100)
        {
            StartSimulation();
            yield break;
        }


        do
        {
            yield return new WaitForFixedUpdate();

            if (targetObj == null)
            {
                StartSimulation();
                yield break;
            }

            t.position = Vector2.MoveTowards(t.position, targetPosition, Time.fixedDeltaTime * idleVelocity);
            t.position = new Vector3(t.position.x, t.position.y, t.position.y);

        } while (Vector2.Distance(t.position, targetPosition) >= 0.1f);

        a.SetBool("walk", false);

        StartCoroutine(ShootInTarget(targetObj));
    }

    protected IEnumerator ShootInTarget(GameObject targetObj)
    {   
        if (targetObj == null)
        {
            a.SetBool("attack1", false);
            a.SetBool("attack2", false);
            StartSimulation();
            yield break;
        }

        a.SetBool("attack1", true);
        a.SetBool("attack2", false);

        for (int i = 0; i < bulletCount; ++i)
        {
            GameObject projectileInst = Instantiate(projectilePrefab, shootingPlace.position + (Vector3)MaximUtils.RandomVector2(0.1f), Quaternion.identity);
            Projectile projectile = projectileInst.GetComponent<Projectile>();
            Vector2 directionToTargetNormalized = ((Vector2)targetObj.transform.position - (Vector2)t.position).normalized;
            Vector2 projectileDirection = (directionToTargetNormalized + MaximUtils.RandomVector2FixMagnitude(0.3f)).normalized;
            projectile.direction = (Vector3)projectileDirection;
            projectile.damage = myDamage;
            projectile.velocity = arrorVelocity;
            Destroy(projectileInst, 1.2f);

            yield return new WaitForSeconds(0.15f);
        }

        yield return new WaitForSeconds(0.3f);
        a.SetBool("attack1", false);
        a.SetBool("attack2", false);
        a.SetBool("walk", false);
        // Pause and stay

        yield return new WaitForSeconds(3);


        StartSimulation();
    }
}
