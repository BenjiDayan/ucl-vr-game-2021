using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Transform player;

    private void LateUpdate()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
        // camera rotates with player too
        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
    }
}
