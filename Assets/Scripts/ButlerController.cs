using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;


public class ButlerController : MonoBehaviour
{
    private enum State
    {
        Idle,
        Engaged,
        Struck,
        Dead,
    };

    private Rigidbody2D mBody;
    private GameObject player;
    private Animator mAnimator;
    private State currentState = State.Idle;
    private LineRenderer lineToPlayer;
    private List<GameObject> rats; 
    private float timer = 0.0f;
    private float spawnTimer = 0.0f;
    private float lastHP;

    [Header("Butler Parameters")]
    [SerializeField] private float range = 1f;
    [SerializeField] private float engagedSpeed = 1f;
    [SerializeField] private float timeToMove = 5f;
    [SerializeField] private float spawnCooldown = 3f;
    [SerializeField] private GameObject rat;
    [SerializeField] private int ratLimit = 6;
    [SerializeField] private int RatsToSpawn = 2;
    [SerializeField] private bool EnableLine = false;

    [Header("Debug")]
    [SerializeField] public bool _EditorShowRange = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        mBody = GetComponent<Rigidbody2D>();
        lineToPlayer = GetComponent<LineRenderer>();
        mAnimator = GetComponent<Animator>();
        rats = new List<GameObject>();
        Utils.AnimationState(mAnimator, "idle");
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        GameObject mObj = this.gameObject;

        if(mObj != null)
        {
            // Check if im grabbed by gravity gun
            if (mObj.transform.parent == null || !mObj.transform.parent.CompareTag("Weapon"))
            {
                switch (currentState)
                {
                    case (State.Idle):
                        mBody.velocity = Vector2.zero;
                        break;
                    case (State.Engaged):
                        if (timer >= timeToMove)
                        {
                            Vector3 playerDir = (player.transform.position - transform.position).normalized;
                            Vector3 randomDir = Utils.RandomVector2(3.1415f * 2, 0f).normalized;
                            Vector3 moveDir = playerDir + randomDir;
                            moveDir.Normalize();
                            mBody.velocity = moveDir * engagedSpeed * Time.fixedDeltaTime;
                            timer = 0f;
                        }
                        break;
                    case (State.Dead):
                        mBody.velocity = Vector2.zero;
                        break;
                }
            }
        }
    }
    private void Update()
    {
        spawnTimer += Time.deltaTime;
        GameObject mObj = this.gameObject;

        if (mObj != null)
        {
            Utils.FlipSprites(mObj, player);
            // Check if im grabbed by gravity gun
            if (mObj.transform.parent == null || !mObj.transform.parent.CompareTag("Weapon"))
            {
                if (!Utils.isPlayerClose(mObj, player, range))
                    currentState = State.Idle;
                else
                    currentState = State.Engaged;
                float butlerHP = mBody.GetComponent<HealthSystem>().getHealth();
                if (butlerHP != lastHP)
                    currentState = State.Struck;
                lastHP = butlerHP;
                if(butlerHP <= 0)
                    currentState = State.Dead;

                switch (currentState)
                {
                    case (State.Idle):
                        Utils.AnimationState(mAnimator, "idle");
                        lineToPlayer.enabled = false;
                        break;

                    case (State.Engaged):
                        if(EnableLine)
                            Utils.DrawLine(lineToPlayer, mObj, player);
                        checkAliveRats();
                        if (spawnTimer >= spawnCooldown && rats.Count < ratLimit)
                        {
                            SpawnRats();
                            Utils.AnimationState(mAnimator, "spawn");
                            spawnTimer = 0f;
                        }
                        else
                        {
                            Utils.AnimationState(mAnimator, "idle");
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

    public void SpawnRats()
    {
        if(rat != null)
        {
            for(int i = 0; i < RatsToSpawn; i++)
            {
                GameObject ratClone = Instantiate(rat, transform.position, Quaternion.identity);
                if(ratClone != null)
                    rats.Add(ratClone);
            }
        }
    }

    public void checkAliveRats()
    {
        if (rats != null)
        {
            for (int i = 0; i < rats.Count; i++)
            {
                if (rats[i] == null)
                { 
                    rats.RemoveAt(i);
                }
            }
        }
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
