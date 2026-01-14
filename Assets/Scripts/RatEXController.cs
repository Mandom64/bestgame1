using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class RatEXController : MonoBehaviour
{
    private enum State
    {
        Idle,
        Engaged,
        Dead,
    };

    private State currentState = State.Idle;
    private HealthSystem mHealth;
    private Rigidbody2D mBody;
    private GameObject player;
    private Animator mAnimator;
    private LineRenderer lineToPlayer;
    private float timer = 0.0f;

    [Header("Rat Parameters")]
    [SerializeField] private float range = 1f;
    [SerializeField] private float timeToMove = 5f;
    [SerializeField] private float idleSpeed = 15f;
    [SerializeField] private float engagedSpeed = 1f;
    [SerializeField] private bool EnableLine = false;

    [Header("Explosion Parameters")]
    [SerializeField] private AudioSource explosionSound;
    [SerializeField] private float damage = 50f;
    [SerializeField] private float explosionForce = 100f;
    [SerializeField] private float splashRange = 1f;
    [SerializeField] private float playerExplosionDistance = 1f;
    [SerializeField] private float flashDistance = 3f;
    public LayerMask damageLayers;
    private bool hasExploded = false;

    [Header("Debug")]
    [SerializeField] public bool _EditorShowRange = false;


    void Start()
    {
        player = GameObject.FindWithTag("Player");
        mBody = GetComponent<Rigidbody2D>();
        mHealth = GetComponent<HealthSystem>();
        mAnimator = GetComponent<Animator>();
        lineToPlayer = GetComponent<LineRenderer>();
        mBody.velocity = new Vector2(1, 1);
    }

    void FixedUpdate()
    {
        timer += Time.deltaTime;
        FixedBehavior();
    }
    void Update()
    {
        timer += Time.deltaTime;
        ContBehavior();
    }

    private void FixedBehavior()
    {
        if (gameObject != null)
        {
            // Check if im grabbed by gravity gun
            if (gameObject.transform.parent == null || !gameObject.transform.parent.CompareTag("Weapon"))
            {
                switch (currentState)
                {
                    case (State.Idle):
                        //if (timer >= timeToMove)
                        //{
                        //    Utils.AnimationState(mAnimator, "run");
                        //    Vector2 randomDir = Utils.RandomVector2(3.1415f * 2, 0f);
                        //    randomDir.Normalize();
                        //    mBody.velocity = randomDir * idleSpeed * Time.fixedDeltaTime;
                        //    timer = 0f;
                        //}
                        mBody.velocity = Vector2.zero;
                        break;
                    case (State.Engaged):
                        Vector2 movement = player.transform.position - transform.position;
                        movement.Normalize();
                        if (mBody.velocity != Vector2.zero)
                        {
                            mBody.velocity = movement * engagedSpeed * Time.fixedDeltaTime;
                            if (Utils.PlayerDistance(this.gameObject, player) <= flashDistance)
                                Utils.AnimationState(mAnimator, "flash_run");
                            else
                                Utils.AnimationState(mAnimator, "run");
                        }
                        else
                        {
                            mBody.velocity = Vector2.zero;
                            Utils.AnimationState(mAnimator, "idle");
                        }
                        break;
                    case (State.Dead):
                        Utils.AnimationState(mAnimator, "idle");
                        mBody.velocity = Vector2.zero;
                        gameObject.layer = LayerMask.NameToLayer("Dead Objects");
                        break;
                }
            }
        }
    }

    private void ContBehavior()
    {
        if (gameObject != null)
        {
            Utils.FlipSprites(this.gameObject, player);
            if (gameObject.transform.parent == null || !gameObject.transform.parent.CompareTag("Weapon"))
            {
                if (!Utils.isPlayerClose(this.gameObject, player, range))
                    currentState = State.Idle;
                else
                    currentState = State.Engaged;
                float butlerHP = mBody.GetComponent<HealthSystem>().getHealth();
                if (butlerHP <= 0)
                    currentState = State.Dead;

                switch (currentState)
                {
                    case (State.Idle):
                        lineToPlayer.enabled = false;
                        break;
                    case (State.Engaged):
                        if (EnableLine)
                            Utils.DrawLine(lineToPlayer, this.gameObject, player);
                        if (Utils.PlayerDistance(this.gameObject, player) <= playerExplosionDistance)
                        {
                            if (splashRange > 0 && !hasExploded)
                            {
                                Explode();
                            }
                        }
                        break;
                    case (State.Dead):
                        gameObject.layer = LayerMask.NameToLayer("Dead Objects");
                        lineToPlayer.enabled = false;
                        break;
                }
            }
        }
    }

    private void Explode()
    {
        if (!hasExploded)
        {
            var hits = Physics2D.OverlapCircleAll(this.transform.position, splashRange, damageLayers);
            foreach (var hit in hits)
            {
                GameObject objectHit = null;
                if (hit.gameObject != null)
                    objectHit = hit.gameObject;
                if (objectHit != null && objectHit != this.gameObject)
                {
                    Rigidbody2D objectHitBody = objectHit.GetComponent<Rigidbody2D>();
                    if (objectHitBody != null)
                    {
                        var forceDir = (objectHit.transform.position - this.transform.position).normalized;
                        objectHitBody.AddForce(explosionForce * forceDir, ForceMode2D.Impulse);
                    }
                    var closestPoint = hit.ClosestPoint(this.transform.position);
                    var distance = Vector3.Distance(closestPoint, this.transform.position);
                    var damagePercent = Mathf.InverseLerp(splashRange, 0, distance);
                    HealthSystem enemyHP = objectHit.GetComponent<HealthSystem>();
                    if (enemyHP != null)
                    {
                        enemyHP.Damage(damagePercent * damage);
                        EnemyHealthbar enemyHealthbar = objectHit.GetComponentInChildren<EnemyHealthbar>();
                        if (enemyHealthbar != null)
                            enemyHealthbar.UpdateHealthbarUI(enemyHP.getHealth(), enemyHP.getHealthMax());
                        Debug.Log(enemyHP.name + " took " + damagePercent * damage + " from an explosive rat!!!");
                    }
                }
            }

            hasExploded = true;
            mAnimator.SetBool("explode", true);
            mBody.velocity = Vector2.zero;
            if (explosionSound != null)
                explosionSound.Play();
            Destroy(this.gameObject, 0.75f);
        }
    }

    private void OnDrawGizmos()
    {
        if (_EditorShowRange)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}
