using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerFPS : MonoBehaviour
{
    [SerializeField] PlayerGun gun;

    PlayerMovement movement;

    public PlayerGun _gun => gun;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }
}
