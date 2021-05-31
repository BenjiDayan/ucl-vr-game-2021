using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMask : MonoBehaviour
{
    Transform droneTransform;
    float damping = 2f;


    void ReceiveDroneObject(GameObject assignedDrone)
    {
        droneTransform = assignedDrone.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 horizontalDirection = droneTransform.forward;
        horizontalDirection.y = 0f;

        transform.position = droneTransform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, droneTransform.rotation, Time.deltaTime * damping);
    }
}
