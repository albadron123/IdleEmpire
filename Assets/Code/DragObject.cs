using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public interface IDragInteraction
{
    public void FinishDrag();
    public void StartDrag();
}

public class DragObject : MonoBehaviour
{
    
    public IDragInteraction dragInteraction;

    public GameObject shadow = null;

    private void Start()
    {

    }


    private void OnMouseDown()
    {
        if (CoreGame.inst.currentCursor != CoreGame.FunctionalCursor.Basic)
        {
            return;
        }
        
        if (CoreGame.inst.canDrag)
        {
            if (CoreGame.inst.draggedObject == null)
            {
                CoreGame.inst.draggedObject = this;
                if (dragInteraction != null)
                {
                    dragInteraction.StartDrag();
                    if (shadow != null)
                    {
                        shadow.SetActive(false);
                    }
                }
            }
        }
    }

    private void OnMouseUp()
    {
        if (CoreGame.inst.draggedObject == this)
        {
            CoreGame.inst.draggedObject = null;

            if (dragInteraction != null)
            {
                //Finish dragging
                dragInteraction.FinishDrag();
            }
        }
    }
}
