using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonerEnemy : EnemyCreature
{

    [SerializeField]
    GameObject groupToSummon;


    public override void StartSimulation()
    {
        simulation = StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        a.SetBool("walk", true);

        Vector2 targetPos = new Vector2(Random.Range(-4.0f, 4.0f), Random.Range(-4.0f, 4.0f));

        do
        {
            yield return new WaitForFixedUpdate();

            t.position = Vector3.MoveTowards(t.position, targetPos, Time.fixedDeltaTime * idleVelocity);
            t.position = new Vector3(t.position.x, t.position.y, t.position.y);

        } while (Vector2.Distance(t.position, targetPos) >= 0.3f);


        a.SetBool("walk", false);
        a.SetBool("attack", false);

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(Summon());
    }

    private IEnumerator Summon()
    {
        a.SetBool("walk", false);
        a.SetBool("attack", true);

        yield return new WaitForSeconds(0.5f);

        Instantiate(groupToSummon, t.position, Quaternion.identity);

        yield return new WaitForSeconds(0.5f);

        a.SetBool("attack", false);

        yield return new WaitForSeconds(3f);

        StartCoroutine(Move());
    }
}
