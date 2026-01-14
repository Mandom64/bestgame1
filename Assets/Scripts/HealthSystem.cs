using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    private float health;
    public bool isDead = false;

    [Header("My health parameters")]
    [SerializeField] private bool deleteOnDeath = true;
    [SerializeField] private float deleteOnDeathTimer = 3f;
    [SerializeField] private float healthMax = 100;

    public void Start()
    {
        this.health = healthMax;
    }
    public float getHealth() { return health;}
    public float getHealthMax() { return healthMax;}
    public void Heal(float healAmount)
    {
        health += healAmount;
        if(health > healthMax)
            health = healthMax;
    }
    public void Damage(float damageAmount)
    {
        if(!isDead)
        {
            health -= damageAmount;
            if(health <= 0)
            {
                health = 0;
                if(deleteOnDeath) 
                {
                    Destroy(this.gameObject, deleteOnDeathTimer);
                    Debug.Log(this.gameObject + " is deleted!");
                }
                else
                {
                    this.gameObject.GetComponent<Explosives>().Explode();
                }
                isDead = true;
            }
        }
    }
}
