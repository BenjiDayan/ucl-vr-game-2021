using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

// Gun object is child of player object
public class PlayerGunFPS : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] KeyCode switchProjectile = KeyCode.O;


    [Header("Projectiles")]
    [SerializeField] int currentProjectile = 0;
    [SerializeField] Projectile[] projectilePrefabs = null;
    [SerializeField] float cooldown = 0.3f;

    float cooldownCounter = 0;

    InputDevice device;

    private void Start()
    {
        cooldownCounter = 0;

        var rightHandDevices = new List<InputDevice>();   
        InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, rightHandDevices);
        if(rightHandDevices.Count == 1)
        {
            device = rightHandDevices[0];
            Debug.Log("Found right hand device");

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
            Debug.Log("No right hand devices!!");
        }

        var inputDevices = new List<InputDevice>();
        InputDevices.GetDevices(inputDevices);

        foreach (var device in inputDevices)
        {
            Debug.Log(string.Format("Device found with name '{0}' and role '{1}'", device.name, device.role.ToString()));
        }
                
    }

    private void Update()
    {
        cooldownCounter += Time.deltaTime;

        SwitchProjectile();
        bool triggerValue2;
        if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue2) && triggerValue2) {
            Debug.Log("Trigger Button pressed");
        } 

        bool triggerValue;
        //if (cooldownCounter >= cooldown && device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue)
        if (cooldownCounter >= cooldown && 
            (   
                Input.GetButtonDown("Fire1") ||
                (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue)
            )
        )
        {
            Shoot();

            cooldownCounter = 0;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward);
    }

    void Shoot()
    {
        GameObject projectileObj = Instantiate(projectilePrefabs[currentProjectile].gameObject,
            transform.position + 0.5f * transform.forward, Quaternion.identity) as GameObject;

        Projectile projectile = projectileObj.GetComponent<Projectile>();

        projectile.transform.forward = transform.forward;
        projectile._trajectory = transform.forward;

        // var rightHandDevices = new List<InputDevice>();   
        // InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandDevices);
        // InputDevice temp_device ;
        // var inputFeatures = new List<InputFeatureUsage>();
        // if(rightHandDevices.Count == 1)
        // {
        //     temp_device = rightHandDevices[0];
        //     if (temp_device.TryGetFeatureUsages(inputFeatures)) {
        //         foreach (var feature in inputFeatures) {
        //             if (feature.type == typeof(bool)) {
        //                 bool featureValue;
        //                 if (temp_device.TryGetFeatureValue(feature.As<bool>(), out featureValue))
        //                 {
        //                     Debug.Log(string.Format("Bool feature '{0}''s value is '{1}'", feature.name, featureValue.ToString()));
        //                 }
        //             }
        //             else {
        //                 Debug.Log(string.Format("Non bool feature '{0}''s has type is '{1}'", feature.name, feature.type));
        //             }
        //         }
        //     }
        // }

    }

    void SwitchProjectile()
    {
        if (Input.GetKeyDown(switchProjectile))
        {
            currentProjectile++;
            currentProjectile = currentProjectile % projectilePrefabs.Length;
        }
    }
}
