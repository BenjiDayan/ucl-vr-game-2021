using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMask : MonoBehaviour
{
    bool crashing = false;
    public float thrustFailRate = 0.05f;
    public GameObject particleObjectPrefab;
    float thrustFailure = 0f;
    Rigidbody rb;


    void BeginCrash()
    {
        crashing = true;
        Instantiate(particleObjectPrefab, transform, false);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (crashing)
        {
            rb.AddForce(Vector3.down * thrustFailure * 9.81f * Time.deltaTime, ForceMode.VelocityChange);
            thrustFailure = Mathf.Clamp(thrustFailure + thrustFailRate * Time.deltaTime, 0f, 0.1f);

            if (transform.position.y < -64.5f)
            {
                Debug.Log("You win!");
            }
        }
    }
}
