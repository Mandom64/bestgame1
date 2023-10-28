using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

enum RatEXState
{
    Idle,
    Engaged,
    Dead,
};

public class RatEXController : MonoBehaviour
{
    private Rigidbody2D body;
    private GameObject player;
    private LineRenderer lineToPlayer;
    private Animator mAnimator;
    private RatEXState currentState = RatEXState.Idle;
    public AudioSource explosionSound;
    [Header("Rat Parameters")]
    public float range = 1f;
    public float timeToMove = 5f;
    public float idleSpeed = 15f;
    public float engagedSpeed = 1f;
    public bool EnableLine = false;
    [Header("Explosion Parameters")]
    public float damage = 50f;
    public float splashRange = 1f;
    public float playerExplosionDistance = 1f;
    public float flashDistance = 3f;
    private HealthSystem mHealth;
    public List<LayerMask> damageLayers = new List<LayerMask>();
    private bool hasExploded = false;
    float timer = 0.0f;


    void Start()
    {
        player = GameObject.FindWithTag("Player");
        body = GetComponent<Rigidbody2D>();
        mHealth = GetComponent<HealthSystem>();
        damageLayers.Add(LayerMask.NameToLayer("Player"));
        mAnimator = GetComponent<Animator>();
        lineToPlayer = GetComponent<LineRenderer>();
        body.velocity = new Vector2(1, 1);
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        if (gameObject != null)
        {
            // Check if im grabbed by gravity gun
            if (gameObject.transform.parent == null || !gameObject.transform.parent.CompareTag("Weapon"))
            {
                switch (currentState)
                {
                    case (RatEXState.Idle):
                        if (timer >= timeToMove)
                        {
                            AnimationState("run");
                            Vector2 randomDir = RandomVector2(3.1415f * 2, 0f);
                            randomDir.Normalize();
                            body.velocity = randomDir * idleSpeed * Time.fixedDeltaTime;
                            timer = 0f;
                        }
                        break;
                    case (RatEXState.Engaged):
                        Vector2 movement = player.transform.position - transform.position;
                        movement.Normalize();
                        if (body.velocity != Vector2.zero)
                        {
                            body.velocity = movement * engagedSpeed * Time.fixedDeltaTime;
                            if (PlayerDistance() <= flashDistance)
                                AnimationState("flash_run");
                            else
                                AnimationState("run");
                        }
                        else
                        {
                            body.velocity = Vector2.zero;
                            AnimationState("idle");
                        }
                        break;
                    case (RatEXState.Dead):
                        AnimationState("idle");
                        body.velocity = Vector2.zero;
                        gameObject.layer = LayerMask.NameToLayer("Dead Objects");
                        break;
                }
            }
        }
    }
    void Update()
    {
        timer += Time.deltaTime;
        if (gameObject != null)
        {
            FlipSprites();
            if (gameObject.transform.parent == null || !gameObject.transform.parent.CompareTag("Weapon"))
            {
                if (!isPlayerClose())
                    currentState = RatEXState.Idle;
                else
                    currentState = RatEXState.Engaged;
                float butlerHP = body.GetComponent<HealthSystem>().getHealth();
                if (butlerHP <= 0)
                    currentState = RatEXState.Dead;

                switch (currentState)
                {
                    case (RatEXState.Idle):
                        lineToPlayer.enabled = false;
                        break;
                    case (RatEXState.Engaged):
                        if (EnableLine)
                            DrawLineToPlayer();
                        if(PlayerDistance() <= playerExplosionDistance)
                        {
                            if (splashRange > 0 && !hasExploded)
                            {
                                Explode();
                            }
                        }
                        break;
                    case (RatEXState.Dead):
                        gameObject.layer = LayerMask.NameToLayer("Dead Objects");
                        lineToPlayer.enabled = false;
                        break;
                }
            }
        }
    }
    public void Explode()
    {
        foreach (LayerMask mask in damageLayers)
        {
            var hitColliders = Physics2D.OverlapCircleAll(this.transform.position, splashRange, mask);
            foreach (var hitCollider in hitColliders)
            {
                var closestPoint = hitCollider.ClosestPoint(this.transform.position);
                var distance = Vector3.Distance(closestPoint, this.transform.position);
                var damagePercent = Mathf.InverseLerp(splashRange, 0, distance);
                HealthSystem enemyHP = hitCollider.gameObject.GetComponent<HealthSystem>();
                Debug.Log(enemyHP.name + "is hit by exploding rat");
                if (enemyHP != null)
                {
                    enemyHP.Damage(damagePercent * damage);
                    Debug.Log(enemyHP.name + " took " + damagePercent * damage);
                }
            }
        }
        hasExploded = true;
        body.velocity = Vector2.zero;
        currentState = RatEXState.Dead;
        mAnimator.SetBool("explosion", true);
        if(explosionSound != null)
            explosionSound.Play();
        Destroy(this.gameObject, 1f);
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

    private float PlayerDistance()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        return distance;
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

    void AnimationState(string currState)
    {
        if (mAnimator != null)
        {
            mAnimator.SetBool("idle", false);
            mAnimator.SetBool("run", false);
            mAnimator.SetBool("flash_run", false);
            mAnimator.SetBool("explosion", false);
            mAnimator.SetBool(currState, true);
        }
    }
    public void FlipSprites()
    {
        body.GetComponent<SpriteRenderer>().flipX = (player.transform.position.x < transform.position.x);
    }
}
