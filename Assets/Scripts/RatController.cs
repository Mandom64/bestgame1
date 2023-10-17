using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
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
    private float timer = 0.0f;
    private RectTransform healthBarTransform;
    private ratState currentState = ratState.Idle;
    private LineRenderer lineToPlayer;
    public AudioSource attackSound;
    [Header("Rat Parameters")]
    public float range = 1f;
    public float idleSpeed = 15f;
    public float engagedSpeed = 1f;
    public float timeToMove = 5f;
    public float attackCooldown = 1f;
    public float attackTime = 1f;
    public float attackForce = 10f;
    public Slider healthBar;
    private bool isAttacking = false;
   

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        body = GetComponent<Rigidbody2D>();
        lineToPlayer = GetComponent<LineRenderer>();
    }

    private void FixedUpdate()
    {
        if (gameObject != null)
        {
            timer += Time.deltaTime;

            if (healthBar != null)
                showHealthBar();

            // Check if im grabbed by gravity gun
            if (gameObject.transform.parent == null || !gameObject.transform.parent.CompareTag("Weapon"))
            {
                if (!isPlayerClose())
                    currentState = ratState.Idle;
                else
                    currentState = ratState.Engaged;
                //Debug.Log(currentState);

                switch (currentState)
                {
                    case (ratState.Idle):
                        lineToPlayer.enabled = false;
                        if (timer >= timeToMove && !isAttacking)
                        {
                            Vector2 randomDir = RandomVector2(3.1415f * 2, 0f);
                            randomDir.Normalize();
                            body.velocity = randomDir * idleSpeed * Time.deltaTime;
                            timer = 0f;
                        }
                        break;

                    case (ratState.Engaged):
                        DrawLineToPlayer();

                        // calculate distance to move
                        if (!isAttacking)
                        {
                            var step = engagedSpeed * Time.deltaTime;
                            transform.position = Vector3.MoveTowards(transform.position,
                                player.transform.position, step);
                        }
                        if (timer >= attackCooldown && !isAttacking)
                        {
                            StartCoroutine(Attack());
                            timer = 0f;
                        }
                        break;

                    case (ratState.Dead):
                        break;
                }
            }
        }
    }
    void Update()
    {
        
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        body.AddForceAtPosition(body.velocity.normalized * attackForce, player.transform.position);
        yield return new WaitForSeconds(attackTime);
        if (attackSound != null)
            attackSound.Play();
        isAttacking = false;
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

    private void showHealthBar()
    {
        // Healthbar slider and text update
        if (gameObject != null)
        {
            HealthSystem playerHealth = gameObject.GetComponent<HealthSystem>();
            float healthPercentage = (float)playerHealth.getHealth() / (float)playerHealth.getHealthMax();
            healthBar.value = healthPercentage;
            Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            Vector2 aboveSprite = screenPos;
            aboveSprite.y += 25f;
            // Update the position of the Slider's RectTransform
            healthBar.transform.position = aboveSprite;
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
