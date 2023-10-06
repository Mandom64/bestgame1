using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public int healthMax = 100;
    private int health;

    public void Start()
    {
        this.health = healthMax;
    }

    public int getHealth() { return health;}
    public int getHealthMax() { return healthMax;}

    public void Damage(int damageAmount)
    {
        // Debug.Log(gameObject.name + " took " + 
        //     damageAmount + " damage!");
        health -= damageAmount;

        if(health < 0)
        {
            Debug.Log(gameObject + " is deleted!");
            health = 0;
            Destroy(gameObject);
        }
    }

    public void Heal(int healAmount)
    {
        health += healAmount;
        if(health > healthMax)
            healthMax = health;
    }
}
