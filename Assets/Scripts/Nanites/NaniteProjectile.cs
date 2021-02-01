using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaniteProjectile : MonoBehaviour
{

    public GameObject naniteCloud;

    void OnCollisionEnter(Collision collision)
    {
        string colliderName = collision.gameObject.name;
        if (colliderName != "Gun" && colliderName != "Player" && GameObject.Find("Nanite Cloud(Clone)") == null)
        {
            Instantiate(naniteCloud, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
