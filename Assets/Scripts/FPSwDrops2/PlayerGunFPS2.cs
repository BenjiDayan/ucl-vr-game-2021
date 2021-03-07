using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerGunFPS2 : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] KeyCode switchProjectile = KeyCode.Q;
    [SerializeField] InputFeatureUsage<bool> switchProjectileVR = CommonUsages.secondaryButton;
    [SerializeField] KeyCode reloadKey = KeyCode.E;
    [SerializeField] InputFeatureUsage<bool> reloadKeyVR = CommonUsages.primaryButton;
    [SerializeField] KeyCode shootGunKey = KeyCode.Mouse0;
    [SerializeField] InputFeatureUsage<bool> shootGunKeyVR = CommonUsages.triggerButton;

    [Header("Projectiles")]
    [SerializeField] int currentProjectile = 0;
    [SerializeField] Projectile[] projectilePrefabs = null;

    [SerializeField] Magazine laser = new Magazine(12, 20);
    [SerializeField] Magazine bullet = new Magazine(12, 20);
    [SerializeField] Magazine rocket = new Magazine(4, 10);

    Magazine[] magazines;

    [SerializeField] float cooldown = 0.3f;

    float cooldownCounter = 0;

    public Magazine _laser => laser;
    public Magazine _bullet => bullet;
    public Magazine _rocket => rocket;

    InputDevice device;

    private void Start()
    {
        magazines = new Magazine[] { laser, bullet, rocket };

        cooldownCounter = 0;

        var rightHandDevices = new List<InputDevice>();   
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandDevices);
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

        Reload();

        Shoot();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward);
    }

    void Shoot()
    {
        bool triggerValue;
        if (cooldownCounter >= cooldown && 
            (   
                Input.GetKeyDown(shootGunKey) ||
                (device.TryGetFeatureValue(CommonUsages.triggerButton, out triggerValue) && triggerValue)
            )
        )
        {
            if (magazines[currentProjectile]._empty) return;

            GameObject projectileObj = Instantiate(projectilePrefabs[currentProjectile].gameObject,
                transform.position + 0.5f * transform.forward, Quaternion.identity) as GameObject;

            Projectile projectile = projectileObj.GetComponent<Projectile>();

            projectile.transform.forward = transform.forward;
            projectile._trajectory = transform.forward;

            switch (currentProjectile)
            {
                case 0:
                    laser.ChangeAmmo(-1);
                    break;
                case 1:
                    bullet.ChangeAmmo(-1);
                    break;
                case 2:
                    rocket.ChangeAmmo(-1);
                    break;
                default:
                    break;
            }

            cooldownCounter = 0;
        }
    }

    void SwitchProjectile()
    {
        bool switchValue;
        if (   
            Input.GetKeyDown(switchProjectile) ||
            (device.TryGetFeatureValue(switchProjectileVR, out switchValue) && switchValue)
        )
        {
            currentProjectile++;
            currentProjectile = currentProjectile % projectilePrefabs.Length;
        }
    }

    void Reload()
    {
        bool reloadValue;
        if (   
            Input.GetKeyDown(reloadKey) ||
            (device.TryGetFeatureValue(reloadKeyVR, out reloadValue) && reloadValue)
        )
        {
            magazines[currentProjectile].Reload();
        }
    }
}
