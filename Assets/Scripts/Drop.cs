using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Drop : MonoBehaviour
{
    [SerializeField] protected ItemID type;

    [SerializeField] Spawner spawner;

    [SerializeField] float rotationSpeed = 90;
    [SerializeField] float lifetime = 20;

    [SerializeField] protected int quantity = 1;

    Vector3 dtheta = new Vector3(0, 0, 0);

    float lifetimeCount = 0;

    Rigidbody rb;

    public ItemID _dropType => type;

    public Spawner _spawner
    {
        get => spawner;
        set => spawner = value;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        lifetimeCount += Time.deltaTime;

        if (lifetimeCount >= lifetime)
        {
            spawner.ResetCooldown();
            spawner._spawnedDrop = false;

            Destroy(gameObject);
        }

        dtheta.y = rotationSpeed * Time.deltaTime;
        transform.Rotate(dtheta);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var player = collision.gameObject.GetComponent<PlayerController>();

        if (player != null) CollectDrop(player);
    }

    public virtual void CollectDrop(PlayerController player)
    {
        spawner.ResetCooldown();
        spawner._spawnedDrop = false;

        // UI effects

        Destroy(gameObject);
    }
}
