using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Projectile
{
    [SerializeField] float acceleration = 5;

    protected override void FixedUpdate()
    {
        speed += acceleration * Time.fixedDeltaTime;

        base.FixedUpdate();
    }
}
