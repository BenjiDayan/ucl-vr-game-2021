using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLookVR : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //playerBody.transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
        //transform.rotation = Quaternion.Euler(transform.eulerAngles.x, 0f, transform.eulerAngles.z);
        Vector3 relRotation = this.gameObject.transform.localRotation.eulerAngles;
        //Debug.Log("relRotation: " + relRotation.ToString());
        playerBody.transform.Rotate(0.0f, relRotation[1], 0.0f);
        this.gameObject.transform.Rotate(0.0f, -relRotation[1], 0.0f);
        //Debug.Log("rotated, now: " + this.gameObject.transform.localRotation.eulerAngles.ToString());

    //     float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
    //     float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

    //     //mouseY corresponds to desire to look up/down. Correspondingly rotate
    //     //camera's transform about the x-axis. Additionally clamp the rotation
    //     //so you can't tilt your head all the way backwards.
    //     xRotation -= mouseY;
    //     xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    //     transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

    //     //mouseX corresponds to desire to look left/right. Correspondingly rotate
    //     //whole player's transform about the y-axis (camera follows). 
    //     playerBody.Rotate(Vector3.up * mouseX);
    // }
    }
}
