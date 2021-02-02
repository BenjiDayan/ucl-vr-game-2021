using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoDrop : Drop
{
    public override void CollectDrop(PlayerController player)
    {
        var gun = player._gun;

        switch (type)
        {
            case ItemID.Laser:
                gun._laser.ChangeAmmo(quantity);
                break;
            case ItemID.Bullet:
                gun._bullet.ChangeAmmo(quantity);
                break;
            case ItemID.Rocket:
                gun._rocket.ChangeAmmo(quantity);
                break;
            default:
                break;
        }

        base.CollectDrop(player);
    }
}
