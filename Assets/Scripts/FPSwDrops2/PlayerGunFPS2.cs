using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerGunFPS2 : MonoBehaviour
{
    [SerializeField] AudioSource reloadSound;
    [Header("Input")]
    [SerializeField] KeyCode switchProjectile = KeyCode.Q;
    [SerializeField] InputFeatureUsage<bool> switchProjectileVR = CommonUsages.secondaryButton;
    [SerializeField] KeyCode reloadKey = KeyCode.E;
    [SerializeField] InputFeatureUsage<bool> reloadKeyVR = CommonUsages.gripButton;
    [SerializeField] KeyCode shootGunKey = KeyCode.Mouse0;
    [SerializeField] InputFeatureUsage<bool> shootGunKeyVR = CommonUsages.triggerButton;

    [Header("Projectiles")]
    [SerializeField] int currentProjectile = 0;
    [SerializeField] Projectile[] projectilePrefabs = null;

    [SerializeField] Magazine laser = new Magazine(12, 48);
    [SerializeField] Magazine bullet = new Magazine(12, 96);
    [SerializeField] Magazine rocket = new Magazine(1, 24);

    public Magazine[] magazines;

    [SerializeField] float cooldown = 0.3f;
    [SerializeField] float[] reloadTime = new float[] {1.5f, 1.5f, 1.5f};

    float cooldownCounter = 0;

    public Magazine _laser => laser;
    public Magazine _bullet => bullet;
    public Magazine _rocket => rocket;

    InputDevice device;

    PlayerUI ui;

    bool reloading = false;
    float reloadFinishTime = 0f;

    MeshRenderer[] renderers = new MeshRenderer[] { null, null, null };

    private void Start()
    {

        for (int i = 0; i < 3; i++)
        {
            renderers[i] = transform.GetChild(i).GetComponent<MeshRenderer>();
        }


        reloadSound = GetComponent<AudioSource>();

        ui = (PlayerUI)FindObjectOfType(typeof(PlayerUI));

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

        if (!reloading)
        {
            Shoot();

            Reload();
        }

        UpdateUI();

        if (reloading && Time.realtimeSinceStartup > reloadFinishTime)
        {
            reloading = false;
            magazines[currentProjectile].Reload();
        }
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
                Input.GetKey(shootGunKey) ||
                (device.TryGetFeatureValue(shootGunKeyVR, out triggerValue) && triggerValue)
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

    void UpdateUI()
    {
        string outputString = "";
        outputString += new string[] { "Laser", "Bullet", "Rocket" }[currentProjectile];
        outputString += "\n";
        outputString += magazines[currentProjectile]._ammo.ToString() + " / " + magazines[currentProjectile]._stock.ToString();
        ui.UpdateAmmo(outputString);
    }

    void SwitchProjectile()
    {
        bool switchValue;
        if (
            Input.GetKeyDown(switchProjectile) ||
            (device.TryGetFeatureValue(switchProjectileVR, out switchValue) && switchValue)
        )
        {
            reloading = false;

            renderers[currentProjectile].enabled = false;
            currentProjectile++;
            currentProjectile = currentProjectile % projectilePrefabs.Length;
            renderers[currentProjectile].enabled = true;
        }
    }

    void Reload()
    {
        bool reloadValue;
        if (
            (((Input.GetKeyDown(reloadKey) ||
            (device.TryGetFeatureValue(reloadKeyVR, out reloadValue) && reloadValue)) &&
            magazines[currentProjectile]._ammo != magazines[currentProjectile]._size) ||
            magazines[currentProjectile]._ammo == 0) &&
            magazines[currentProjectile]._stock != 0
        )
        {
            reloading = true;
            reloadFinishTime = Time.realtimeSinceStartup + reloadTime[currentProjectile];
            reloadSound.Play();
        }
    }
}
