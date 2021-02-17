using System;
using UnityEngine;

[Serializable]
public class Magazine
{
    [SerializeField] int ammoSize;
    [SerializeField] int stockSize;

    [SerializeField] int ammo;
    [SerializeField] int stock;

    public int _size => ammoSize;
    public int _stockSize => stockSize;

    public int _ammo => ammo;
    public int _stock => stock;

    public bool _empty => ammo == 0;

    public Magazine(int ammoSize, int stockSize)
    {
        this.ammoSize = ammoSize;
        this.stockSize = stockSize;

        ammo = ammoSize;
        stock = ammo;
    }

    public void ChangeAmmo(int amount)
    {
        ammo += amount;

        if (ammo > ammoSize) ammo = ammoSize;
        if (ammo < 0) ammo = 0;
    }

    public void ChangeStock(int amount)
    {
        stock += amount;

        if (stock > stockSize) stock = stockSize;
        if (stock < 0) stock = 0;
    }

    public void Reload()
    {
        if (ammo == ammoSize) return;
        if (stock <= 0) return;

        for(int i = ammo; i < ammoSize; i++)
        {
            ammo++;

            if (stock > 0) stock--;
            else break;
        }

        //int diff = ammoSize - ammo;

        //ammo += diff;
        //stock -= diff;

        //if (ammo > ammoSize) ammo = ammoSize;
        //if (ammo < 0) ammo = 0;

        //if (stock > stockSize) stock = stockSize;
        //if (stock < 0) stock = 0;
    }
}
