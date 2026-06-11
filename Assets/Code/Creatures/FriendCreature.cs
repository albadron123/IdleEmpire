using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FriendCreature : Creature
{

    public override void StartSimulation()
    {
        ChangeState();
    }

    protected virtual void ChangeState()
    {
        if (DoSpecialActionCheck())
        {
            StopAllCoroutines();
            simulation = StartCoroutine(DoSpecialAction());
        }
        else
        {
            StopAllCoroutines();
            simulation = StartCoroutine(IdleWalking());
        }
    }


    protected override void Update()
    {
        base.Update();
        
        List<Collider2D> cols = new List<Collider2D>();
        Physics2D.OverlapCollider(GetComponent<Collider2D>(), new ContactFilter2D().NoFilter(), cols);
        foreach (Collider2D col in cols)
        {
            if (col.gameObject.tag == CoreGame.TAG_ENEMY_PROJECTILE)
            {
                Projectile proj = col.gameObject.GetComponent<Projectile>();
                if (proj.doAffectBlobs && !proj.ignoreList.Contains(gameObject))
                {
                    int damage = proj.damage;
                    Destroy(col.gameObject);
                }
            }
        }
        
    }

    private void OnMouseExit()
    {
        foreach (LineRenderer lr in CoreGame.inst.specialLrs2)
        {
            lr.positionCount = 0;
        }
    }

    private void OnMouseOver()
    {
        MaximUtils.RenderDashedCircle(CoreGame.inst.specialLrs2, t.position, 0.55f, Time.time, 7);
    }


    protected virtual bool DoSpecialActionCheck()
    {
        return false;
    }

    protected virtual IEnumerator DoSpecialAction()
    {
        yield break;
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

                if (DoSpecialActionCheck())
                {
                    StartCoroutine(DoSpecialAction());
                    yield break;
                }

                yield return new WaitForFixedUpdate();
            } 
            while (Vector3.Distance(t.position, destination) >= 0.05f);

            a.SetBool("walk", false);
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.9f, 2f));
        }

    }

}
