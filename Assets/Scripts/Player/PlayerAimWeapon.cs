using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAimWeapon : MonoBehaviour
{
    private Transform aimTransform;
    public GameObject bulletPrefab;
    public float bulletSpeed;
    public float deathTimer;

    private void Awake()
    {
        aimTransform = transform.Find("Aim");
    }
    void Update()
    {
        handleAiming();
        handleShooting();
    }

    private void handleAiming()
    {
        Vector3 mousePos = GetMouseWorldPosition(Input.mousePosition);
        Vector3 aimDirection = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aimTransform.eulerAngles = new Vector3(0, 0, angle);
        //  Debug.Log(angle);
    }
    private void handleShooting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = GetMouseWorldPosition(Input.mousePosition);
            Vector2 shootDirection = (mousePos - transform.position).normalized;
            GameObject bullet = Instantiate(bulletPrefab, aimTransform.position, Quaternion.identity);
            Rigidbody2D bulletRigidBody = bullet.GetComponent<Rigidbody2D>();
            bulletRigidBody.velocity = shootDirection * bulletSpeed;
            Destroy(bullet, deathTimer);
        }
    }

    private float mouseAngle()
    {
        Vector3 mousePos = GetMouseWorldPosition(Input.mousePosition);
        Vector3 aimDirection = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        return angle;
    }
    public Vector3 GetMouseWorldPosition(Vector3 screenPosition)
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }
}



    