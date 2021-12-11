using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Does not work on top of hookable buildings
public class GrapplingHook : MonoBehaviour
{
    [SerializeField] KeyCode hookKey = KeyCode.J;

    [SerializeField] bool travelling = false;

    [SerializeField] float gravity = -9.81f;

    [SerializeField] float range = 50;
    [SerializeField] float speed = 2.5f;

    [SerializeField] float cooldown = 5f;
    [SerializeField] float cooldownCount = Mathf.Infinity;

    [SerializeField] Vector3 targetPos;

    float t = 0;
    Vector3 dir;

    PlayerMovementFPS movement;

    void Start()
    {
        movement = GetComponent<PlayerMovementFPS>();
    }

    private void Update()
    {
        cooldownCount += Time.deltaTime;

        Shoot();

        Travel();
    }

    void Shoot()
    {
        if (cooldownCount >= cooldown && Input.GetKeyDown(hookKey))
        {
            // Shoot a ray
            RaycastHit hit;
            Ray ray = new Ray(transform.position + 0.5f * transform.forward, transform.forward);

            // Check ray collides
            if (Physics.Raycast(ray, out hit, range))
            {
                var hook = hit.collider.GetComponent<HookPoint>();
                print(hook == null);
                print("Hook: " + hook.ToString());
                if (hook != null)
                {
                    targetPos = hook._targetPos;
                    movement.gravity = 0;
                    t = 0;
                    dir = targetPos - transform.position;
                    travelling = true;
                }
            }

            cooldownCount = 0;
        }
    }

    void Travel()
    {
        if (travelling)
        {
            t += Time.deltaTime * speed / dir.magnitude;

            transform.position = Vector3.Lerp(transform.position, targetPos, t);

            if (Vector3.Distance(transform.position, targetPos) <= 0.02f)
            {
                travelling = false;
                movement.gravity = gravity;
            }
        }
    }
}
