using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Gun object is child of player object
public class PlayerGun : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] KeyCode aimUp = KeyCode.I;
    [SerializeField] KeyCode aimDown = KeyCode.K;
    [SerializeField] KeyCode aimLeft = KeyCode.J;
    [SerializeField] KeyCode aimRight = KeyCode.L;
    [SerializeField] KeyCode resetAim = KeyCode.U;
    [SerializeField] KeyCode shootKey = KeyCode.Space;
    [SerializeField] KeyCode switchProjectile = KeyCode.O;

    [Header("Aiming")]
    [SerializeField] float rotationSpeed = 30;
    [SerializeField] float fov_xz = 90;
    [SerializeField] float fov_xy = 90;
    [SerializeField] float cooldown = 0.3f;

    [Header("Projectiles")]
    [SerializeField] int currentProjectile = 0;
    [SerializeField] Projectile[] projectilePrefabs = null;
    //[SerializeField] Projectile projectilePrefab = null;

    float cooldownCounter = 0;

    float xUpper, xLower, yUpper, yLower;

    float dtx, dty;

    Vector3 dtheta = new Vector3(0, 0, 0);

    private void Start()
    {
        cooldownCounter = 0;

        xUpper = 0.5f * fov_xz;
        xLower = -xUpper;

        yUpper = 0.5f * fov_xy;
        yLower = -yUpper;
    }

    private void Update()
    {
        cooldownCounter += Time.deltaTime;

        Switch();

        Aim();

        if (cooldownCounter >= cooldown && Input.GetKeyDown(shootKey))
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

    void Aim()
    {
        if (Input.GetKey(aimUp))
        {
            dtx -= rotationSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(aimDown))
        {
            dtx += rotationSpeed * Time.deltaTime;
        }

        if (Mathf.Abs(dtx) > Mathf.Abs(xUpper)) dtx = Mathf.Sign(dtx) * xUpper;

        if (Input.GetKey(aimRight))
        {
            dty += rotationSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(aimLeft))
        {
            dty -= rotationSpeed * Time.deltaTime;
        }

        if (Mathf.Abs(dty) > Mathf.Abs(yUpper)) dty = Mathf.Sign(dty) * yUpper;

        dtheta.x = dtx;
        dtheta.y = dty;

        if (Input.GetKeyDown(resetAim))
        {
            dtx = 0;
            dty = 0;
            dtheta = Vector3.zero;

            transform.localRotation = Quaternion.Euler(dtheta);
        }

        if (dtheta != Vector3.zero) transform.localRotation = Quaternion.Euler(dtheta);
    }

    void Shoot()
    {
        GameObject projectileObj = Instantiate(projectilePrefabs[currentProjectile].gameObject,
            transform.position + 0.5f * transform.forward, Quaternion.identity) as GameObject;

        Projectile projectile = projectileObj.GetComponent<Projectile>();

        projectile.transform.forward = transform.forward;
        projectile._trajectory = transform.forward;
    }

    void Switch()
    {
        if (Input.GetKeyDown(switchProjectile))
        {
            currentProjectile++;
            currentProjectile = currentProjectile % projectilePrefabs.Length;
        }
    }
}
