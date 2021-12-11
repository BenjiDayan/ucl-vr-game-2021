using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class TeleportOnClick : MonoBehaviour
{
    [SerializeField] KeyCode teleportKey = KeyCode.Mouse1;
    [SerializeField] InputFeatureUsage<bool> teleportKeyVR = CommonUsages.triggerButton;

    public float range = 100f;
    private RaycastHit lastRaycastHit;

    private InputDevice device;

    Vector3 GetTeleportPoint(GameObject inputObject)
    {
        Vector3 outputPosition = lastRaycastHit.point;
        outputPosition.y = lastRaycastHit.collider.bounds.max.y;

        Vector3 horizontalForward = -lastRaycastHit.normal;
        horizontalForward.y = 0f;
        horizontalForward.Normalize();
        outputPosition += horizontalForward * 0.5f;

        return (outputPosition);
    }

    private void Start() {
    }

    private GameObject GetLookedAtObject()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;
        if (Physics.Raycast(origin, direction, out lastRaycastHit, range, 1 << 9) && (lastRaycastHit.collider.gameObject.tag == "Generic Building" || lastRaycastHit.collider.gameObject.tag == "Landmark"))
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
            transform.root.position = new Vector3(lastRaycastHit.point.x, lastRaycastHit.point.y + 2, lastRaycastHit.point.z) - lastRaycastHit.normal * 2;
        }
        else {
            //This is designed for teleporting on top of buildings
            Debug.Log("teleporting on top of building: it's position is" + targetTrans.position.ToString());
            //transform.root.position = new Vector3(targetTrans.position.x, targetTrans.position.y + targetTrans.lossyScale.y/2 + 2, targetTrans.position.z);
            transform.root.position = GetTeleportPoint(teleportationTarget);
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
        bool triggerValue;
        if  (   
                Input.GetKeyDown(teleportKey) ||
                (device.TryGetFeatureValue(teleportKeyVR, out triggerValue) && triggerValue)
            )
        {
            GameObject targetObject = GetLookedAtObject();
            if (targetObject != null)
                TeleportToLookAt(targetObject);
        }
    }
}
