using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLookFPS : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //Cursor is locked - good for FPS looking
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //mouseY corresponds to desire to look up/down. Correspondingly rotate
        //camera's transform about the x-axis. Additionally clamp the rotation
        //so you can't tilt your head all the way backwards.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        //mouseX corresponds to desire to look left/right. Correspondingly rotate
        //whole player's transform about the y-axis (camera follows). 
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
