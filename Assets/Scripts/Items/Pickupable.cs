using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : MonoBehaviour
{
    public delegate void OnPickedUp(NPCData go);
    public delegate void OnPickedDown(NPCData go);

    public event OnPickedUp onPickedUp;
    public event OnPickedDown onPickedDown;

    public NPCData carriedBy;

    public void PickUp(NPCData go)
    {
        carriedBy = go;
        go.carryingItem = this;
        if (onPickedUp != null)
            onPickedUp.Invoke(carriedBy);
    }

    public void Drop()
    {
        transform.parent = null;
        carriedBy.carryingItem = null;
        var copy = carriedBy;
        carriedBy = null;

        RaycastHit hitInfo;
        //Shoots a ray into the 3D world starting at our mouseposition
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo))
        {
            transform.position = hitInfo.point;
            transform.rotation = Quaternion.identity;
        }

        if (onPickedDown != null)
            onPickedDown.Invoke(copy);
    }


    public void ItemTransfer(NPCData other)
    {
        Drop();
        PickUp(other);
    }
}
