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
        FriendCreature fc = GetComponent<FriendCreature>();
        if(fc != null && fc.shoked) return;
        
        if (CoreGame.inst.canDrag)
        {
            if (CoreGame.inst.draggedObject == null)
            {
                CoreGame.inst.draggedObject = this;
                if (dragInteraction != null)
                {
                    dragInteraction.StartDrag();
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
