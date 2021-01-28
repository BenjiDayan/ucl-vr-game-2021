using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentCollapser : MonoBehaviour
{

    public Transform parentTransform;
    public Rigidbody childRigidbody;
    public Transform buildingSegment;
    public float startingMomentum;
    public float deletionTimer;
    public Rigidbody rb;
    public Transform exploderTransform;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        buildingSegment = GetComponent<Transform>();
        parentTransform = transform.parent.GetComponent<Transform>();
        exploderTransform = transform.parent.parent.Find("Explosion Origin");
    }

    void Update()
    {
            if (Input.GetKeyDown("3"))
            {
                RandomCollapse();
            }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (
            (collision.gameObject.transform.parent == null || collision.gameObject.transform.parent.gameObject != buildingSegment.parent.gameObject) &&
            collision.gameObject.name != "Plane"
            )
        {
            RandomCollapse();
        }

    }

    void RandomCollapse()
    {
        if (rb.constraints != RigidbodyConstraints.None)
        {
            Debug.Log("collapsing");
            float randomAngle = Random.Range(0.0f, 2f * Mathf.PI);
            Vector3 randomDirection = new Vector3(Mathf.Sin(randomAngle), 0, Mathf.Cos(randomAngle));
            foreach (Transform child in parentTransform)
            {
                childRigidbody = child.gameObject.GetComponent<Rigidbody>();
                childRigidbody.constraints = RigidbodyConstraints.None;
                childRigidbody.AddForce(randomDirection * startingMomentum);
            }
            StartCoroutine(WaitCoroutine());
        }

    }

    IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(deletionTimer);
        foreach (Transform child in parentTransform)
        {
            Destroy(child.gameObject);
        }
    }

    IEnumerator DestroyColliderTimer(float duration)
    {
        Collider myCollider = GetComponent<BoxCollider>();
        yield return new WaitForSeconds(duration);
        Destroy(myCollider);
    }
}
