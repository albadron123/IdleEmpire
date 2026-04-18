using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour, IDragInteraction
{
    [SerializeField] GameObject dirtPfb;
    [SerializeField] GameObject explostionPfb;

    LineRenderer[] lrs;

    [SerializeField]
    int damage = 4;

    Collider2D myCol;

    bool canExplode = false;
    bool wasActive = false;
    
    void Start()
    {
        myCol = GetComponent<Collider2D>();
        GetComponent<DragObject>().dragInteraction = this;

        lrs = MaximUtils.CreateLineRendererBatch("BOMB_OUTLINE", 17, new Color(0.3f, 0.3f, 0.3f, 0.8f), CoreGame.inst.spriteDefaultMaterial, 0.08f, "Default");
    }

    public void StartDrag()
    {
        if (wasActive == false)
        {
            lrs = MaximUtils.CreateLineRendererBatch("BOMB_OUTLINE", 17, new Color(0.3f, 0.3f, 0.3f, 0.8f), CoreGame.inst.spriteDefaultMaterial, 0.08f, "Default");
        }
        wasActive = true;

        canExplode = false;
    }

    public void FinishDrag()
    {
        canExplode = true;
        GetComponent<DragObject>().shadow.SetActive(true);
    }


    void Update()
    {
        if (wasActive)
        {
            MaximUtils.RenderDashedCircle(lrs, transform.position + Vector3.forward * 10, CoreGame.bombRadius, 0.6f * Time.time, 16);
        }
        if (canExplode)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);

            List<Collider2D> cols = new List<Collider2D>();
            Physics2D.OverlapCollider(myCol, new ContactFilter2D().NoFilter(), cols);
            foreach (Collider2D col in cols)
            {
                if (col.gameObject.tag == CoreGame.TAG_ENEMY)
                {
                    //Explode
                    Collider2D[] affected = Physics2D.OverlapCircleAll(transform.position, CoreGame.bombRadius);
                    for (int i = 0; i < affected.Length; ++i)
                    {
                        if (affected[i].gameObject.tag == CoreGame.TAG_ENEMY || affected[i].gameObject.tag == CoreGame.TAG_BUILDING)
                        {
                            affected[i].gameObject.GetComponent<DestructableObject>().ChangeHealth(-damage);
                        }
                    }
                    GameObject inst = Instantiate(explostionPfb, transform.position, Quaternion.identity);
                    Destroy(inst, 0.3f);
                    Instantiate(dirtPfb, new Vector3(transform.position.x, transform.position.y, 70), Quaternion.identity);
                    Destroy(lrs[0].transform.parent.gameObject);
                    Destroy(gameObject);
                    break;
                }
            }
        }
    }

   
}
