using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoDrop2 : Drop2
{
    public override void CollectDrop(PlayerGunFPS2 gun)
    {
        switch (type)
        {
            case ItemID.Laser:
                gun._laser.ChangeStock(quantity);
                break;
            case ItemID.Bullet:
                gun._bullet.ChangeStock(quantity);
                break;
            case ItemID.Rocket:
                gun._rocket.ChangeStock(quantity);
                break;
            default:
                break;
        }

        base.CollectDrop(gun);
    }
}
