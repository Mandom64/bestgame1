using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{    
    public float baseWallDamage = 10.0f;
    public float wallDamageMultiplier = 7.5f;
    public float minWallDamage = 35f;
    EnemyHealthbar healthbar;
    Animator mAnimator;

    public void Start()
    {
        healthbar = GetComponentInChildren<EnemyHealthbar>();
        mAnimator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject objectHit = collision.gameObject;
        if (objectHit.CompareTag("Bullet"))
        {   
            HealthSystem myHP = gameObject.GetComponent<HealthSystem>();
            float damageAmount = objectHit.GetComponent<Damage>().damage;
            if(myHP != null)
            {
                myHP.Damage(damageAmount);
                healthbar.UpdateHealthbarUI(myHP.getHealth(), myHP.healthMax);
                if (mAnimator != null)
                {
                    mAnimator.SetBool("struck", true);
                }
                Destroy(objectHit);
                Debug.Log(gameObject.name + " took " + damageAmount + " from a Bullet");
            }
        }

        if(objectHit.CompareTag("Pellet"))
        {   
            HealthSystem myHP = gameObject.GetComponent<HealthSystem>();
            float damageAmount = objectHit.GetComponent<Damage>().damage;
            if(myHP != null)
            {
                myHP.Damage(damageAmount);
                healthbar.UpdateHealthbarUI(myHP.getHealth(), myHP.healthMax);
                if (mAnimator != null)
                    mAnimator.SetBool("struck", true);
                Destroy(objectHit);
                Debug.Log(gameObject.name + " took " + damageAmount + " from a Pellet");
            }
        }
        // Check if the collision is with an object on the "Wall" layer
        if (objectHit.CompareTag("Walls"))
        {   
            HealthSystem myHP = gameObject.GetComponent<HealthSystem>();
            if(myHP != null)
            {
                // Calculate damage based on velocity
                float velocityMagnitude = collision.relativeVelocity.magnitude;
                int damageAmount = Mathf.RoundToInt(baseWallDamage + 
                    velocityMagnitude * wallDamageMultiplier);
                if(damageAmount > minWallDamage)
                {
                    // Apply damage to the player
                    myHP.Damage(damageAmount);
                    healthbar.UpdateHealthbarUI(myHP.getHealth(), myHP.healthMax);
                    if(mAnimator != null)
                        mAnimator.SetBool("struck", true);
                    Debug.Log(gameObject.name + " took " + damageAmount + " from a WALL");
                }
            }
        }
    }

}
