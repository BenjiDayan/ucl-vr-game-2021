using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportOnClick : MonoBehaviour
{
    public float range = 50f;
    private RaycastHit lastRaycastHit;
    private GameObject GetLookedAtObject()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;
        if (Physics.Raycast(origin, direction, out lastRaycastHit, range))
            return lastRaycastHit.collider.gameObject;
        else
            return null;
    }
    private void TeleportToLookAt(GameObject teleportationTarget)
    {
        //transform.position = lastRaycastHit.point + lastRaycastHit.normal * 2; 
        Transform targetTrans = teleportationTarget.transform;
        
        if (Vector3.Dot(lastRaycastHit.normal, Vector3.up) > 1/Mathf.Sqrt(2)) {
            //This is designed for teleporting next to objects - i.e. backtrack along the surface normal a little, and go up so as not to fall through the floor
            transform.parent.position = new Vector3(lastRaycastHit.point.x, lastRaycastHit.point.y + 2, lastRaycastHit.point.z) - lastRaycastHit.normal * 2;
        }
        else {
            //This is designed for teleporting on top of buildings
            transform.parent.position = new Vector3(targetTrans.position.x, targetTrans.position.y + targetTrans.lossyScale.y/2 + 2, targetTrans.position.z);
        }
    }
    /*
    private void TeleportToLookAt()
    {
        //transform.position = lastRaycastHit.point + lastRaycastHit.normal * 2;
       transform.parent.position = new Vector3(lastRaycastHit.point.x, lastRaycastHit.point.y + 2, lastRaycastHit.point.z) - lastRaycastHit.normal * 2;
    }
    */
    void Update()
    {
        if (Input.GetButtonDown("Fire2")) {
            GameObject targetObject = GetLookedAtObject();
            if (targetObject != null)
                TeleportToLookAt(targetObject);
        }
    }
}
