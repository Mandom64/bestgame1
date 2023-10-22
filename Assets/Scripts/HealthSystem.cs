using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public float healthMax = 100;
    public float deleteTimer = 3f;
    private float health;

    public void Start()
    {
        this.health = healthMax;
    }
    public float getHealth() { return health;}
    public float getHealthMax() { return healthMax;}
    public void Damage(float damageAmount)
    {
        health -= damageAmount;
        if(health <= 0)
        {
            health = 0;
            Destroy(gameObject, deleteTimer);
            Debug.Log(gameObject + " is deleted!");
        }
    }
    public void Heal(float healAmount)
    {
        health += healAmount;
        if(health > healthMax)
            health = healthMax;
    }
}
