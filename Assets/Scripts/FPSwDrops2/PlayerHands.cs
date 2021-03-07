using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHands : MonoBehaviour
{
    [SerializeField] KeyCode grabKey = KeyCode.F;

    [SerializeField] PlayerGunFPS2 gun;

    private void Update()
    {
        Grab();
    }

    private void Grab()
    {
        // Click mouse button
        if (Input.GetKeyDown(grabKey))
        {
            // Pick up with dominant hand
            Collider[] hits = Physics.OverlapSphere(transform.position + 0.5f * transform.forward, 1);
            if (hits != null)
            {
                foreach(var hit in hits)
                {
                    AmmoDrop2 drop = hit.gameObject.GetComponent<AmmoDrop2>();
                    if (drop != null) drop.CollectDrop(gun);
                }
            }
        }
    }
}
