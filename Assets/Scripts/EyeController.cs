using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public enum EnemyState
    {
        Idle,
        Engaged,
        Dead,
    };

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D body;
    private GameObject player;
    private float timer = 0.0f;
    private RectTransform healthBarTransform;
    private EnemyState currentState = EnemyState.Idle;
    private LineRenderer lineToPlayer;
    [Header("Eye Parameters")]
    public float range = 1f;
    public float idleSpeed = 1f;
    public float engagedSpeed = 15f;
    public float timeToMove = 5f;
    public Slider healthBar;
    [Header("Bullet Parameters")]
    public GameObject bullet;
    public float bulletSpeed = 15.0f;
    public float bulletDeathTimer = 3.0f;
    public float cooldownTimer = 0.5f;
    

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        body = GetComponent<Rigidbody2D>();
        healthBarTransform = healthBar.GetComponent<RectTransform>();
        lineToPlayer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if(gameObject != null)
        {
            timer += Time.deltaTime;
            showHealthBar();
            // Check if im grabbed by gravity gun
            if(gameObject.transform.parent == null || !gameObject.transform.parent.CompareTag("Weapon"))
            {
                if (!isPlayerClose())
                    currentState = EnemyState.Idle;
                else
                    currentState = EnemyState.Engaged;
                //Debug.Log(currentState);

                switch (currentState)
                {
                    case (EnemyState.Idle):
                        lineToPlayer.enabled = false;
                        if (timer >= timeToMove)
                        {
                            Vector2 randomDir = RandomVector2(3.1415f * 2, 0f);
                            randomDir = randomDir.normalized * idleSpeed * Time.deltaTime;
                            body.AddForce(randomDir);
                            timer = 0f;
                        }
                        break;

                    case (EnemyState.Engaged):
                        DrawLineToPlayer();

                        // calculate distance to move
                        var step = engagedSpeed * Time.deltaTime; 
                        transform.position = Vector3.MoveTowards(transform.position,
                            player.transform.position, step);

                        if(timer >= cooldownTimer)
                        {
                            Shoot(player);
                            timer = 0f;
                        }
                        break;

                    case (EnemyState.Dead):
                        break;
                }
            }
        }       
    }

    private bool isPlayerClose()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if(distance >= range)
            return false;
        else 
            return true;
    }

    private void showHealthBar()
    {
        // Healthbar slider and text update
        if(gameObject != null)
        {
            HealthSystem playerHealth = gameObject.GetComponent<HealthSystem>();
            float healthPercentage = (float)playerHealth.getHealth() / (float)playerHealth.getHealthMax();
            healthBar.value = healthPercentage;     
            Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            Vector2 aboveSprite = screenPos;
            aboveSprite.y += 25f;
            // Update the position of the Slider's RectTransform
            healthBarTransform.position = aboveSprite;     
        }
    }
    private void Shoot(GameObject objToShoot)
    {
        if (objToShoot != null)
        {
            Vector3 shootAtPos = objToShoot.transform.position;
            Vector2 shootDir = (shootAtPos - transform.position).normalized;
            GameObject bulletInstance = Instantiate(bullet, transform.position, Quaternion.identity);
            Rigidbody2D bulletBody = bulletInstance.GetComponent<Rigidbody2D>();
            bulletInstance.layer = LayerMask.NameToLayer("Enemy Projectiles");
            bulletBody.velocity = shootDir * bulletSpeed;
            Destroy(bulletInstance, bulletDeathTimer);
        }
    }
    
    public void DrawLineToPlayer()
    {
        lineToPlayer.enabled = true;
        lineToPlayer.material = new Material(Shader.Find("Sprites/Default"));
        lineToPlayer.material.color = Color.red;
        lineToPlayer.SetPosition(0, transform.position);
        lineToPlayer.SetPosition(1, player.transform.position);
    }

    public Vector3 GetMouseWorldPosition(Vector3 screenPos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        return worldPos;
    }

    public Vector2 RandomVector2(float angle, float angleMin)
    {
        float random = UnityEngine.Random.value * angle + angleMin;
        return new Vector2(Mathf.Cos(random), Mathf.Sin(random));
    }
    
}
