using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendCreature : Creature
{

    public override void StartSimulation()
    {
        Collider2D col = MaximUtils.GetNearestOverlappedWithTag2D(t.position, 3, CoreGame.TAG_ENEMY);
        if (col != null)
        {
            simulation =  StartCoroutine(MoveToAttackTarget(col.gameObject));
        }
        else
        {
            simulation = StartCoroutine(IdleWalking());
        }
    }
    

    protected override IEnumerator IdleWalking()
    {

        while (true)
        {

            destination = t.position + (Vector3)MaximUtils.RandomVector2(1.5f);
            destination = new Vector3(Mathf.Clamp(destination.x, -5, 8), Mathf.Clamp(destination.y, -4.5f, 4.5f), destination.z);
            a.SetBool("walk", true);

            do
            {
                t.position = Vector3.MoveTowards(t.position, destination, Time.fixedDeltaTime * idleVelocity);
                t.position = new Vector3(t.position.x, t.position.y, t.position.y);
                destination.z = t.position.y;
                
                // Check for nearestEnemies to attack :                
                Collider2D col = MaximUtils.GetNearestOverlappedWithTag2D(t.position, 3, CoreGame.TAG_ENEMY);
                if (col != null)
                {
                    StartCoroutine(MoveToAttackTarget(col.gameObject));
                    yield break;
                }

                yield return new WaitForFixedUpdate();
            } 
            while (Vector3.Distance(t.position, destination) >= 0.05f);

            a.SetBool("walk", false);
            yield return new WaitForSeconds(Random.Range(0.9f, 2f));
        }

    }

}
