using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGunFPS2 : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] KeyCode switchProjectile = KeyCode.Q;
    [SerializeField] KeyCode reloadKey = KeyCode.E;

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

    private void Start()
    {
        magazines = new Magazine[] { laser, bullet, rocket };

        cooldownCounter = 0;
    }

    private void Update()
    {
        cooldownCounter += Time.deltaTime;

        SwitchProjectile();

        Reload();

        if (cooldownCounter >= cooldown && Input.GetButtonDown("Fire1"))
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
    }

    void SwitchProjectile()
    {
        if (Input.GetKeyDown(switchProjectile))
        {
            currentProjectile++;
            currentProjectile = currentProjectile % projectilePrefabs.Length;
        }
    }

    void Reload()
    {
        if (Input.GetKeyDown(reloadKey))
        {
            magazines[currentProjectile].Reload();
        }
    }
}
