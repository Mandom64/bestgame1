using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public class EyeController : MonoBehaviour
{
    private enum State
    {
        Idle,
        Engaged,
        Struck,
        Dead,
    };

    private State currentState = State.Idle;
    private Rigidbody2D mBody;
    private Animator mAnimator;
    private GameObject player;
    private LineRenderer lineToPlayer;
    private float timer = 0.0f;
    private float lastHP;

    [Header("Eye Parameters")]
    [SerializeField] public float range = 1f;
    [SerializeField] public float idleSpeed = 1f;
    [SerializeField] public float engagedSpeed = 15f;
    [SerializeField] public float timeToMove = 5f;
    [SerializeField] public Slider healthBar;
    [SerializeField] public bool EnableLine = false;

    [Header("Bullet Parameters")]
    [SerializeField] public GameObject bullet;
    [SerializeField] public float bulletSpeed = 15.0f;
    [SerializeField] public float bulletDeathTimer = 3.0f;
    [SerializeField] public float cooldownTimer = 0.5f;
    [SerializeField] public Sprite eye_dead;
    
    

    void Start()
    {
        mBody = GetComponent<Rigidbody2D>();
        mAnimator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
        lineToPlayer = GetComponent<LineRenderer>();
        Utils.AnimationState(mAnimator, "running");
    }

    void FixedUpdate()
    {
        FixedBehavior();
    }

    void Update()
    {
        ContBehavior();
    }

    private void FixedBehavior()
    {
        GameObject mObj = this.gameObject;
        if (mObj != null)
        {
            timer += Time.deltaTime;
            // Check if im grabbed by gravity gun
            if (mObj.transform.parent == null || !mObj.transform.parent.CompareTag("Weapon"))
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

                    case (State.Engaged):
                        // calculate distance to move
                        var step = engagedSpeed * Time.deltaTime;
                        transform.position = Vector3.MoveTowards(mObj.transform.position,
                            player.transform.position, step);
                        break;
                    case (State.Struck):
                        break;
                    case (State.Dead):
                        mBody.velocity = Vector2.zero;
                        break;
                }
            }
        }
    }
    private void ContBehavior()
    {
        GameObject mObj = this.gameObject;

        if (mObj != null)
        {
            timer += Time.deltaTime;
            Utils.FlipSprites(mObj, player);

            // Check if im grabbed by gravity gun
            if (mObj.transform.parent == null || !mObj.transform.parent.CompareTag("Weapon"))
            {
                if (!Utils.isPlayerClose(mObj, player, range))
                    currentState = State.Idle;
                else
                    currentState = State.Engaged;
                float eyeHP = mBody.GetComponent<HealthSystem>().getHealth();
                if (eyeHP != lastHP)
                    currentState = State.Struck;
                lastHP = eyeHP;
                if (eyeHP <= 0)
                    currentState = State.Dead;

                switch (currentState)
                {
                    case (State.Idle):
                        Utils.AnimationState(mAnimator, "running");
                        lineToPlayer.enabled = false;
                        break;

                    case (State.Engaged):
                        if (EnableLine)
                            Utils.DrawLine(lineToPlayer, mObj, player);
                        Utils.AnimationState(mAnimator, "running");
                        if (timer >= cooldownTimer)
                        {
                            Shoot(player);
                            timer = 0f;
                        }
                        break;
                    case (State.Struck):
                        Utils.AnimationState(mAnimator, "struck");
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
}
