using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blob : MonoBehaviour, IDragInteraction
{
    Transform t;

    public float initialZPosition;

    Collider2D myCollider;


    BuildingObject currentBuilding = null;
    GameObject currentPlace = null;


    Creature meCreature;

    [SerializeField]
    GameObject outline;





    void Start()
    {
        t = transform;
        initialZPosition = t.position.z;
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
        outline.SetActive(true);

        if (currentBuilding != null)
        {
            CoreGame.inst.canBuild = false;    

            currentBuilding.RemoveBlob(this, currentPlace);
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


        /*
        List<Collider2D> results = new List<Collider2D>();
        
        GameObject foundPlace = null;
        
        Physics2D.OverlapCollider(myCollider, new ContactFilter2D().NoFilter(), results);
        foreach(Collider2D res in results)
        {
            if (res.gameObject.tag == CoreGame.TAG_BLOB_PLACE)
            {
                foundPlace = res.gameObject;
                break;
            }
        }
        */

        Collider2D foundPlaceCol = MaximUtils.GetNearestOverlappedWithTag2D(myCollider, CoreGame.TAG_BLOB_PLACE);
        GameObject foundPlace = null;
        if (foundPlaceCol != null)
        {
            foundPlace = foundPlaceCol.gameObject;    
        }

        if (foundPlace != null)
        {
            SpriteRenderer foundPlaceSr = foundPlace.GetComponent<SpriteRenderer>();
            if (foundPlaceSr.enabled)
            {
                foundPlace.GetComponent<SpriteRenderer>().enabled = false;
                t.position = foundPlace.transform.position + new Vector3(0, 0.25f, -1);
                currentPlace = foundPlace;
                currentBuilding = currentPlace.transform.parent.GetComponent<BuildingObject>();
                currentBuilding.AddBlob(this, currentPlace);
                GetComponent<DestructableObject>().sliderContainer.SetActive(false);
            }
            else
            {
                //Make a displacement
                t.position = new Vector3(t.position.x + Random.Range(-0.25f, 0.25f), t.position.y + Random.Range(-0.25f, 0.25f), initialZPosition);
                meCreature.StartSimulation();
            }
        }
        else
        {
            t.position = new Vector3(t.position.x, t.position.y, initialZPosition);
            meCreature.StartSimulation();
        }

        outline.SetActive(false);
    }
}
