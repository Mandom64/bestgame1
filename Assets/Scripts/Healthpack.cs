using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthpack : MonoBehaviour
{
    public float healAmount = 150f;

    public void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject objectHit = collision.gameObject;
        if(objectHit != null && objectHit.CompareTag("Player"))
        {
            HealthSystem playerHP = objectHit.GetComponent<HealthSystem>();
            playerHP.Heal(healAmount);
            Destroy(this.gameObject);
        }
    }
}
