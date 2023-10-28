using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

enum ratState
{
    Idle,
    Engaged,
    Dead,
};

public class RatController : MonoBehaviour
{
    private Rigidbody2D body;
    private GameObject player;
    private ratState currentState = ratState.Idle;
    private LineRenderer lineToPlayer;
    private Animator mAnimator;
    public AudioSource attackSound;
    [Header("Rat Parameters")]
    public float range = 1f;
    public float timeToMove = 5f;
    public float idleSpeed = 15f;
    public float engagedSpeed = 1f;
    public float attackSpeed = 10f;
    public float attackTime = 1f;
    public float attackCooldown = 5f;
    public float pounceTime = 0.25f;
    private bool isAttacking = false;
    private bool isPouncing = false;
    public bool EnableLine = false;
    float timer = 0.0f;
   

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        body = GetComponent<Rigidbody2D>();
        mAnimator = GetComponent<Animator>();
        lineToPlayer = GetComponent<LineRenderer>();
        body.velocity = new Vector2(1,1);
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        if(gameObject != null)
        {
            // Check if im grabbed by gravity gun
            if (gameObject.transform.parent == null || !gameObject.transform.parent.CompareTag("Weapon"))
            {
                switch (currentState)
                {
                    case (ratState.Idle):

                        if (timer >= timeToMove && !isAttacking && !isPouncing)
                        {
                            AnimationState("run");
                            Vector2 randomDir = RandomVector2(3.1415f * 2, 0f);
                            randomDir.Normalize();
                            body.velocity = randomDir * idleSpeed * Time.fixedDeltaTime;
                            timer = 0f;
                        }
                        
                        break;
                    case (ratState.Engaged):
                        if(!isAttacking && !isPouncing)
                        {
                            Vector2 movement = player.transform.position - transform.position;
                            movement.Normalize();
                            if (body.velocity != Vector2.zero)
                            {
                                body.velocity = movement * engagedSpeed * Time.fixedDeltaTime;
                                AnimationState("run");
                            }
                            else
                            {
                                //body.velocity = Vector2.zero;
                                AnimationState("idle");
                            }
                        }
                        break;

                    case (ratState.Dead):
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
        //Debug.Log(currentState);
        timer += Time.deltaTime;
        if (gameObject != null)
        {
            FlipSprites();
            if (gameObject.transform.parent == null || !gameObject.transform.parent.CompareTag("Weapon"))
            {
                if (!isPlayerClose())
                    currentState = ratState.Idle;
                else
                    currentState = ratState.Engaged;
                float butlerHP = body.GetComponent<HealthSystem>().getHealth();
                if (butlerHP <= 0)
                    currentState = ratState.Dead;

                switch (currentState)
                {
                    case (ratState.Idle):
                        lineToPlayer.enabled = false;
                        break;
                    case (ratState.Engaged):
                        if(EnableLine)
                            DrawLineToPlayer();
                        if(timer >= attackCooldown)
                        {
                            if (!isAttacking)
                            {
                                StartCoroutine(Pounce());
                            }
                            else if (!isPouncing)
                            {
                                AnimationState("jump");
                                StartCoroutine(Attack());
                                timer = 0f;
                            }
                        }
                        break;
                    case (ratState.Dead):
                        gameObject.layer = LayerMask.NameToLayer("Dead Objects");
                        lineToPlayer.enabled = false;
                        break;
                }
            }
        }
       
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        Vector2 attackDir = (player.transform.position - transform.position).normalized;
        
        body.AddForce(attackDir * attackSpeed);
        yield return new WaitForSeconds(attackTime);
        if (attackSound != null)
            attackSound.Play();
        isAttacking = false;
    }

    private IEnumerator Pounce()
    {
        isPouncing = true;
        isAttacking = true;
        AnimationState("pounce");
        body.velocity = Vector2.zero;
        yield return new WaitForSeconds(pounceTime);
        isPouncing = false;
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
            mAnimator.SetBool("jump", false);
            mAnimator.SetBool("pounce", false);
            mAnimator.SetBool(currState, true);
        }
    }
    public void FlipSprites()
    {
        body.GetComponent<SpriteRenderer>().flipX = (player.transform.position.x < transform.position.x);
    }
}
