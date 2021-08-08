using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class Projectile : MonoBehaviour
{
    [SerializeField] protected bool affectedByGravity = false;

    [SerializeField] public float speed = 50;

    [SerializeField] protected float lifetime = 1;

    [SerializeField] protected Vector3 trajectory;

    [SerializeField] protected AudioClip destructionSound;
    [SerializeField] protected float destructionVolume = 1f;

    protected float lifetimeCount = 0;

    Rigidbody rb;

    public Vector3 _trajectory
    {
        get => trajectory;
        set
        {
            trajectory.x = value.x;
            trajectory.y = value.y;
            trajectory.z = value.z;
        }
    }

    public Rigidbody _rb => rb;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = affectedByGravity;
    }

    protected virtual void Update()
    {
        lifetimeCount += Time.deltaTime;

        if (lifetimeCount >= lifetime) Destroy(gameObject);
    }

    protected virtual void FixedUpdate()
    {
        rb.MovePosition(transform.position + trajectory * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        int damage = 0;
        var damageable = collision.gameObject.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.OnDamage(damage);
            if (destructionSound != null) {
                AudioSource.PlayClipAtPoint(destructionSound, transform.position, destructionVolume);
            }
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, trajectory);
    }
}
