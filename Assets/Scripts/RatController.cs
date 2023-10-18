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
    private RectTransform healthBarTransform;
    private ratState currentState = ratState.Idle;
    private LineRenderer lineToPlayer;
    public AudioSource attackSound;
    public Slider healthBar;
    [Header("Rat Parameters")]
    public float range = 1f;
    public float timeToMove = 5f;
    public float idleSpeed = 15f;
    public float engagedSpeed = 1f;
    public float attackSpeed = 10f;
    public float attackTime = 1f;
    public float attackCooldown = 5f;
    private bool isAttacking = false;
    float timer = 0.0f;
   

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        body = GetComponent<Rigidbody2D>();
        lineToPlayer = GetComponent<LineRenderer>();
        body.velocity = new Vector2(1,1);
    }

    private void FixedUpdate()
    {
        if(gameObject != null)
        {
            // Check if im grabbed by gravity gun
            if (gameObject.transform.parent == null || !gameObject.transform.parent.CompareTag("Weapon"))
            {
                switch (currentState)
                {
                    case (ratState.Idle):

                        if (timer >= timeToMove && !isAttacking)
                        {
                            Vector2 randomDir = RandomVector2(3.1415f * 2, 0f);
                            randomDir.Normalize();
                            body.velocity = randomDir * idleSpeed * Time.fixedDeltaTime;
                            timer = 0f;
                        }
                        break;

                    case (ratState.Engaged):

                        // calculate distance to move
                        if (!isAttacking)
                        {
                            Vector2 movement = player.transform.position - transform.position;
                            movement.Normalize();
                            if (body.velocity != Vector2.zero)
                                body.velocity = movement * engagedSpeed * Time.fixedDeltaTime;
                            else
                                body.velocity = Vector2.zero;
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
        timer += Time.deltaTime;

        if (gameObject != null)
        {
            if (gameObject.transform.parent == null || !gameObject.transform.parent.CompareTag("Weapon"))
            {
                //Debug.Log("rat is attacking = " + isAttacking);
                //Debug.Log("rat stat is = "  + currentState);
                if (!isPlayerClose())
                    currentState = ratState.Idle;
                else
                    currentState = ratState.Engaged;
                switch (currentState)
                {
                    case (ratState.Idle):

                        lineToPlayer.enabled = false;
                        break;
                    case (ratState.Engaged):

                        DrawLineToPlayer();
                        if (timer >= attackCooldown && !isAttacking)
                        {
                            StartCoroutine(Attack());
                            timer = 0f;
                        }

                        break;
                    case (ratState.Dead):
                        body.velocity = Vector2.zero;
                        gameObject.layer = LayerMask.NameToLayer("Dead Objects");
                        lineToPlayer.enabled = false;
                        break;
                }
                if (healthBar != null)
                    showHealthBar();
            }
        }
       
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        body.AddForce(body.velocity * attackSpeed * Time.fixedDeltaTime);
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
