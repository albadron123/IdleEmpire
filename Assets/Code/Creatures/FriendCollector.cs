using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendCollector : FriendCreature
{
    [SerializeField] float viewDistance = 4;

    private Collider2D col;

    protected override bool DoSpecialActionCheck()
    {
        col = MaximUtils.GetNearestOverlappedWithTag2D(t.position, viewDistance, CoreGame.TAG_CLICKABLE_RESOURCE);
        return (col != null);
    }

    protected override IEnumerator DoSpecialAction()
    {
        // === Go and collect the resource ===

        GameObject targetObj = col.gameObject;

        if (targetObj == null)
        {
            simulation = StartCoroutine(IdleWalking());
            yield break;
        }

        a.SetBool("walk", true);

        target = targetObj.GetComponent<DestructableObject>();

        do
        {
            yield return new WaitForFixedUpdate();

            if (targetObj == null)
            {
                ChangeState();
                yield break;
            }

            destination = targetObj.transform.position;
            destination.z = t.position.y;

            t.position = Vector3.MoveTowards(t.position, destination, CoreGame.joniVelocity > 0? Time.fixedDeltaTime * CoreGame.joniVelocity : 1);
            t.position = new Vector3(t.position.x, t.position.y, t.position.y);

        } while (Vector3.Distance(t.position, destination) >= 0.05f);
        a.SetBool("walk", false);

        a.SetBool("attack", true);

        Resource.ResourceType type = targetObj.GetComponent<ClickableResource>().type;
        CoreGame.inst.ChangeResource(type, CoreGame.joniTechnique);
        CoreGame.inst.CreateIconPopUp(t.position, $"+{CoreGame.joniTechnique}", CoreGame.inst.allResources[(int)type].icon);

        Destroy(targetObj.gameObject);

        yield return new WaitForSeconds(0.5f);

        a.SetBool("attack", false);

        ChangeState();
        yield break;
    }
}
