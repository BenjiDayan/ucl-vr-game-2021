using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    Rigidbody rb;
    GameObject player;
    float realAcceleration;
    //int damping = 2;

    GameObject debrisTarget;
    GameObject bubble;
    Rigidbody targetRb;
    float targetOffset;

    GameObject droneTarget;

    
    bool trueTarget;

    Vector3[] bubbleOffsets;

    [Header("Flight settings")]
    [SerializeField] public float acceleration = 10f;
    [SerializeField] public float speedLimit = 2f;
    [SerializeField] public float defaultHeight = 340f;
    [SerializeField] public float playerOrbitRadius = 20f;
    [SerializeField] public float playerOrbitPeriod = 4f;
    Vector3 playerOrbitVelocity;
    float playerOrbitSpeed;

    [Header("Attacking settings")]
    [SerializeField] public float attackRunDistance = 100f;
    [SerializeField] public float maxAttackAngle = 20f;
    [SerializeField] public float attackRunDuration = 10f;
    [SerializeField] public float attackRunCooldown = 10f;
    [SerializeField] public float attackRunSpeed = 50f;
    [SerializeField] public float attackRunSidewaysMovement = 5f;

    float futureTime;
    int invertRotation = 1;

    [Header("Hacking settings")]
    [SerializeField] public float hackDuration = 5.5f;
    [SerializeField] public float rebootDuration = 4f;

    float hackCompleteAt;

    [Header("Health settings")]
    [SerializeField] public float startingHP = 3f;
    [SerializeField] public float bulletDamage = 1f;
    [SerializeField] public float laserDamage = 2f;
    [SerializeField] public float rocketDamage = 3f;
    float hp;

    [Header("Prefabs")]
    [SerializeField] public GameObject droneMaskPrefab;

    GameObject droneMask;

    Vector3 constructionSiteTarget;

    GameObject droneController;

    [Header("Leave as default")]
    [SerializeField] public Vector3 orbitOrigin;
    [SerializeField] public Vector3 targetPosition;
    [SerializeField] public string mode = "follow player";
    [SerializeField] public float rebootStart;

    //Orbiting information
    float orbitStartTime;
    float orbitOffset;
    float orbitRadius;
    float orbitTopSpeed;
    int orbitRotationsNumber;
    float orbitDuration;
    float orbitDistance;

    GameObject hologram;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("FPS_Player");
        droneController = GameObject.Find("Drone Controller");
        hologram = GameObject.Find("Hologram");
        playerOrbitSpeed = playerOrbitRadius * 2f * Mathf.PI / playerOrbitPeriod;

        droneMask = Instantiate(droneMaskPrefab);
        droneMask.SendMessage("ReceiveDroneObject", gameObject);

        hp = startingHP;
    }

    void GoToConstructionSite (Vector3 siteTarget)
    {
        constructionSiteTarget = siteTarget;
        mode = "move towards site";
        orbitOrigin = droneController.GetComponent<PDController>().raycatcherTransform.position;
    }

    void EndTask()
    {
        mode = "follow player";
        rb.constraints = RigidbodyConstraints.None;
    }

    void AssignDebrisTarget(GameObject target)
    {
        debrisTarget = target;
        bubble = target.transform.GetChild(0).gameObject;
        targetRb = debrisTarget.GetComponent<Rigidbody>();
        targetOffset = bubble.transform.localScale.y * target.transform.localScale.y / 2;
        mode = "grab debris";
    }

    void ReceiveEnemyDroneTarget(GameObject target)
    {
        droneTarget = target;
        mode = "begin hack";
    }

    void HackComplete()
    {
        rebootStart = Time.realtimeSinceStartup;
        mode = "reboot";
    }

    //This is how drones used to follow the player
    /*
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
    */

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

            //If I can't get to the sky above the target, go to the sky above me
            if (CastRays())
            {
                targetPosition = transform.position;
                targetPosition.y = defaultHeight;
            }
        }
        else
        {
            trueTarget = true;
        }

        //Point in the right direction
        if (trueTarget)
        {
            Vector3 targetDirection;
            if (targetPosition == transform.position)
            {
                targetDirection = Vector3.zero;
            }
            else
            {
                targetDirection = (targetPosition - transform.position);
                targetDirection.Normalize();
            }
            Vector3 idealVelocity = targetDirection * Mathf.Sqrt(2 * acceleration * Vector3.Distance(targetPosition, transform.position));
            switch (mode)
            {
                case "grab debris":
                    idealVelocity += targetRb.velocity;
                    break;
                case "follow player":
                    idealVelocity += playerOrbitVelocity;
                    break;
                case "attack":
                    Vector3 attackDirection = player.transform.position - transform.position;
                    attackDirection.y = 0;
                    idealVelocity += Vector3.Normalize(attackDirection) * attackRunSpeed;
                    break;
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

    void FollowPlayer()
    {
        Vector3 orbitPoint = player.transform.position;
        orbitPoint.y = defaultHeight;
        Vector3 leadingOrbitPoint = orbitPoint;

        List<GameObject> dronesFollowingPlayer = droneController.GetComponent<PDController>().dronesFollowingPlayer;
        float angle = dronesFollowingPlayer.IndexOf(gameObject) * 2f * Mathf.PI / dronesFollowingPlayer.Count;
        angle += ((Time.realtimeSinceStartup / playerOrbitPeriod) % 1) * 2f * Mathf.PI;

        orbitPoint += new Vector3(
            Mathf.Sin(angle) * playerOrbitRadius,
            0,
            Mathf.Cos(angle) * playerOrbitRadius
            );

        leadingOrbitPoint += new Vector3(
            Mathf.Sin(angle + 0.01f) * playerOrbitRadius,
            0,
            Mathf.Cos(angle + 0.01f) * playerOrbitRadius
            );

        playerOrbitVelocity = leadingOrbitPoint - orbitPoint;
        playerOrbitVelocity = Vector3.Normalize(playerOrbitVelocity) * playerOrbitSpeed;

        targetPosition = orbitPoint;

        Navigate();

        //For the benefit of the drone mask
        targetPosition = leadingOrbitPoint;
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

        //Give the debris to the player if I am close enough
        if (Vector3.Distance(debrisTarget.transform.position, player.transform.position) < targetOffset)
        {
            hologram.SendMessage("AddDebris");
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

    void MoveTowardsSite()
    {
        targetPosition = constructionSiteTarget;

        if (Vector3.Distance(targetPosition, transform.position) < rb.velocity.magnitude * Time.deltaTime)
        {
            transform.position = targetPosition;
            mode = "wait for instructions";
            rb.velocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            droneController.SendMessage("InPosition");
            return;
        }

        Navigate();
    }

    void BeginOrbit(float offset)
    {
        mode = "orbit";
        orbitStartTime = Time.realtimeSinceStartup;

        //Get information from drone controller
        orbitOffset = offset;
        orbitOrigin.y = droneController.GetComponent<PDController>().buildingDimensions[droneController.GetComponent<PDController>().buildingIndex][2];
        orbitRadius = droneController.GetComponent<PDController>().radius;
        orbitDuration = droneController.GetComponent<PDController>().buildingConstructionTime;

        //Work out details of orbit
        float d;
        int i = 0;
        float num1 = acceleration * orbitDuration / 2f;
        float num2 = Mathf.Pow(num1 * 2f, 2f);
        float num3;
        float sAdd;
        float sSubtract;
        while (true)
        {
            i++;
            d = Mathf.Sqrt(Mathf.Pow(i * orbitRadius * 2f * Mathf.PI, 2f) + Mathf.Pow(orbitOrigin.y, 2f));
            num3 = Mathf.Sqrt(num2 - (4f * acceleration * d)) / 2f;
            sAdd = num1 + num3;
            sSubtract = num1 - num3;
            if (sAdd < sSubtract && sAdd < speedLimit)
            {
                orbitTopSpeed = sAdd;
            }
            else if (sSubtract < speedLimit)
            {
                orbitTopSpeed = sSubtract;
            }
            else
            {
                break;
            }
            orbitDistance = d;
        }
        orbitRotationsNumber = i - 1;
    }

    void Orbit()
    {
        float t = Time.realtimeSinceStartup - orbitStartTime;
        float accelerationDuration = orbitTopSpeed / acceleration;
        float remainingT = orbitDuration - t;
        float orbitDistanceCovered;

        //Starting acceleration
        if (t < accelerationDuration)
        {
            orbitDistanceCovered = acceleration * Mathf.Pow(t, 2f) / 2f;
        }
        //Braking deceleration
        else if (remainingT < accelerationDuration)
        {
            orbitDistanceCovered = orbitDistance - (acceleration * Mathf.Pow(remainingT, 2f) / 2f);
        }
        //Moving at constant speed
        else
        {
            orbitDistanceCovered = orbitTopSpeed * t - (Mathf.Pow(orbitTopSpeed, 2f) / (acceleration * 2f));
        }

        //Move to where in the orbit I should be
        float angle = (orbitOffset + (orbitDistanceCovered / orbitDistance) * orbitRotationsNumber) * 2f * Mathf.PI;
        Vector3 goToPosition = new Vector3(
            Mathf.Sin(angle) * orbitRadius,
            0,
            Mathf.Cos(angle) * orbitRadius
            ) + orbitOrigin;
        goToPosition.y = (orbitDistanceCovered / orbitDistance) * orbitOrigin.y;
        transform.position = goToPosition;
    }

    void AttackCooldown()
    {
        if (Time.realtimeSinceStartup > futureTime)
        {
            mode = "begin attack run";
        }
        else
        {
            Vector3 relativePosition = transform.position - player.transform.position;
            relativePosition.y = 0f;
            targetPosition = Quaternion.Euler(0, 20 * invertRotation, 0) * Vector3.Normalize(relativePosition) * attackRunDistance + player.transform.position;
            targetPosition.y += 50;

            Navigate();
        }
    }

    void Attack()
    {
        Vector3 playerHorizontalPosition = player.transform.position;
        playerHorizontalPosition.y = transform.position.y;

        targetPosition = Quaternion.Euler(0, attackRunSidewaysMovement * invertRotation, 0) * (transform.position - playerHorizontalPosition) + playerHorizontalPosition;
        if (Time.realtimeSinceStartup > futureTime
            || Vector3.Angle((playerHorizontalPosition - transform.position), (player.transform.position - transform.position)) > maxAttackAngle
            || Physics.Raycast(transform.position, player.transform.position - transform.position, Vector3.Distance(player.transform.position, transform.position), 1 << 9))
        {
            invertRotation = Random.value < .5 ? 1 : -1;
            futureTime = Time.realtimeSinceStartup + attackRunCooldown;
            mode = "attack cooldown";
            AttackCooldown();
        }
        else
        {
            Navigate();

            targetPosition = player.transform.position;
        }
    }

    void BeginAttackRun()
    {
        Vector3 playerHorizontalPosition = player.transform.position;
        playerHorizontalPosition.y += 30;
        Vector3 horizontalPosition = transform.position;
        horizontalPosition.y = playerHorizontalPosition.y;

        targetPosition = Vector3.Normalize(horizontalPosition - playerHorizontalPosition) * attackRunDistance + playerHorizontalPosition;
        for (int j = 0; j < 4; j++)
        {
            for (int i = 0; i < 10; i++)
            {
                if (Physics.Raycast(targetPosition, player.transform.position - targetPosition, Vector3.Distance(player.transform.position, targetPosition), 1 << 9))
                {
                    targetPosition.y = playerHorizontalPosition.y;
                    targetPosition = Quaternion.Euler(0, 36, 0) * (targetPosition - playerHorizontalPosition) + playerHorizontalPosition;
                }
                else
                {
                    break;
                }
            }

            if (Physics.Raycast(targetPosition, player.transform.position - targetPosition, Vector3.Distance(player.transform.position, targetPosition), 1 << 9))
            {
                targetPosition.y += (defaultHeight - targetPosition.y) / (4f - j);
                playerHorizontalPosition.y += (defaultHeight - targetPosition.y) / (4f - j);
                horizontalPosition.y += (defaultHeight - targetPosition.y) / (4f - j);
            }
            else
            {
                break;
            }
        }

        if (Vector3.Distance(transform.position, targetPosition) < 1 &&
            !Physics.Raycast(targetPosition, player.transform.position - targetPosition, Vector3.Distance(player.transform.position, targetPosition), 1 << 9))
        {
            invertRotation = Random.value < .5 ? 1 : -1;
            futureTime = attackRunDuration + Time.realtimeSinceStartup;
            mode = "attack";
            Attack();
        }
        else
        {
            Navigate();
        }
    }

    void Scrambled()
    {
        targetPosition = transform.position;

        Navigate();
    }

    void Reboot()
    {
        if (Time.realtimeSinceStartup - rebootStart < rebootDuration)
        {
            targetPosition = transform.position;

            Navigate();
        }
        else
        {
            tag = "Player Drone";
            mode = "follow player";
            FollowPlayer();
        }
    }

    void HackDrone()
    {
        if (Time.realtimeSinceStartup < hackCompleteAt)
        {
            targetPosition = transform.position;

            Navigate();

            //So the mask looks in the right direction
            targetPosition = droneTarget.transform.position;
        }
        else
        {
            droneTarget.SendMessage("HackComplete");
            mode = "follow player";
            FollowPlayer();
        }
    }

    void BeginHack()
    {
        Vector3 relativeLocation = transform.position - droneTarget.transform.position;
        relativeLocation.y = 0;
        targetPosition = Vector3.Normalize(relativeLocation) * 12.1855f + droneTarget.transform.position;
        if (Vector3.Distance(targetPosition, transform.position) < 0.2)
        {
            droneMask.SendMessage("BeginHack");
            hackCompleteAt = Time.realtimeSinceStartup + hackDuration;
            mode = "hack drone";
            HackDrone();
        }

        Navigate();

        //So the mask looks in the right direction
        targetPosition = droneTarget.transform.position;
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
            case "move towards site":
                MoveTowardsSite();
                break;
            case "wait for instructions":
                break;
            case "orbit":
                Orbit();
                break;
            case "begin attack run":
                BeginAttackRun();
                break;
            case "attack":
                Attack();
                break;
            case "attack cooldown":
                AttackCooldown();
                break;
            case "scrambled":
                Scrambled();
                break;
            case "reboot":
                Reboot();
                break;
            case "begin hack":
                BeginHack();
                break;
            case "hack drone":
                HackDrone();
                break;
        }
    }

    void ReceiveCollisionInfo(Collision collision)
    {
        string colliderName = collision.gameObject.name;
        if (colliderName.Contains("Bullet"))
        {
            hp -= bulletDamage;
        }
        else if (colliderName.Contains("Laser"))
        {
            hp -= laserDamage;
        }
        else if (colliderName.Contains("Rocket"))
        {
            hp -= rocketDamage;
        }

        if (hp <= 0)
        {
            mode = "scrambled";
            droneMask.SendMessage("ScrambleStart");
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