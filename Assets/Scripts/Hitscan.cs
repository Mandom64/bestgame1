using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitscan : MonoBehaviour
{
    public AudioSource fireSound;
    public LayerMask EnemiesLayer;
    public LineRenderer hitRay;
    private Ammo mAmmo;
    public float cooldownTimer = 0.25f;
    public float shootDistance = 10f;
    public float damage = 15f;
    private float timer = 0.0f;
    private bool canShoot = true;

    private void Start()
    {
        hitRay = GetComponent<LineRenderer>();
        mAmmo = GetComponent<Ammo>();
    }
    void Update()
    {
        timer += Time.deltaTime;
        Transform parent = transform.parent;
        if (parent != null)
        {
            if (parent.CompareTag("Player") && Time.timeScale == 1)
            {
                hitRay.enabled = false;
                EnableAiming();
                if (Input.GetMouseButton(0) && (timer >= cooldownTimer))
                {
                    Shoot();
                    timer = 0.0f;
                }

            }
        }
    }

    private void Shoot()
    {
        canShoot = mAmmo.CanShoot();
        if (canShoot)
        {
            Vector3 mousePos = GetMouseWorldPosition(Input.mousePosition);
            Vector2 shootDir = (mousePos - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position,
                shootDir, shootDistance, EnemiesLayer);
            if (hit.collider != null)
            {
                GameObject enemyHit = hit.collider.gameObject;
                if (enemyHit != null && !enemyHit.CompareTag("Walls"))
                {
                    DrawRay(enemyHit.transform);
                    HealthSystem enemyHP = enemyHit.GetComponent<HealthSystem>();
                    enemyHP.Damage(damage);
                    enemyHit.GetComponentInChildren<EnemyHealthbar>().UpdateHealthbarUI(enemyHP.getHealth(), enemyHP.getHealthMax());
                    mAmmo.UseAmmo(1);
                }
                if (fireSound != null)
                    fireSound.Play();
            }
            else
            {
                DrawRay(mousePos);
                mAmmo.UseAmmo(1);
                if (fireSound != null)
                    fireSound.Play();
            }
        }
    }
    private void EnableAiming()
    {
        Vector3 mousePos = GetMouseWorldPosition(Input.mousePosition);
        Vector3 aimDir = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        if (angle > -90f && angle < 90f)
            gameObject.GetComponent<SpriteRenderer>().flipY = false;
        else
            gameObject.GetComponent<SpriteRenderer>().flipY = true;
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    public Vector3 GetMouseWorldPosition(Vector3 screenPos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        return worldPos;
    }
    public void DrawRay(Transform at)
    {
        hitRay.enabled = true;
        hitRay.material = new Material(Shader.Find("Sprites/Default"));
        hitRay.material.color = Color.yellow;
        hitRay.SetPosition(0, transform.position);
        hitRay.SetPosition(1, at.position);
    }
    public void DrawRay(Vector3 at)
    {
        hitRay.enabled = true;
        hitRay.material = new Material(Shader.Find("Sprites/Default"));
        hitRay.material.color = Color.yellow;
        hitRay.SetPosition(0, transform.position);
        hitRay.SetPosition(1, at);
    }
}


