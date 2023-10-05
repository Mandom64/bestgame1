using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    private WeaponType weaponType;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Bullet"))
        {   
            HealthSystem objectHit = gameObject.GetComponent<HealthSystem>();
            int damageAmount = getDamageAmountByType(WeaponType.Gun);
            Debug.Log(objectHit);
            if(objectHit != null)
            {
                Debug.Log(damageAmount);
                objectHit.Damage(damageAmount);
            }
        }

    }

    private int getDamageAmountByType(WeaponType weaponType)
    {
        switch(weaponType)
        {
            case WeaponType.Gun:
                return 10;
            default:
                return 0;
        }
    }

}
