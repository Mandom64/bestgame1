using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    [SerializeField] public int maxAmmo = 50;
    private int currAmmo;
    public bool canShoot = true;

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
    public void IncreaseAmmo(int newAmmo)
    {
        currAmmo += newAmmo;
    }
}
