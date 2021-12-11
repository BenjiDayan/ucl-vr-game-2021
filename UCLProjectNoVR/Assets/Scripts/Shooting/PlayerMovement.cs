using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Use character controller (cc) methods for now
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed = 10;

    float dx, dz;

    Vector3 ds = new Vector3(0, 0);

    Rigidbody rb;
    CharacterController cc;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        MovePlayerCc();
    }

    private void FixedUpdate()
    {
    }

    void MovePlayerCc()
    {
        ds.x = Input.GetAxis("Horizontal");
        ds.z = Input.GetAxis("Vertical");

        dx = Input.GetAxisRaw("Horizontal");
        dz = Input.GetAxisRaw("Vertical");

        //if (ds != Vector3.zero) transform.forward = ds;

        cc.SimpleMove(ds * speed * Time.deltaTime);
    }
}
