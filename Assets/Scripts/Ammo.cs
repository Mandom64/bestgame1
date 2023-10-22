using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    public int maxAmmo = 50;
    public bool canShoot = true;
    private int currAmmo;

    public void Start()
    {
        currAmmo = maxAmmo;
    }
    public int getAmmo() {  return currAmmo; }
    public bool CanShoot() {  return canShoot; }
    public void UseAmmo(int ammoUsed)
    {
        currAmmo -= ammoUsed;
        if (currAmmo <= 0)
            canShoot = false;
        else
            canShoot = true;
    }
    public void NewAmmo(int newAmmo)
    {
        currAmmo += newAmmo;
    }
}
