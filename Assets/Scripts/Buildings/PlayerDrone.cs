using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDrone : MonoBehaviour
{
    Rigidbody rb;
    GameObject player;
    float realAcceleration;
    int damping = 2;
    public string mode = "follow player";

    GameObject debrisTarget;
    GameObject bubble;
    Rigidbody targetRb;
    float targetOffset;

    Vector3 targetPosition;
    bool trueTarget;

    Vector3[] bubbleOffsets;

    public float acceleration = 10f;
    public float speedLimit = 2f;
    public float defaultHeight = 340f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("FPS_Player");
    }

    void AssignDebrisTarget(GameObject target)
    {
        debrisTarget = target;
        bubble = target.transform.GetChild(0).gameObject;
        targetRb = debrisTarget.GetComponent<Rigidbody>();
        targetOffset = bubble.transform.localScale.y * target.transform.localScale.y / 2;
        mode = "grab debris";
    }

    void FollowPlayer()
    {
        targetPosition = player.transform.position;
        targetPosition.y = defaultHeight;
        //targetPosition += new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
        realAcceleration = (((targetPosition - transform.position).magnitude - 10) * acceleration / 10);
        if (realAcceleration > acceleration)
        {
            realAcceleration = acceleration;
        }
        else if (realAcceleration < 0)
        {
            realAcceleration = 0f;
        }


        Quaternion rotation = Quaternion.LookRotation(targetPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
        rb.AddForce(transform.forward * realAcceleration);
        if (rb.velocity.magnitude > speedLimit)
        {
            rb.velocity = Vector3.Normalize(rb.velocity) * speedLimit;
        }
    }

    bool CastRays()
    {
        if (mode == "bring debris")
        {
            Vector3 rayOrigin;
            bubbleOffsets = new Vector3[]
            {
                bubble.transform.right * (-1),
                bubble.transform.right,
                bubble.transform.up,
                bubble.transform.up * (-1)
            };
            foreach (Vector3 offset in bubbleOffsets)
            {
                rayOrigin = bubble.transform.position + offset * targetOffset;
                if (Physics.Raycast(rayOrigin, targetPosition - rayOrigin, Vector3.Distance(targetPosition, rayOrigin), 1 << 9))
                {
                    return true;
                }
            }
            return false;
        }
        else
        {
            return Physics.Raycast(transform.position, targetPosition - transform.position, Vector3.Distance(targetPosition, transform.position), 1 << 9);
        }
    }

    void Navigate()
    {
        Vector3 lookVector;
        bool doAccelerate = true;

        //If I don't have an unobstructed path to the target, try to get to a point in the sky above it
        if (CastRays())
        {
            targetPosition.y = defaultHeight;
            trueTarget = false;
        }
        else
        {
            trueTarget = true;
        }
        //If I can't get to the sky above the target, go to the sky above me
        if (CastRays())
        {
            targetPosition = transform.position;
            targetPosition.y = defaultHeight;
        }

        //Point in the right direction
        if (trueTarget)
        {
            Vector3 targetDirection = (targetPosition - transform.position);
            targetDirection.Normalize();
            Vector3 idealVelocity = targetDirection * Mathf.Sqrt(2 * acceleration * Vector3.Distance(targetPosition, transform.position));
            if (mode == "grab debris")
            {
                idealVelocity += targetRb.velocity;
            }
            if (idealVelocity.magnitude > speedLimit)
            {
                idealVelocity = Vector3.Normalize(idealVelocity) * speedLimit;
            }
            Vector3 directionalAcceleration = idealVelocity - rb.velocity;
            if (mode == "grab debris" && targetRb.velocity.y < -0.1f)
            {
                float angleA = Vector3.Angle(directionalAcceleration, Vector3.up) * Mathf.Deg2Rad;
                float angleB = Mathf.Asin(Mathf.Sin(angleA) * (-Physics.gravity.y) / acceleration);
                lookVector = directionalAcceleration + Vector3.down * directionalAcceleration.magnitude * Mathf.Sin(angleB) / Mathf.Sin(Mathf.PI - angleA - angleB);
            }
            else
            {
                if (directionalAcceleration.magnitude < acceleration * Time.deltaTime)
                {
                    rb.velocity = idealVelocity;
                    lookVector = idealVelocity;
                    doAccelerate = false;
                }
                else
                {
                    lookVector = idealVelocity - rb.velocity;
                }
            }
        }
        else
        {
            lookVector = targetPosition - transform.position;
        }
        transform.rotation = Quaternion.LookRotation(lookVector);

        //Accelerate
        if (doAccelerate)
        {
            rb.AddForce(transform.forward * acceleration * Time.deltaTime, ForceMode.VelocityChange);
        }
        if (rb.velocity.magnitude > speedLimit)
        {
            rb.velocity = Vector3.Normalize(rb.velocity) * speedLimit;
        }
    }

    void GrabDebris()
    {
        targetPosition = debrisTarget.transform.position;
        targetPosition.y += targetOffset;

        if (Vector3.Distance(targetPosition, transform.position) < rb.velocity.magnitude * Time.deltaTime + 0.5f)
        {
            mode = "bring debris";
            Destroy(targetRb);
            targetRb = player.GetComponent<Rigidbody>();
            bubble.GetComponent<Renderer>().enabled = true;
            BringDebris();
            return;
        }

        Navigate();

    }

    void BringDebris()
    {
        //Move the debris
        debrisTarget.transform.position = transform.position + Vector3.down * targetOffset;
        if (Vector3.Distance(debrisTarget.transform.position, player.transform.position) < targetOffset)
        {
            Destroy(bubble);
            Destroy(debrisTarget);
            mode = "follow player";
            FollowPlayer();
            return;
        }

        targetPosition = player.transform.position;
        targetPosition.y += targetOffset * 2 - 1;
        transform.rotation = Quaternion.LookRotation(targetPosition - transform.position);

        Navigate();
    }

    void Update()
    {
        switch (mode)
        {
            case "follow player":
                FollowPlayer();
                break;
            case "grab debris":
                GrabDebris();
                break;
            case "bring debris":
                BringDebris();
                break;
        }
    }
}



//Adjust direction so I don't overshoot (bad way of doing this)
/*if (mode == "grab debris")
{
    targetVelocity = targetRb.velocity;
}
else
{
    targetVelocity = Vector3.zero;
}
if (
    trueTarget &&
    Mathf.Pow((rb.velocity - targetVelocity).magnitude, 2) > 2 * acceleration * Vector3.Distance(targetPosition, transform.position)
    )
{
    //transform.rotation = Quaternion.LookRotation(transform.position - targetPosition);
    transform.rotation = Quaternion.LookRotation(-rb.velocity);
}
//Otherwise aim at where I'm trying to go
else
{
    transform.rotation = Quaternion.LookRotation(targetPosition - transform.position);
}*/


//Adjust max speed so I don't overshoot
/*if (trueTarget)
{
    if (mode == "grab debris")
    {
        targetSpeed = (targetRb.velocity + rb.velocity).magnitude - rb.velocity.magnitude;
    }
    else
    {
        targetSpeed = 0f;
    }
    realMaxSpeed = Mathf.Sqrt(2 * acceleration * Vector3.Distance(targetPosition, transform.position)) + targetSpeed;
    if (realMaxSpeed > speedLimit)
    {
        realMaxSpeed = speedLimit;
    }
}
else
{
    realMaxSpeed = speedLimit;
}*/