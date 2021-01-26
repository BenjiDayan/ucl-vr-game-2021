using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCollapser : MonoBehaviour
{

    public Transform parentTransform;
    public Rigidbody childRigidbody;
    public float startingMomentum;
    public int deletionTimer;
    public bool delete = false;
    public Transform exploderTransform;
    public Transform gravityOrigin;
    public bool collapsible = true;
    public float collapseSpeed = 10;

    // Start is called before the first frame update
    void Start()
    {
        parentTransform = transform;
        exploderTransform = transform.parent.Find("Explosion Origin");
        gravityOrigin = transform.parent.Find("Collapse Origin");
    }

    // Update is called once per frame
    void Update()
    {
        if (collapsible)
        {
            collapsible = false;
            if (Input.GetKeyDown("1"))
            {
                InternalExplosion();
            }
            else if (Input.GetKeyDown("2"))
            {
                GravityCollapse();
            }
            else
            {
                collapsible = true;
            }
        }
    }

    void InternalExplosion()
    {
        Vector3 dir;
        float distance;
        Debug.Log("collapsing");
        foreach (Transform child in parentTransform)
        {
            childRigidbody = child.gameObject.GetComponent<Rigidbody>();
            childRigidbody.constraints = RigidbodyConstraints.None;
            dir = (child.position - exploderTransform.position).normalized;
            distance = Vector3.Distance(child.position, exploderTransform.position);
            childRigidbody.AddForce(dir * startingMomentum * 100000f / Mathf.Pow(distance, 2));
        }
        StartCoroutine(WaitCoroutine());
    }

    void GravityCollapse()
    {
        float distance;
        Debug.Log("collapsing");
        foreach (Transform child in parentTransform)
        {
            childRigidbody = child.gameObject.GetComponent<Rigidbody>();
            childRigidbody.constraints = RigidbodyConstraints.None;
            distance = Vector3.Distance(child.position, gravityOrigin.position);
            child.gameObject.SendMessage("DestroyColliderTimer", Mathf.Sqrt(distance) / collapseSpeed);
        }
        StartCoroutine(WaitCoroutine());
    }

    IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(deletionTimer);
        delete = true;
        foreach (Transform child in parentTransform)
        {
            Destroy(child.gameObject);
        }
    }
}
