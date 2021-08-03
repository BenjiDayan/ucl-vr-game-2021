using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To be attached directly to objects climbable with the grappling hook
public class HookPoint : MonoBehaviour
{
    [SerializeField] Vector3 offset = new Vector3(0, 1, 0);

    [SerializeField] Vector3 targetPos;

    MeshRenderer mr;

    public Vector3 _targetPos => targetPos;

    private void Start()
    {
        mr = GetComponent<MeshRenderer>();

        targetPos = mr.bounds.center + new Vector3(0, mr.bounds.extents.y, 0) + offset;
    }
}
