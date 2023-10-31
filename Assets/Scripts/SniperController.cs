using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class SniperController : MonoBehaviour
{
    private enum State
    {
        Idle,
        Run,
        Aim,
        Shoot,
        Dead,
    };

    private Rigidbody2D mBody;
    private GameObject player;
    private Animator mAnimator;
    private LineRenderer lineToPlayer;
    private State currentState = State.Idle;
    private float timer = 0.0f;

    [Header("Sniper Parameters")]
    [SerializeField] public float range = 1f;
    [SerializeField] public float idleSpeed = 1f;
    [SerializeField] public float timeToMove = 5f;
    [SerializeField] public float aimTimer = 1f;
    [SerializeField] public bool EnableLine = false;

    [Header("Bullet Parameters")]
    [SerializeField] public GameObject sniperWeapon;
    [SerializeField] public GameObject bullet;
    [SerializeField] public float bulletSpeed = 15.0f;
    [SerializeField] public float bulletDeathTimer = 3.0f;
    [SerializeField] public float cooldownTimer = 3f;
    private bool isAttacking = false;



    void Start()
    {
        player = GameObject.FindWithTag("Player");
        mBody = GetComponent<Rigidbody2D>();
        lineToPlayer = GetComponent<LineRenderer>();
        mAnimator = GetComponent<Animator>();
        Utils.AnimationState(mAnimator, "run");
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
                    case (State.Idle):
                        if (timer >= timeToMove)
                        {
                            Vector2 randomDir = Utils.RandomVector2(3.1415f * 2, 0f);
                            randomDir.Normalize();
                            mBody.velocity = randomDir * idleSpeed * Time.deltaTime;
                            timer = 0f;
                        }
                        break;

                    case (State.Aim):
                        mBody.velocity = Vector2.zero;
                        break;
                    case (State.Shoot):
                        mBody.velocity = Vector2.zero;
                        break;
                    case (State.Dead):
                        mBody.velocity = Vector2.zero;
                        break;
                }

            }
        }
    }

    void Update()
    {
        GameObject mObj = this.gameObject;
        if (mObj != null)
        {
            timer += Time.deltaTime;
            bool playerInRange = Utils.isPlayerClose(mObj, player, range);
            Utils.FlipSprites(mObj, player);
            if (mObj.transform.parent == null || !mObj.transform.parent.CompareTag("Weapon"))
            {
                if (!playerInRange)
                    currentState = State.Idle;
                if(playerInRange && !isAttacking)
                    currentState = State.Aim;
                else
                    currentState = State.Shoot;
                float eyeHP = mBody.GetComponent<HealthSystem>().getHealth();
                if (eyeHP <= 0)
                    currentState = State.Dead;

                switch (currentState)
                {
                    case (State.Idle):
                        Utils.AnimationState(mAnimator, "idle");
                        lineToPlayer.enabled = false;
                        break;

                    case (State.Aim):
                            //Debug.Log("hello aim here");
                        Utils.DrawLine(lineToPlayer, mObj, player);
                        AimAtPlayer(player, sniperWeapon);
                        if (timer >= aimTimer)
                        {
                            currentState = State.Shoot;
                            isAttacking = true;
                            timer = 0f;
                        }
                        break;
                    case (State.Shoot):
                        lineToPlayer.enabled = false;
                        if(timer >= cooldownTimer && playerInRange)
                        {
                            Shoot(player);
                            currentState = State.Idle;
                            isAttacking = false;
                            timer = 0f;
                        }
                        break;
                    case (State.Dead):
                        Utils.AnimationState(mAnimator, "dead");
                        mObj.layer = LayerMask.NameToLayer("Dead Objects");
                        lineToPlayer.enabled = false;
                        break;
                }
            }
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
    
    public void AimAtPlayer(GameObject player, GameObject weapon)
    {
        Vector3 aimDir = (player.transform.position - weapon.transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        if (angle > -90f && angle < 90f)
            weapon.GetComponent<SpriteRenderer>().flipY = false;
        else
            weapon.GetComponent<SpriteRenderer>().flipY = true;
        weapon.transform.eulerAngles = new Vector3(0, 0, angle);
    }

}
