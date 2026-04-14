using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherEnemy : EnemyCreature
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform shootingPlace;
    [SerializeField] float arrorVelocity;

    float shootingDistance = 3f;


    public override void StartSimulation()
    {
        GameObject targetObj = CoreGame.inst.builtObjects[Random.Range(0,CoreGame.inst.builtObjects.Count)].gameObject;
        simulation = StartCoroutine(GetOnDistanceFromTarget(targetObj));
    }

    protected IEnumerator GetOnDistanceFromTarget(GameObject targetObj)
    {
        a.SetBool("walk", true);

        if(targetObj == null)
        {
            targetObj = CoreGame.inst.builtObjects[Random.Range(0, CoreGame.inst.builtObjects.Count)].gameObject;
        }

        
        Vector2 targetPosition = (Vector2)targetObj.transform.position + MaximUtils.RandomVector2FixMagnitude(2f) + MaximUtils.RandomVector2(0.25f);
        //We are attemopting to get the correct target position
        int attempt = 0;
        while(MaximUtils.GetNearestOverlappedWithTag2D(targetPosition, 0.4f, CoreGame.TAG_BUILDING) != null && attempt < 100)
        {
            targetPosition = (Vector2)targetObj.transform.position + MaximUtils.RandomVector2FixMagnitude(2f) + MaximUtils.RandomVector2(0.25f);
            ++attempt;
        }
        if(attempt >= 100)
        {
            targetObj = CoreGame.inst.builtObjects[Random.Range(0, CoreGame.inst.builtObjects.Count)].gameObject;
            StartCoroutine(GetOnDistanceFromTarget(targetObj));
            yield break;
        }


        do
        {
            yield return new WaitForFixedUpdate();

            if (targetObj == null)
            {
                targetObj = CoreGame.inst.builtObjects[Random.Range(0, CoreGame.inst.builtObjects.Count)].gameObject;
                StartCoroutine(GetOnDistanceFromTarget(targetObj));
                yield break;
            }

            t.position = Vector2.MoveTowards(t.position, targetPosition, Time.fixedDeltaTime * activeVelocity);
            t.position = new Vector3(t.position.x, t.position.y, t.position.y);

        } while (Vector2.Distance(t.position, targetPosition) >= 0.1f);

        a.SetBool("walk", false);
        
        StartCoroutine(ShootInTarget(targetObj));
    }

    protected IEnumerator ShootInTarget(GameObject targetObj)
    {
        //For now let the archers just to shoot from one point most of the time
        while (Random.value < 0.8f)
        {
            if (targetObj == null)
            {
                a.SetBool("attack", false);
                targetObj = CoreGame.inst.builtObjects[Random.Range(0, CoreGame.inst.builtObjects.Count)].gameObject;
                StartCoroutine(GetOnDistanceFromTarget(targetObj));
                yield break;
            }

            a.SetBool("attack", true);
            GameObject projectileInst = Instantiate(projectilePrefab, shootingPlace.position, Quaternion.identity);
            Projectile projectile = projectileInst.GetComponent<Projectile>();
            projectile.direction =  (Vector3)((Vector2)targetObj.transform.position - (Vector2)t.position);
            projectile.damage = myDamage;
            projectile.velocity = arrorVelocity;
            Destroy(projectileInst,1.2f);

            yield return new WaitForSeconds(0.3f);
            a.SetBool("attack", false);
            a.SetBool("walk", false);
            // Pause and stay

            yield return new WaitForSeconds(3);
            
        }

        if (targetObj == null)
        {
            targetObj = CoreGame.inst.builtObjects[Random.Range(0, CoreGame.inst.builtObjects.Count)].gameObject;
            StartCoroutine(GetOnDistanceFromTarget(targetObj));
            yield break;
        }

        StartCoroutine(GetOnDistanceFromTarget(targetObj));
    }

}
