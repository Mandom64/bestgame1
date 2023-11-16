using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RatController : MonoBehaviour
{
    private enum State
    {
        Idle,
        Engaged,
        Dead,
    };

    private Rigidbody2D mBody;
    private GameObject player;
    private State currentState = State.Idle;
    private LineRenderer lineToPlayer;
    private Animator mAnimator;
    public AudioSource attackSound;
    private bool pauseMovement = false;
    private bool isAttacking = false;
    private bool isPouncing = false;
    public bool EnableLine = false;
    float timer = 0.0f;

    [Header("Rat Parameters")]
    [SerializeField] public float range = 1f;
    [SerializeField] public float timeToMove = 5f;
    [SerializeField] public float idleSpeed = 15f;
    [SerializeField] public float engagedSpeed = 1f;
    [SerializeField] public float attackSpeed = 10f;
    [SerializeField] public float attackTime = 1f;
    [SerializeField] public float attackCooldown = 5f;
    [SerializeField] public float pounceTime = 0.25f;

    [Header("Debug")]
    [SerializeField] public bool _EditorShowRange = false;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        mBody = GetComponent<Rigidbody2D>();
        mAnimator = GetComponent<Animator>();
        lineToPlayer = GetComponent<LineRenderer>();
        mBody.velocity = new Vector2(1,1);
    }

    private void FixedUpdate()
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
            Utils.FlipSprites(this.gameObject, player);

            // Check if im grabbed by gravity gun
            if (gameObject.transform.parent == null ||
                !gameObject.transform.parent.CompareTag("Weapon") &&
                !pauseMovement)
            {
                switch (currentState)
                {
                    case (State.Idle):
                        lineToPlayer.enabled = false;
                        //if (timer >= timeToMove && !isAttacking && !isPouncing)
                        //{
                        //    Utils.AnimationState(mAnimator, "run");
                        //    Vector2 randomDir = Utils.RandomVector2(3.1415f * 2, 0f).normalized;
                        //    mBody.AddForce(randomDir * idleSpeed); 
                        //    timer = 0f;
                        //}
                        mBody.velocity = Vector2.zero;
                        break;

                    case (State.Engaged):
                        if (!isAttacking && !isPouncing)
                        {
                            Vector2 moveDir = (player.transform.position - this.transform.position).normalized;
                            if (mBody.velocity != Vector2.zero)
                            {
                                //var step = engagedSpeed * Time.deltaTime;
                                //transform.position = Vector3.MoveTowards(transform.position,
                                //    player.transform.position, step);
                                //mBody.velocity = mBody.velocity * engagedSpeed * Time.fixedDeltaTime;
                                mBody.AddForce(moveDir * engagedSpeed);
                                Utils.AnimationState(mAnimator, "run");
                            }
                            else
                            {
                                //mBody.velocity = Vector2.zero;
                                mBody.AddForce(Vector2.zero);
                                Utils.AnimationState(mAnimator, "idle");
                            }
                        }
                        break;

                    case (State.Dead):
                        lineToPlayer.enabled = false;
                        Utils.AnimationState(mAnimator, "dead");
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

                        break;
                    case (State.Engaged):
                        if (EnableLine)
                            Utils.DrawLine(lineToPlayer, this.gameObject, player);
                        if (timer >= attackCooldown)
                        {
                            if (!isAttacking)
                            {
                                StartCoroutine(Pounce());
                            }
                            else if (!isPouncing)
                            {
                                Utils.AnimationState(mAnimator, "jump");
                                StartCoroutine(Attack());
                                timer = 0f;
                            }
                        }
                        break;
                    case (State.Dead):

                        break;
                }
            }
        }
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        Vector2 attackDir = (player.transform.position - transform.position).normalized;
        
        mBody.AddForce(attackDir * attackSpeed);
        yield return new WaitForSeconds(attackTime);
        if (attackSound != null)
            attackSound.Play();
        isAttacking = false;
    }

    private IEnumerator Pounce()
    {
        isPouncing = true;
        isAttacking = true;
        Utils.AnimationState(mAnimator, "pounce");
        mBody.velocity = Vector2.zero;
        yield return new WaitForSeconds(pounceTime);
        isPouncing = false;
    }

    private void OnDrawGizmos()
    {
        if(_EditorShowRange)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}
