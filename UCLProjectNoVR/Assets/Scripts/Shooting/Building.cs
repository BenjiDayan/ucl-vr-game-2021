using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Building : MonoBehaviour, IDamageable
{
    public void OnDamage(float damage)
    {
        print("Explode");
    }

    public IEnumerator OnDamageCo(float damage)
    {
        yield return null;
    }
}
