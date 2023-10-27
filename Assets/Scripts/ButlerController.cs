using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

enum ButlerState
{
    Idle,
    Engaged,
    Struck,
    Dead,
};

public class ButlerController : MonoBehaviour
{
    private Rigidbody2D body;
    private GameObject player;
    private Animator mAnimator;
    private ButlerState currentState = ButlerState.Idle;
    private LineRenderer lineToPlayer;
    private float timer = 0.0f;
    private float lastHP;
    [Header("Butler Parameters")]
    public float range = 1f;
    public float idleSpeed = 15f;
    public float engagedSpeed = 1f;
    public float timeToMove = 5f;
    public float spawnCooldown = 3f;
    public int ratLimit = 6;
    public int RatsToSpawn = 2;
    public bool EnableLine = false;
    public GameObject rat;
    private List<GameObject> rats = new List<GameObject>();
   

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        body = GetComponent<Rigidbody2D>();
        lineToPlayer = GetComponent<LineRenderer>();
        mAnimator = GetComponent<Animator>();
        mAnimator.SetBool("idle", true);
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
                    case (ButlerState.Idle):
                        if (timer >= timeToMove)
                        {
                            Vector2 randomDir = RandomVector2(3.1415f * 2, 0f);
                            randomDir.Normalize();
                            body.velocity = randomDir * idleSpeed * Time.fixedDeltaTime;
                            timer = 0f;
                        }
                        break;
                    case (ButlerState.Engaged):
                        Vector2 movement = player.transform.position - transform.position;
                        movement.Normalize();
                        if (body.velocity != Vector2.zero)
                            body.velocity = movement * engagedSpeed * Time.fixedDeltaTime;
                        else
                            body.velocity = Vector2.zero;
                        break;
                    case (ButlerState.Dead):
                        body.velocity = Vector2.zero;
                        break;
                }
            }
        }
    }
    private void Update()
    {
        if (gameObject != null)
        {
            timer += Time.deltaTime;

            FlipSprites();
            // Check if im grabbed by gravity gun
            if (gameObject.transform.parent == null || !gameObject.transform.parent.CompareTag("Weapon"))
            {
                if (!isPlayerClose())
                    currentState = ButlerState.Idle;
                else
                    currentState = ButlerState.Engaged;
                float butlerHP = body.GetComponent<HealthSystem>().getHealth();
                if (butlerHP != lastHP)
                    currentState = ButlerState.Struck;
                lastHP = butlerHP;
                if(butlerHP <= 0)
                    currentState = ButlerState.Dead;

                switch (currentState)
                {
                    case (ButlerState.Idle):
                        AnimationState("idle");
                        lineToPlayer.enabled = false;
                        break;

                    case (ButlerState.Engaged):
                        if(EnableLine)
                            DrawLineToPlayer();
                        checkAliveRats();
                        if (timer >= spawnCooldown && rats.Count < ratLimit)
                        {
                            SpawnRats();
                            AnimationState("spawn");
                            timer = 0f;
                        }
                        else
                        {
                            AnimationState("idle");
                        }
                        break;
                    case (ButlerState.Struck):
                        AnimationState("struck");
                        break;
                    case (ButlerState.Dead):
                        AnimationState("dead");
                        gameObject.layer = LayerMask.NameToLayer("Dead Objects");
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
        if(mAnimator != null)
        {
            mAnimator.SetBool("idle", false);
            mAnimator.SetBool("struck", false);
            mAnimator.SetBool("spawn", false);
            mAnimator.SetBool("dead", false);
            mAnimator.SetBool(currState, true);
        }
    }
    public void FlipSprites()
    {
        body.GetComponent<SpriteRenderer>().flipX = (player.transform.position.x < transform.position.x);
    }
}
