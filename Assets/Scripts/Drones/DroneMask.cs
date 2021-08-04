using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMask : MonoBehaviour
{
    Transform droneTransform;
    Drone droneScript;
    GameObject drone;
    public float damping = 2f;
    Material myMat, eyeMat, shieldMat;
    Color shieldColour = new Color(1, 1, 1, 1);
    public float shieldFadeTime = 0.5f;
    float impactTime;
    Transform shieldTransform;
    public Color enemyColour;
    public Color playerDroneColour;
    public Color playerDroneEyeColour;
    float eyeBrightness = 2.118547f;
    public float scrambleHueChangeRate = 1f;
    public float rebootHueChangeRate = 0.12f;

    //scrambled
    Quaternion scrambleStartRotation;
    float scrambleStartTime;
    Color oldColour;
    Color newColour;
    float colourChangeStartTime;

    //hack
    float hackStartTime;
    Transform scanTransform;
    MeshRenderer scanRenderer;
    Light scanLight;

    void ReceiveDroneObject(GameObject assignedDrone)
    {
        drone = assignedDrone;
        droneTransform = assignedDrone.transform;
        droneScript = assignedDrone.GetComponent<Drone>();
        foreach (Transform child in transform)
        {
            if (child.gameObject.name.Contains("muzzle_flash"))
            {
                child.SendMessage("ReceiveDroneObject", assignedDrone);
            }
        }

        shieldTransform = transform.Find("drone_shield");
        scanTransform = transform.Find("drone_scan");
        scanRenderer = scanTransform.gameObject.GetComponent<MeshRenderer>();
        scanLight = transform.Find("Point Light").gameObject.GetComponent<Light>();

        myMat = GetComponent<MeshRenderer>().material;
        eyeMat = transform.Find("drone_eye").GetComponent<MeshRenderer>().material;
        shieldMat = transform.Find("drone_shield").GetComponent<MeshRenderer>().material;
        if (assignedDrone.tag == "Enemy Drone")
        {
            myMat.SetColor("_EmissionColor", enemyColour);
            eyeMat.SetColor("_EmissionColor", enemyColour * eyeBrightness);
        }
        else
        {
            Destroy(GetComponent<SphereCollider>());
        }

        impactTime = -shieldFadeTime;
    }

    void ScrambleStart()
    {
        Vector3 rotationAngles = Vector3.zero;
        rotationAngles.y = transform.eulerAngles.y;
        scrambleStartRotation = Quaternion.Euler(rotationAngles);
        scrambleStartTime = Time.realtimeSinceStartup;

        colourChangeStartTime = Time.realtimeSinceStartup;
        oldColour = Random.ColorHSV(0, 1f, 0, 1f, 1f, 1f);
        newColour = Random.ColorHSV(0, 1f, 0, 1f, 1f, 1f);

        Destroy(GetComponent<SphereCollider>());
    }

    void BeginHack()
    {
        hackStartTime = Time.realtimeSinceStartup;
    }
    
    void LateUpdate()
    {
        scanRenderer.enabled = false;
        scanLight.enabled = false;

        Quaternion targetRotation = droneTransform.rotation;

        string parentMode = droneScript.mode;
        if (parentMode == "wait for instructions" || parentMode == "orbit")
        {
            Vector3 lookTarget = droneScript.orbitOrigin - droneTransform.position;
            lookTarget.y = 0f;
            targetRotation = Quaternion.LookRotation(lookTarget);
            if (parentMode == "orbit")
            {
                transform.rotation = Quaternion.LookRotation(lookTarget);
            }
        }
        else if (parentMode == "scrambled")
        {
            targetRotation = scrambleStartRotation;
            float timeSinceScrambled = Time.realtimeSinceStartup - scrambleStartTime;
            targetRotation *= Quaternion.Euler(Mathf.Cos(timeSinceScrambled * 5) * 20, Mathf.Sin(timeSinceScrambled * 5) * 20, 0);
            targetRotation *= Quaternion.Euler(Mathf.Sin(timeSinceScrambled * 30) * 10, 0, 0);

            Color currentColour = Color.Lerp(oldColour, newColour, (Time.realtimeSinceStartup - colourChangeStartTime) / scrambleHueChangeRate);
            myMat.SetColor("_EmissionColor", currentColour);
            eyeMat.SetColor("_EmissionColor", currentColour * eyeBrightness);
            if (Time.realtimeSinceStartup - colourChangeStartTime > scrambleHueChangeRate)
            {
                colourChangeStartTime = Time.realtimeSinceStartup;
                oldColour = newColour;
                newColour = Random.ColorHSV(0, 1f, 0, 1f, 1f, 1f);
            }
        }
        else if (parentMode == "reboot")
        {
            float fade = 0;
            targetRotation = scrambleStartRotation;
            float rebootTimeRemaining = droneScript.rebootDuration + droneScript.rebootStart - Time.realtimeSinceStartup;
            if (rebootTimeRemaining > 2f)
            {
                fade = rebootTimeRemaining - 2f;
            }
            else if (rebootTimeRemaining < 1.5f)
            {
                fade = 1 - rebootTimeRemaining / 1.5f;
            }
            else
            {
                fade = 0;
            }

            if (rebootTimeRemaining > 2f)
            {
                Color currentColour = Color.Lerp(oldColour, newColour, (Time.realtimeSinceStartup - colourChangeStartTime) / rebootHueChangeRate);
                myMat.SetColor("_EmissionColor", currentColour * fade);
                eyeMat.SetColor("_EmissionColor", currentColour * eyeBrightness * fade);
                if (Time.realtimeSinceStartup - colourChangeStartTime > rebootHueChangeRate)
                {
                    colourChangeStartTime = Time.realtimeSinceStartup;
                    oldColour = newColour;
                    newColour = Random.ColorHSV(0, 1f, 0, 1f, 1f, 1f);
                }
            }
            else
            {
                myMat.SetColor("_EmissionColor", playerDroneColour * fade);
                eyeMat.SetColor("_EmissionColor", playerDroneEyeColour * 1.516925f * fade);
            }
        }
        else
        {
            if (droneScript.targetPosition - droneTransform.position != Vector3.zero)
            {
                targetRotation = Quaternion.LookRotation(droneScript.targetPosition - droneTransform.position);
            }

            if (parentMode == "hack drone")
            {
                if (Time.realtimeSinceStartup - hackStartTime > 0.2 && Time.realtimeSinceStartup - hackStartTime < 5.3)
                {
                    scanRenderer.enabled = true;
                    scanLight.enabled = true;
                }
                if (Time.realtimeSinceStartup - hackStartTime < 0.5 || Time.realtimeSinceStartup - hackStartTime > 5)
                {
                    scanTransform.localRotation = Quaternion.identity;
                }
                else
                {
                    scanTransform.localRotation = Quaternion.EulerAngles(new Vector3(Mathf.Sin((Time.realtimeSinceStartup - hackStartTime - 0.5f) * 1.3962634016f) * 0.523599f, 0, 0));
                }
            }
        }

        transform.position = droneTransform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * damping);

        shieldColour.a = Mathf.Clamp(1f - (Time.realtimeSinceStartup - impactTime) / shieldFadeTime, 0f, 1f);
        shieldMat.SetColor("_Color", shieldColour);

    }

    void OnCollisionEnter(Collision collision)
    {
        drone.SendMessage("ReceiveCollisionInfo", collision);

        string colliderName = collision.gameObject.name;
        if (colliderName.Contains("Bullet") || colliderName.Contains("Laser") || colliderName.Contains("Rocket"))
        {
            impactTime = Time.realtimeSinceStartup;
            shieldColour.a = 1;
            shieldMat.SetColor("_Color", shieldColour);
            shieldTransform.rotation = Quaternion.LookRotation(collision.GetContact(0).point - transform.position);
        }
    }
}
