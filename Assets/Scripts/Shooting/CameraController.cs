using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] bool active = true;

    [SerializeField] Transform target;
    [SerializeField] PlayerGun playerGun;

    public bool _active
    {
        get => active;
        set => active = value;
    }

    public Transform _target
    {
        get => target;
        set => target = value;
    }

    private void Start()
    {
        transform.position = target.position;
    }

    private void LateUpdate()
    {
        if (active)
        {
            transform.position = target.position;
            transform.forward = playerGun.transform.forward;
        }
    }
}
