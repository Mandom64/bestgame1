using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

enum SniperState
{
    Idle,
    Run,
    Aim,
    Shoot,
    Dead,
};

public class SniperController : MonoBehaviour
{
    private Rigidbody2D body;
    private GameObject player;
    private Animator mAnimator;
    private float timer = 0.0f;
    private SniperState currentState = SniperState.Idle;
    private LineRenderer lineToPlayer;
    [Header("Sniper Parameters")]
    public float range = 1f;
    public float idleSpeed = 1f;
    public float timeToMove = 5f;
    public float aimTimer = 1f;
    public bool EnableLine = false;
    [Header("Bullet Parameters")]
    public GameObject bullet;
    public float bulletSpeed = 15.0f;
    public float bulletDeathTimer = 3.0f;
    public float cooldownTimer = 3f;
    private bool isAttacking = false;
    public GameObject sniperWeapon;



    void Start()
    {
        player = GameObject.FindWithTag("Player");
        body = GetComponent<Rigidbody2D>();
        lineToPlayer = GetComponent<LineRenderer>();
        mAnimator = GetComponent<Animator>();
        AnimationState("run");
    }

    private void FixedUpdate()
    {
        // Check if im dead
        if (gameObject != null)
        {
            timer += Time.deltaTime;
            // Check if im grabbed by gravity gun
            if (gameObject.transform.parent == null || !gameObject.transform.parent.CompareTag("Weapon"))
            {
                switch (currentState)
                {
                    case (SniperState.Idle):
                        if (timer >= timeToMove)
                        {
                            Vector2 randomDir = RandomVector2(3.1415f * 2, 0f);
                            randomDir.Normalize();
                            body.velocity = randomDir * idleSpeed * Time.deltaTime;
                            timer = 0f;
                        }
                        break;

                    case (SniperState.Aim):
                        body.velocity = Vector2.zero;
                        break;
                    case (SniperState.Shoot):
                        body.velocity = Vector2.zero;
                        break;
                    case (SniperState.Dead):
                        body.velocity = Vector2.zero;
                        break;
                }

            }
        }
    }

    void Update()
    {
        if (gameObject != null)
        {
            timer += Time.deltaTime;

            FlipSprites();
            if (gameObject.transform.parent == null || !gameObject.transform.parent.CompareTag("Weapon"))
            {
                if (!isPlayerClose())
                    currentState = SniperState.Idle;
                if(isPlayerClose() && !isAttacking)
                    currentState = SniperState.Aim;
                else
                    currentState = SniperState.Shoot;
                float eyeHP = body.GetComponent<HealthSystem>().getHealth();
                if (eyeHP <= 0)
                    currentState = SniperState.Dead;

                switch (currentState)
                {
                    case (SniperState.Idle):
                        AnimationState("idle");
                        lineToPlayer.enabled = false;
                        break;

                    case (SniperState.Aim):
                            //Debug.Log("hello aim here");
                        DrawLineToPlayer();
                        EnableAiming();
                        if (timer >= aimTimer)
                        {
                            currentState = SniperState.Shoot;
                            isAttacking = true;
                            timer = 0f;
                        }
                        break;
                    case (SniperState.Shoot):
                        lineToPlayer.enabled = false;
                        if(timer >= cooldownTimer && isPlayerClose())
                        {
                            Shoot(player);
                            currentState = SniperState.Idle;
                            isAttacking = false;
                            timer = 0f;
                        }
                        break;
                    case (SniperState.Dead):
                        AnimationState("dead");
                        gameObject.layer = LayerMask.NameToLayer("Dead Objects");
                        lineToPlayer.enabled = false;
                        break;
                }
            }
        }
    }

    private bool isPlayerClose()
    {
        if (player == null)
            return false;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance >= range)
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
    private void EnableAiming()
    {
        Vector3 aimDir = (player.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        if (angle > -90f && angle < 90f)
            sniperWeapon.GetComponent<SpriteRenderer>().flipY = false;
        else
            sniperWeapon.GetComponent<SpriteRenderer>().flipY = true;
        sniperWeapon.transform.eulerAngles = new Vector3(0, 0, angle);
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
    public void FlipSprites()
    {
        body.GetComponent<SpriteRenderer>().flipX = (player.transform.position.x < transform.position.x);
    }

    private void AnimationState(string currState)
    {
        if(mAnimator != null) 
        {
            mAnimator.SetBool("idle", false);
            mAnimator.SetBool("run", false);
            mAnimator.SetBool("shoot", false);
            mAnimator.SetBool("dead", false);
            mAnimator.SetBool(currState, true);
        }
    }
}
