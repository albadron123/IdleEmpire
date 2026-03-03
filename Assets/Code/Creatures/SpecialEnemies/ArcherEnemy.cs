using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherEnemy : EnemyCreature
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float arrorVelocity;

    float shootingDistance = 3f;

    public override void StartSimulation()
    {
        GameObject targetObj = CoreGame.inst.builtObjects[Random.Range(0,CoreGame.inst.builtObjects.Count)].gameObject;
        simulation = StartCoroutine(GetOnDistanceFromTarget(targetObj));
    }

    protected IEnumerator GetOnDistanceFromTarget(GameObject targetObj)
    {
        if(targetObj == null)
        {
            targetObj = CoreGame.inst.builtObjects[Random.Range(0, CoreGame.inst.builtObjects.Count)].gameObject;
        }
        Vector2 targetPosition = (Vector2)targetObj.transform.position + MaximUtils.RandomVector2FixMagnitude(2f) + MaximUtils.RandomVector2(0.25f);

        a.SetBool("walk", true);
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
            float d = Vector2.Distance(t.position, targetPosition);
        } while (Vector2.Distance(t.position, targetPosition) >= 0.25f);

        a.SetBool("walk", false);
        
        StartCoroutine(ShootInTarget(targetObj));
    }

    protected IEnumerator ShootInTarget(GameObject targetObj)
    {
        if (targetObj == null)
        {
            targetObj = CoreGame.inst.builtObjects[Random.Range(0, CoreGame.inst.builtObjects.Count)].gameObject;
            StartCoroutine(GetOnDistanceFromTarget(targetObj));
        }
        //Shoot projectile 
        a.SetBool("attack", true);
        GameObject projectileInst = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectile = projectileInst.GetComponent<Projectile>();
        projectile.direction =  (Vector3)((Vector2)targetObj.transform.position - (Vector2)t.position);
        projectile.damage = myDamage;
        projectile.velocity = arrorVelocity;
        Destroy(projectileInst,1.2f);
        // TODO: get rid of this magic constant later !!! (and of any magic constants in the code where it is in WaitForSeconds & replace with actual animation velocities
        yield return new WaitForSeconds(0.3f);

        if (targetObj == null)
        {
            targetObj = CoreGame.inst.builtObjects[Random.Range(0, CoreGame.inst.builtObjects.Count)].gameObject;
            StartCoroutine(GetOnDistanceFromTarget(targetObj));
            a.SetBool("attack", false);
        }
        StartCoroutine(GetOnDistanceFromTarget(targetObj));
    }

}
