using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    private void Start()
    {
        cooldownCounter = 0;
    }

    private void Update()
    {
        cooldownCounter += Time.deltaTime;

        SwitchProjectile();

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
        GameObject projectileObj = Instantiate(projectilePrefabs[currentProjectile].gameObject,
            transform.position + 0.5f * transform.forward, Quaternion.identity) as GameObject;

        Projectile projectile = projectileObj.GetComponent<Projectile>();

        projectile.transform.forward = transform.forward;
        projectile._trajectory = transform.forward;
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
