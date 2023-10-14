using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revolver : MonoBehaviour
{
    public GameObject bullet;
    public float bulletSpeed = 1.0f;
    public float bulletDeathTimer = 3.0f;
    public float cooldownTimer = 0.25f;
    private float timer = 0.0f;

    void Update()
    {
        timer += Time.deltaTime;
        Transform parent = transform.parent;
        if(parent != null)
        {
            if(parent.CompareTag("Player"))
            {
                EnableAiming();
                if(Input.GetMouseButton(0) && (timer >= cooldownTimer))
                {
                    Shoot();
                    timer = 0.0f;
                }
            }
        }
    }

    private void Shoot()
    {
        Vector3 mousePos = GetMouseWorldPosition(Input.mousePosition);
        Vector2 shootDir = (mousePos - transform.position).normalized;
        GameObject pelletInstance = Instantiate(bullet, transform.position, Quaternion.identity);
        Rigidbody2D rb_pellet = pelletInstance.GetComponent<Rigidbody2D>();
        rb_pellet.velocity = shootDir * bulletSpeed;
        Destroy(pelletInstance, bulletDeathTimer);
        
    }
    private void EnableAiming()
    {
        Vector3 mousePos = GetMouseWorldPosition(Input.mousePosition);
        Vector3 aimDir = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        if (angle > 90f)
        {
            gameObject.GetComponent<SpriteRenderer>().flipY = true;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().flipY = false;
        }
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    public Vector3 GetMouseWorldPosition(Vector3 screenPos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        return worldPos;
    }
}
