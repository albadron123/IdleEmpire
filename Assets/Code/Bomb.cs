using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour, IDragInteraction
{
    [SerializeField] GameObject dirtPfb;
    [SerializeField] GameObject explostionPfb;

    [SerializeField]
    int damage = 4;

    Collider2D myCol;

    bool canExplode = false;
    
    void Start()
    {
        myCol = GetComponent<Collider2D>();
        GetComponent<DragObject>().dragInteraction = this;
    }

    public void StartDrag()
    {
        canExplode = false;
    }

    public void FinishDrag()
    {
        canExplode = true;
    }

    
    void Update()
    {
        if (canExplode)
        {
            List<Collider2D> cols = new List<Collider2D>();
            Physics2D.OverlapCollider(myCol, new ContactFilter2D().NoFilter(), cols);
            foreach (Collider2D col in cols)
            {
                if (col.gameObject.tag == CoreGame.TAG_ENEMY)
                {
                    //Explode
                    Collider2D[] affected = Physics2D.OverlapCircleAll(transform.position, 1.5f);
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
                    Destroy(gameObject);
                    break;
                }
            }
        }
    }

   
}
