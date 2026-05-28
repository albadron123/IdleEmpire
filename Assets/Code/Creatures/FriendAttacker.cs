using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendAttacker : FriendCreature
{
    [SerializeField] float viewDistance = 4;

    private Collider2D col;

    protected override bool DoSpecialActionCheck()
    {
        col = MaximUtils.GetNearestOverlappedWithTag2D(t.position, viewDistance, CoreGame.TAG_ENEMY);
        return (col != null);
    }

    protected override IEnumerator DoSpecialAction()
    {
        yield return MoveToAttackTarget(col.gameObject);
    }
}
