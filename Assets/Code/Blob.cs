using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Blob : MonoBehaviour, IDragInteraction
{
    Transform t;

    public float initialZPosition;

    Collider2D myCollider;

    BuildingObject currentBuilding = null;
    //GameObject currentPlace = null;
    

    [SerializeField]
    Creature meCreature;

    [SerializeField]
    GameObject outline;

    public bool isBobby = false;





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

            currentBuilding.RemoveBlob(this);
            
            UnregisterFromBuilding();
        }
    }

    public void UnregisterFromBuilding()
    {
        currentBuilding = null;
    }

    public void RegisterOnBuilding(BuildingObject bo)
    {
        currentBuilding = bo;
    }


    public void FinishDrag()
    {
        

        CoreGame.inst.canBuild = true;

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
                foundPlace.transform.parent.GetComponent<BuildingObject>()?.AddBlob(this, foundPlace);
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
            if (GetComponent<DragObject>().shadow != null)
            {
                GetComponent<DragObject>().shadow.SetActive(true);
            }
        }

        outline.SetActive(false);
    }
}
