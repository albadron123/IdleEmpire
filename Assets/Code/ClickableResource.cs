using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ClickableResource : MonoBehaviour
{
    public Resource.ResourceType type;


    bool clicked = false;
    Transform t;


    void Start()
    {
        t = transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Click()
    {
        if (!clicked)
        {
            clicked = true;
            CoreGame.inst.ChangeResource(type, 1);
            CoreGame.inst.CreateIconPopUp(t.position, "+1", CoreGame.inst.allResources[(int)type].icon);
            t.DOScale(0, 1);
            Destroy(gameObject, 1);
        }
    }
}
