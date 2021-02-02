using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaniteCloud : MonoBehaviour
{

    public float duration;
    public float diameter;
    public GameObject naniteShooter;

    private Vector3 maxScale;
    private float initializationTime;
    private float timeElapsed;
    private int segmentsEaten = 0;
    private GameObject collisionObject;

    private GameObject gun;

    void Start()
    {
        initializationTime = Time.timeSinceLevelLoad;
        maxScale = new Vector3(1, 1, 1) * diameter;
        transform.localScale = new Vector3(0, 0, 0);
    }

    void Update()
    {
        timeElapsed = Time.timeSinceLevelLoad - initializationTime;
        if (timeElapsed > duration)
        {
            GameObject.Find("Gun").SendMessage("AddSegments", segmentsEaten);
            Destroy(gameObject);
        }
        else
        {
            transform.localScale = maxScale * (timeElapsed / duration);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        collisionObject = collision.gameObject;
        if (collisionObject.name.Contains("Building Segment"))
        {
            Destroy(collisionObject);
            segmentsEaten++;
        }

    }
}
