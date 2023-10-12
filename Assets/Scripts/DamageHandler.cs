using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{    
    public float baseWallDamage = 10.0f;
    public float wallDamageMultiplier = 7.5f;

    public void Start()
    {
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {   
            HealthSystem objectHit = gameObject.GetComponent<HealthSystem>();
            float damageAmount = collision.gameObject.GetComponent<Damage>().damage;
            if(objectHit != null)
            {
                objectHit.Damage(damageAmount);
                Destroy(collision.gameObject);
                Debug.Log(objectHit + " took " + damageAmount + " from a Bullet()");
            }
        }

        if(collision.gameObject.CompareTag("Pellet"))
        {   
            HealthSystem objectHit = gameObject.GetComponent<HealthSystem>();
            float damageAmount = collision.gameObject.GetComponent<Damage>().damage;
            if(objectHit != null)
            {
                objectHit.Damage(damageAmount);
                Destroy(collision.gameObject);
                Debug.Log(objectHit + " took " + damageAmount + " from a Pellet[]");
            }
        }
        // Check if the collision is with an object on the "Wall" layer
        if (collision.gameObject.CompareTag("Walls"))
        {   
            HealthSystem objectHit = gameObject.GetComponent<HealthSystem>();
            if(objectHit != null)
            {
                // Calculate damage based on velocity
                float velocityMagnitude = collision.relativeVelocity.magnitude;
                int damageAmount = Mathf.RoundToInt(baseWallDamage + 
                    velocityMagnitude * wallDamageMultiplier);

                // Apply damage to the player
                objectHit.Damage(damageAmount);
                Debug.Log(objectHit + " took " + damageAmount + " from a WALL##");
            }
        }
    }

  
}
