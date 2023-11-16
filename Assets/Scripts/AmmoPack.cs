using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPack : MonoBehaviour
{
    public int ammoAmount = 150;
    public float pullSpeed = 2.5f;
    public float pullRange = 2f;
    private GameObject mObj;
    private GameObject player;
    public void Start()
    {
        mObj = this.gameObject;
        player = GameObject.FindWithTag("Player");
    }

    public void Update()
    {
        if(Utils.isPlayerClose(mObj, player, pullRange))
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position,
                player.transform.position, pullSpeed * Time.deltaTime);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Transform objectHit = collision.transform;
        if (objectHit != null && objectHit.CompareTag("Player"))
        {
            Transform player = objectHit;
            foreach (Transform child in player)
            {
                if(child.gameObject != null && child.gameObject.activeInHierarchy == true)
                {
                    Ammo weaponAmmo = child.GetComponent<Ammo>();
                    if(weaponAmmo != null)
                    {
                        weaponAmmo.IncreaseAmmo(ammoAmount);
                    }
                }
            }
            Destroy(this.gameObject);
        }
    }
}
