using System;
using UnityEngine;

[Serializable]
public class Magazine
{
    [SerializeField] int size;
    [SerializeField] int ammo;

    public int _size => size;
    public int _ammo => ammo;

    public Magazine(int size)
    {
        this.size = size;
        ammo = size;
    }

    public void ChangeAmmo(int amount)
    {
        ammo += amount;
        if (ammo > size) ammo = size;
        if (ammo < 0) ammo = 0;
    }
}
