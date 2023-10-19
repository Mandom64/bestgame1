using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

enum eyeState
    {
        Idle,
        Engaged,
        Dead,
    };

public class EyeController : MonoBehaviour
{
    private Rigidbody2D body;
    private GameObject player;
    private float timer = 0.0f;
    private eyeState currentState = eyeState.Idle;
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
    public Sprite eye_dead;
    SpriteRenderer eyeRenderer;
    Animator mAnimator;
    
    

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        body = GetComponent<Rigidbody2D>();
        lineToPlayer = GetComponent<LineRenderer>();
        eyeRenderer = GetComponent<SpriteRenderer>();
        mAnimator = GetComponent<Animator>();
        if(mAnimator != null)
            mAnimator.SetBool("running", true);
    }

    void Update()
    {
        if(gameObject != null)
        {
            timer += Time.deltaTime;
            // Check if im grabbed by gravity gun
            if(gameObject.transform.parent == null || !gameObject.transform.parent.CompareTag("Weapon"))
            {
                mAnimator.SetBool("struck", false);
                if (!isPlayerClose())
                    currentState = eyeState.Idle;
                else
                    currentState = eyeState.Engaged;
                //Debug.Log(currentState);

                float eyeHP = body.GetComponent<HealthSystem>().getHealth();
                if (eyeHP <= 0)
                    currentState = eyeState.Dead;

                switch (currentState)
                {
                    case (eyeState.Idle):
                        if (mAnimator != null)
                        {
                            mAnimator.SetBool("running", true);
                        }
                        lineToPlayer.enabled = false;
                        if (timer >= timeToMove)
                        {
                            Vector2 randomDir = RandomVector2(3.1415f * 2, 0f);
                            randomDir.Normalize();
                            body.velocity = randomDir * idleSpeed * Time.deltaTime;
                            timer = 0f;
                        }
                        break;

                    case (eyeState.Engaged):
                        if (mAnimator != null)
                        {
                            mAnimator.SetBool("running", true);
                        }
                        DrawLineToPlayer();
                        // calculate distance to move
                        var step = engagedSpeed * Time.deltaTime; 
                        transform.position = Vector3.MoveTowards(transform.position,
                            player.transform.position, step);
                        if (timer >= cooldownTimer)
                        {
                            Shoot(player);
                            timer = 0f;
                        }
                        break;

                    case (eyeState.Dead):
                        if (mAnimator != null)
                            mAnimator.SetBool("dead", true);
                        body.velocity = Vector2.zero;
                        gameObject.layer = LayerMask.NameToLayer("Dead Objects");
                        lineToPlayer.enabled = false;
                        eyeRenderer.sprite = eye_dead;
                        break;
                }
            }
        }       
    }

    private bool isPlayerClose()
    {
        if(player == null)
            return false;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if(distance >= range)
            return false;
        else 
            return true;
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
