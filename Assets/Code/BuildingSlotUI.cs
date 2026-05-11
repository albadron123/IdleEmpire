using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

class BuildingSlotUI : MonoBehaviour
{

    Transform t;
    Vector3 basicScale;
    Collider2D col;

    bool prevOverlapped = false;

    private void Start()
    {
        t = transform;
        col = GetComponent<Collider2D>();
        basicScale = t.localScale;
    }

    private void Update()
    {
        bool overlapped = MaximUtils.MouseOverCollider(col, 0.1f);
        if (!prevOverlapped && overlapped)
        {
            if (CoreGame.inst.draggedObject != null && CoreGame.inst.draggedObject.gameObject.GetComponent<FriendCreature>() != null)
            {
                t.DOScale(basicScale * 1.5f, 0.2f);
            }
        }
        if (prevOverlapped && !overlapped)
        {
            t.DOScale(basicScale, 0.2f);
        }
        prevOverlapped = overlapped;
    }
}

