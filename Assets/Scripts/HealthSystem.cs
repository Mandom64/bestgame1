using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int healthMax = 100;
    private int health;

    public void Start()
    {
        this.health = healthMax;
    }

    public int getHealth()
    {
        return health;
    }

    public void Damage(int damageAmount)
    {
        if (gameObject.CompareTag("Player"))
        {
            Debug.Log("true");
            return;
        }

        Debug.Log(health);
        Debug.Log(gameObject.name);
        health -= damageAmount;
        if(health < 0)
        {
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
