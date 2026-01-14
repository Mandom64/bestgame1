using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hitscan : MonoBehaviour
{
    private Ammo mAmmo;
    private LineRenderer hitRay;
    [Header("Hitscan Weapon Parameters")]
    [SerializeField] private AudioSource fireSound;
    [SerializeField] private LayerMask damageLayers;
    [SerializeField] private float cooldownTimer = 0.25f;
    [SerializeField] private float shootDistance = 10f;
    [SerializeField] private Vector2 rayThickness = new Vector2(0.01f, 0.01f);
    [SerializeField] private float damage = 15f;
    [SerializeField] private bool canShoot = true;
    private float timer = 0.0f;

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
                Utils.EnableAiming(this.gameObject);
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
            Vector3 mousePos = Utils.GetMouseWorldPosition(Input.mousePosition);
            Vector2 shootDir = (mousePos - transform.position).normalized;
            RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position,
                    rayThickness, 0.0f, shootDir, shootDistance, damageLayers);
            GameObject enemyHit = null;
            foreach(RaycastHit2D hit in hits)
            {
                if (hit.collider != null)
                    enemyHit = hit.collider.gameObject;
                if (enemyHit != null)
                {
                    if(enemyHit.CompareTag("Walls"))
                    {
                        DrawRay(mousePos);
                        mAmmo.UseAmmo(1);
                        if (fireSound != null)
                            fireSound.Play();
                        break;
                    }
                    else
                    {
                        DrawRay(enemyHit.transform);
                        HealthSystem enemyHP = enemyHit.GetComponent<HealthSystem>();
                        enemyHP.Damage(damage);
                        EnemyHealthbar enemyHealthbar;
                        if ((enemyHealthbar = enemyHit.GetComponentInChildren<EnemyHealthbar>()) != null)
                        {
                            enemyHealthbar.UpdateHealthbarUI(enemyHP.getHealth(), enemyHP.getHealthMax());
                        }
                        mAmmo.UseAmmo(1);
                        if (fireSound != null)
                            fireSound.Play();
                        break;
                    }
                }
            }

        }
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
