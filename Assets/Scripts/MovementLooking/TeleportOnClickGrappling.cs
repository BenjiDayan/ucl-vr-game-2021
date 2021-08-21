using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class TeleportOnClickGrappling : MonoBehaviour
{
    [SerializeField] KeyCode teleportKey = KeyCode.Mouse1;
    [SerializeField] InputFeatureUsage<bool> teleportKeyVR = CommonUsages.triggerButton;

    private RaycastHit lastRaycastHit;

    private InputDevice device;

    private AudioSource spoolSound;

    [SerializeField] float range = 50;
    [SerializeField] float speed = 2.5f;

    [Tooltip("distance to hooked point after which released")]
    [SerializeField] float epsilon=5.0f;
    private bool travelling = false;
    private float gravity;
    private Vector3 targetPos;
    private List<Vector3> targetPosList;
    private GameObject targetObject;
    private Collider targetCollider;
    private float t = 0;
    private float cancel_travel_cooldown_percent = 0.2f;
    private float travel_again_cooldown_time = 0.2f;
    private Vector3 dir;
    [SerializeField] PlayerMovementFPS movement;
    private void Start() {
        spoolSound = GetComponent<AudioSource>();
        spoolSound.loop = true;

        gravity = movement.gravity;
        Debug.Log("movement: " + movement);

        var leftHandDevices = new List<InputDevice>();   
        InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, leftHandDevices);
        if(leftHandDevices.Count == 1)
        {
            device = leftHandDevices[0];
            Debug.Log("Found left hand device");

            var inputFeatures = new List<InputFeatureUsage>();
            if (device.TryGetFeatureUsages(inputFeatures)) {
                foreach (var feature in inputFeatures) {
                    if (feature.type == typeof(bool)) {
                        bool featureValue;
                        if (device.TryGetFeatureValue(feature.As<bool>(), out featureValue))
                        {
                            Debug.Log(string.Format("Bool feature '{0}''s value is '{1}'", feature.name, featureValue.ToString()));
                        }
                    }
                    else {
                        Debug.Log(string.Format("Non bool feature '{0}''s has type is '{1}'", feature.name, feature.type));
                    }
                }
            }
        }
        else {
            Debug.Log("No left hand devices!!");
        }

        var inputDevices = new List<InputDevice>();
        InputDevices.GetDevices(inputDevices);

        foreach (var device in inputDevices)
        {
            Debug.Log(string.Format("Device found with name '{0}' and role '{1}'", device.name, device.role.ToString()));
        }
    }

    private Collider GetLookedAtCollider()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;
        // if (Physics.Raycast(origin, direction, out lastRaycastHit, range)) {
        //     return lastRaycastHit.collider;
        // }
        // else
        //     return null;

        RaycastHit[] hits;
        hits = Physics.RaycastAll(origin, direction, range);
        for (int i=0; i < hits.Length; i++) {
            GameObject hit = hits[i].transform.gameObject;
            if (hit.layer == LayerMask.NameToLayer("Building")) {
                lastRaycastHit = hits[i];
                return hit.GetComponent<Collider>();
            }
        }
        return null;

    }

    private void ModifyObjectsColliders(GameObject building, bool enabled) {
        // Component[] components = gameObject.GetComponentsInChildren<Component>(true);
        // List<Collider> colliders = new List<Collider>();
        // foreach (Component c in components) {
        //     if (c.GetType() is Collider) {
        //         colliders.Add((Collider) c);
        //     } 
        // }
        // Debug.Log("Found " + colliders.Count.ToString() + " colliders");
        // List<BoxCollider> box_colliders = new List<BoxCollider>();
        // foreach (Component c in components) {
        //     if (c.GetType() is BoxCollider) {
        //         colliders.Add((BoxCollider) c);
        //     } 
        // }
        // Debug.Log("Found " + box_colliders.Count.ToString() + " BoxColliders");
        // foreach (Collider c in colliders) {
        //     c.enabled = enabled;
        // }
    }

    private void TeleportToLookAt(GameObject teleportationTarget)
    {
        //transform.position = lastRaycastHit.point + lastRaycastHit.normal * 2; 
        Transform targetTrans = teleportationTarget.transform;
        
        if (Vector3.Dot(lastRaycastHit.normal, Vector3.up) > 1/Mathf.Sqrt(2)) {
            //This is designed for teleporting next to objects - i.e. backtrack along the surface normal a little, and go up so as not to fall through the floor
            //transform.root.position = new Vector3(lastRaycastHit.point.x, lastRaycastHit.point.y + 2, lastRaycastHit.point.z) - lastRaycastHit.normal * 2;
            targetPos = new Vector3(lastRaycastHit.point.x, lastRaycastHit.point.y + 2, lastRaycastHit.point.z) - lastRaycastHit.normal * 2;
        }
        else {
            //This is designed for teleporting on top of buildings
            Debug.Log("teleporting on top of building: it's position is" + targetTrans.position.ToString());
            //transform.root.position = new Vector3(targetTrans.position.x, targetTrans.position.y + targetTrans.lossyScale.y/2 + 2, targetTrans.position.z);
            //targetPos = new Vector3(targetTrans.position.x, targetTrans.position.y + targetTrans.lossyScale.y/2 + 2, targetTrans.position.z);
            targetPos = new Vector3(lastRaycastHit.point.x, targetTrans.position.y + targetTrans.lossyScale.y + 2, lastRaycastHit.point.z) - lastRaycastHit.normal * 2;
            Debug.Log("lossyScaley: " + targetTrans.lossyScale.y.ToString());
            Debug.Log("lastRaycastHit.point: " + lastRaycastHit.point.ToString());
        }

        movement.gravity = 0;
        t = 0;
        dir = targetPos - transform.root.position;
        travelling = true;
    }

    private List<Vector3> GetTargetPositions(Collider teleportationTarget) {
        Transform targetTrans = teleportationTarget.transform;
        List<Vector3> output = new List<Vector3>();

        float colliderHeightScale = 0.0f;
        if (teleportationTarget is BoxCollider) {
            colliderHeightScale = ((BoxCollider)teleportationTarget).size.y;
        }
        else if (teleportationTarget is CapsuleCollider) {
            colliderHeightScale = ((CapsuleCollider)teleportationTarget).height;
        }

        if (Vector3.Dot(lastRaycastHit.normal, Vector3.up) > 1/Mathf.Sqrt(2)) {
            //This is designed for teleporting next to objects - i.e. backtrack along the surface normal a little, and go up so as not to fall through the floor
            //transform.root.position = new Vector3(lastRaycastHit.point.x, lastRaycastHit.point.y + 2, lastRaycastHit.point.z) - lastRaycastHit.normal * 2;
            Vector3 temp = new Vector3(lastRaycastHit.point.x, lastRaycastHit.point.y + 2, lastRaycastHit.point.z) - lastRaycastHit.normal * 2;
            output.Add(temp);
        }
        else {
            //This is designed for teleporting on top of buildings
            Debug.Log("teleporting on top of building: it's position is" + targetTrans.position.ToString());
            // It seems that bounds.max.y is the real world scale y value of the top of the collider, regardless of position.
            Vector3 temp = new Vector3(lastRaycastHit.point.x, teleportationTarget.bounds.max.y + 4, lastRaycastHit.point.z);
            Debug.Log("lastRaycastHit.point: " + lastRaycastHit.point.ToString());
            Debug.Log("target: " + temp.ToString());
            Debug.Log("bounds center y: " + teleportationTarget.bounds.center.y.ToString());
            Debug.Log("transform bounds max y: " + teleportationTarget.bounds.max.y.ToString());
            Debug.Log("collider size: " + colliderHeightScale.ToString());
            output.Add(temp + lastRaycastHit.normal * 4);
            output.Add(temp - lastRaycastHit.normal * 4);
        }
        return output;

    }
    //Like TeleportOnCollider except sets targetPosList as sequence of coordinates to reach.
    private void TeleportOnColliderMulti(Collider teleporationTarget) {
        targetPosList = GetTargetPositions(teleporationTarget);
        targetPos = targetPosList[0];

        movement.gravity = 0;
        t = 0;
        dir = targetPos - transform.root.position;
        travelling = true;
        spoolSound.Play();
    }
    private void TeleportOnCollider(Collider teleportationTarget) {
                //transform.position = lastRaycastHit.point + lastRaycastHit.normal * 2; 
        Transform targetTrans = teleportationTarget.transform;
        
        if (Vector3.Dot(lastRaycastHit.normal, Vector3.up) > 1/Mathf.Sqrt(2)) {
            //This is designed for teleporting next to objects - i.e. backtrack along the surface normal a little, and go up so as not to fall through the floor
            //transform.root.position = new Vector3(lastRaycastHit.point.x, lastRaycastHit.point.y + 2, lastRaycastHit.point.z) - lastRaycastHit.normal * 2;
            targetPos = new Vector3(lastRaycastHit.point.x, lastRaycastHit.point.y + 2, lastRaycastHit.point.z) - lastRaycastHit.normal * 2;
        }
        else {
            //This is designed for teleporting on top of buildings
            Debug.Log("teleporting on top of building: it's position is" + targetTrans.position.ToString());
            //transform.root.position = new Vector3(targetTrans.position.x, targetTrans.position.y + targetTrans.lossyScale.y/2 + 2, targetTrans.position.z);
            // targetPos = new Vector3(targetTrans.position.x, teleportationTarget.bounds.max.y + 4, targetTrans.position.z);
            //targetPos = new Vector3(lastRaycastHit.point.x, teleportationTarget.bounds.center.y + teleportationTarget.bounds.max.y/2 + 4, targetTrans.position.z) - lastRaycastHit.normal * 2;
            targetPos = new Vector3(lastRaycastHit.point.x, teleportationTarget.bounds.center.y + teleportationTarget.bounds.max.y/2 + 4, lastRaycastHit.point.z) - lastRaycastHit.normal * 2;
            Debug.Log("lastRaycastHit.point: " + lastRaycastHit.point.ToString());
            Debug.Log("target: " + targetPos.ToString());
        }

        movement.gravity = 0;
        t = 0;
        dir = targetPos - transform.root.position;
        travelling = true;
        spoolSound.Play();
    }

    void Travel()
    {
        if (travelling)
        {
            t += Time.deltaTime * speed / dir.magnitude;
            Transform root = transform.root;
            root.position = Vector3.Lerp(root.position, targetPos, t);

            bool triggerValue;
            if  (   
                    (
                        (
                        Input.GetKeyDown(teleportKey) ||
                        (device.TryGetFeatureValue(teleportKeyVR, out triggerValue) && triggerValue) 
                        ) &&
                        t > cancel_travel_cooldown_percent
                    ) ||
                    (Vector3.Distance(root.position, targetPos) <= epsilon)
                )
            {
                Debug.Log("No longer travelling - within " + epsilon.ToString() + " distance of target");
                travelling = false;
                movement.gravity = gravity;
                ModifyObjectsColliders(targetObject, true);
                t=0;
                spoolSound.Stop();
            }
        }
    }

     void TravelMulti()
    {
        if (travelling)
        {
            t += Time.deltaTime * speed / dir.magnitude;
            Transform root = transform.root;
            root.position = Vector3.Lerp(root.position, targetPos, t);

            bool triggerValue;
            if  (   
                    (
                        (
                        Input.GetKeyDown(teleportKey) ||
                        (device.TryGetFeatureValue(teleportKeyVR, out triggerValue) && triggerValue) 
                        ) &&
                        t > cancel_travel_cooldown_percent
                    ) ||
                    (Vector3.Distance(root.position, targetPos) <= epsilon)
                )
            {
                if (targetPosList.Count > 1) {
                    targetPosList.RemoveAt(0);
                    targetPos = targetPosList[0];
                    return;
                }

                Debug.Log("No longer travelling - within " + epsilon.ToString() + " distance of target");

                travelling = false;
                movement.gravity = gravity;
                ModifyObjectsColliders(targetObject, true);
                t=0;
                spoolSound.Stop();
            }
        }
    }

    void Update()
    {
        if (!travelling) {
            t += Time.deltaTime;
        }
        bool triggerValue;
        if  (   
                (Input.GetKeyDown(teleportKey) ||
                (device.TryGetFeatureValue(teleportKeyVR, out triggerValue) && triggerValue)
                ) && t > travel_again_cooldown_time
            )
        {
            targetCollider = GetLookedAtCollider();
            if (targetCollider != null) {
                targetObject = targetCollider.gameObject;
            }
            if (targetObject != null && !travelling)
                ModifyObjectsColliders(targetObject, false);
                //TeleportToLookAt(targetObject);
                // TeleportOnCollider(targetCollider);
                TeleportOnColliderMulti(targetCollider);
        }

        // Travel();
        TravelMulti();
    }
}
