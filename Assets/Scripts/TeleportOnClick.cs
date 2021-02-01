using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportOnClick : MonoBehaviour
{
    private RaycastHit lastRaycastHit;
    private GameObject GetLookedAtObject()
    {
        Vector3 origin = transform.position;
        Vector3 direction = Camera.main.transform.forward;
        float range = 50;
        if (Physics.Raycast(origin, direction, out lastRaycastHit, range))
            return lastRaycastHit.collider.gameObject;
        else
            return null;
    }
    private void TeleportToLookAt()
    {
        //transform.position = lastRaycastHit.point + lastRaycastHit.normal * 2;
       transform.position = new Vector3(lastRaycastHit.point.x, lastRaycastHit.point.y + 2, lastRaycastHit.point.z) - lastRaycastHit.normal * 2;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            if (GetLookedAtObject() != null)
                TeleportToLookAt();
        
    }
}
