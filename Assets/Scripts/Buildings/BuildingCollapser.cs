using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCollapser : MonoBehaviour
{

    public float timeBeforeFreeze = 15f;
    public float startingMomentum = 100f;
    public GameObject player;

    Rigidbody childRigidbody;
    BoxCollider childCollider;

    void OnCollisionEnter(Collision collision)
    {
        string colliderName = collision.gameObject.name;
        if (colliderName.Contains("Bullet") || colliderName.Contains("Laser") || colliderName.Contains("Rocket"))
        {
            Collapse(collision);
        }

    }

    void Collapse(Collision collision)
    {
        Vector3 dir;
        float distance;
        Vector3 explosionOrigin = collision.transform.position;
        Vector3 projectileVelocity = (explosionOrigin - player.transform.position).normalized;
        GetComponent<BoxCollider>().enabled = false;
        foreach (Transform child in transform)
        {
            childRigidbody = child.gameObject.GetComponent<Rigidbody>();
            childRigidbody.constraints = RigidbodyConstraints.None;
            child.gameObject.GetComponent<BoxCollider>().enabled = true;
            distance = Vector3.Distance(child.position, explosionOrigin);
            dir = ((child.position - explosionOrigin).normalized * 1f + projectileVelocity * 1f).normalized;
            childRigidbody.AddForce(dir * startingMomentum * 100000f / Mathf.Pow(distance, 2));
        }
        StartCoroutine(WaitCoroutine());
    }

    IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(timeBeforeFreeze);
        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}
