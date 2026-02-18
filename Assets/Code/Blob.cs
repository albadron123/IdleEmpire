using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blob : MonoBehaviour, IDragInteraction
{
    Transform t;

    public float intialZPosition;

    Collider2D myCollider;


    BuildingObject currentBuilding = null;
    GameObject currentPlace = null;


    Creature meCreature;




    void Start()
    {
        t = transform;
        intialZPosition = t.position.z;
        myCollider = GetComponent<Collider2D>();
        meCreature = GetComponent<Creature>();
        GetComponent<DragObject>().dragInteraction = this;
    }

    void Update()
    {
        
    }

    public void StartDrag()
    {
        meCreature.StopSimulation();

        if (currentBuilding != null)
        {
            CoreGame.inst.canBuild = false;    

            currentBuilding.RemoveBlob(this);
            currentPlace.GetComponent<SpriteRenderer>().enabled = true;
            currentPlace = null;
            currentBuilding = null;


            DestructableObject dObj = GetComponent<DestructableObject>();
            if (dObj.health < dObj.maxHealth)
            {
                dObj.sliderContainer.SetActive(true);
            }
        }
    }

    public void FinishDrag()
    {
        CoreGame.inst.canBuild = true;

        GameObject foundPlace = null;
        List<Collider2D> results = new List<Collider2D>();
        Physics2D.OverlapCollider(myCollider, new ContactFilter2D().NoFilter(), results);
        foreach(Collider2D res in results)
        {
            if (res.gameObject.tag == CoreGame.TAG_BLOB_PLACE)
            {
                foundPlace = res.gameObject;
                break;
            }
        }

        if (foundPlace != null)
        {
            foundPlace.GetComponent<SpriteRenderer>().enabled = false;
            t.position = foundPlace.transform.position + new Vector3(0, 0.25f, -1);
            currentPlace = foundPlace;
            currentBuilding = currentPlace.transform.parent.GetComponent<BuildingObject>();
            currentBuilding.AddBlob(this);
            GetComponent<DestructableObject>().sliderContainer.SetActive(false);
        }
        else
        {
            t.position = new Vector3(t.position.x, t.position.y, intialZPosition);
            meCreature.StartSimulation();
        }
    }
}
