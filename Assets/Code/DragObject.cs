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

    


    private void OnMouseDown()
    {
        if (CoreGame.inst.canDrag)
        {
            if (CoreGame.inst.draggedObject == null)
            {
                CoreGame.inst.draggedObject = this;
                dragInteraction.StartDrag();
            }
        }
    }

    private void OnMouseUp()
    {
        if (CoreGame.inst.draggedObject == this)
        {
            CoreGame.inst.draggedObject = null;

            //Finish dragging
            dragInteraction.FinishDrag();
        }
    }
}
