using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovementFPS : MonoBehaviour
{
    [SerializeField] KeyCode jumpKey = KeyCode.Space;

    public CharacterController controller;
    public float speed = 12f;
    
    //Apparently mixing character controller and rigidbody is a bad idea, so instead to get
    //things like gravity we introduce the velocity vector which is updated.
    public float gravity = -9.81f;
    Vector3 velocity;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float jumpHeight = 6f;
    public float groundDeathHeight = 30f;
    bool isGrounded;

    private PlayerHealth playerHealth;
    

    // Start is called before the first frame update
    void Start()
    {

        playerHealth = (PlayerHealth)FindObjectOfType(typeof(PlayerHealth));
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.transform.position.y < groundDeathHeight) {
            playerHealth.OnDamage(25);
        }

        //isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; //"Set to zero" but actually force player to indeed hit the bottom
        }

        bool jumpValue;
        if ( isGrounded &&
            (   
                Input.GetKeyDown(jumpKey)
            )
        ){
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //local coordinates based direction we want to move
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }
}
