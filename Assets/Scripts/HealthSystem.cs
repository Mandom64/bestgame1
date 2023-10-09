using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public float healthMax = 100;
    private float health;

    public void Start()
    {
        this.health = healthMax;
    }
    public float getHealth() { return health;}
    public float getHealthMax() { return healthMax;}
    public void Damage(float damageAmount)
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
