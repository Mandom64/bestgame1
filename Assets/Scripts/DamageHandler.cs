using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    public WeaponType weaponType;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        HealthSystem objectHit = collision.gameObject.GetComponent<HealthSystem>();
        int damageAmount = getDamageAmountByType(weaponType);
        if(objectHit != null)
        {
            objectHit.Damage(damageAmount);
        }

    }

    private int getDamageAmountByType(WeaponType weaponType)
    {
        switch(weaponType)
        {
            case WeaponType.Gun:
                return 10;
        }
        return 0;
    }

}
