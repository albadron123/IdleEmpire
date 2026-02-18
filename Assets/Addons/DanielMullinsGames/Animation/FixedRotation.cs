using UnityEngine;

public class FixedRotation : ManagedBehaviour
{
    public bool fixX;
    public float xRotation;

    public bool fixY;
    public float yRotation;

    public bool fixZ;    
    public float zRotation;

    public override void ManagedUpdate()
    {
        float x = fixX ? xRotation : transform.eulerAngles.x;
        float y = fixY ? yRotation : transform.eulerAngles.y;
        float z = fixZ ? zRotation : transform.eulerAngles.z;

        transform.eulerAngles = new Vector3(x, y, z);
	}
}
