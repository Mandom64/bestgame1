using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{    
    private WeaponType weaponType;
    public float baseWallDamage = 10.0f;
    public float wallDamageMultiplier = 7.5f;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Bullet"))
        {   
            HealthSystem objectHit = gameObject.GetComponent<HealthSystem>();
            int damageAmount = getDamageAmountByType(WeaponType.Gun);
            if(objectHit != null)
            {
                objectHit.Damage(damageAmount);
                Debug.Log(objectHit + " took " + damageAmount + " from a bullet");
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
                Debug.Log(objectHit + " took " + damageAmount + " from a wall");
            }
        }
    }

    private int getDamageAmountByType(WeaponType weaponType)
    {
        switch(weaponType)
        {
            case WeaponType.Gun:
                return 20;
            default:
                return 0;
        }
    }
}
