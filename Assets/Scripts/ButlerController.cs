using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

enum ButlerState
{
    Idle,
    Engaged,
    Dead,
};

public class ButlerController : MonoBehaviour
{
    private Rigidbody2D body;
    private GameObject player;
    private SpriteRenderer sRenderer;
    private float timer = 0.0f;
    private ButlerState currentState = ButlerState.Idle;
    private LineRenderer lineToPlayer;
    [Header("Butler Parameters")]
    public float range = 1f;
    public float idleSpeed = 15f;
    public float engagedSpeed = 1f;
    public float timeToMove = 5f;
    public float spawnCooldown = 3f;
    public int ratLimit = 6;
    public int RatsToSpawn = 2;
    public GameObject rat;
    public Sprite butler_idle;
    public Sprite butler_struck;
    public Sprite butler_spawn;
    public Sprite butler_dead;
    public float drawTime = 0.5f;
    Sprite currSprite = null;
    private List<GameObject> rats = new List<GameObject>();

    public Slider healthBar;
   

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        body = GetComponent<Rigidbody2D>();
        lineToPlayer = GetComponent<LineRenderer>();
        sRenderer = GetComponent<SpriteRenderer>();
        sRenderer.sprite = butler_idle;
        currSprite = butler_idle;
    }

    void Update()
    {
        if (gameObject != null)
        {
            timer += Time.deltaTime;

            if(healthBar != null) 
                showHealthBar();

            // Check if im grabbed by gravity gun
            if (gameObject.transform.parent == null || !gameObject.transform.parent.CompareTag("Weapon"))
            {
                if (!isPlayerClose())
                    currentState = ButlerState.Idle;
                else
                    currentState = ButlerState.Engaged;

                float butlerHP = body.GetComponent<HealthSystem>().getHealth();
                if(butlerHP <= 0)
                    currentState = ButlerState.Dead;
                //Debug.Log(currentState);

                switch (currentState)
                {
                    case (ButlerState.Idle):
                        currSprite = butler_idle;
                        sRenderer.sprite = currSprite;
                        lineToPlayer.enabled = false;
                        if (timer >= timeToMove)
                        {
                            Vector2 randomDir = RandomVector2(3.1415f * 2, 0f);
                            randomDir.Normalize();
                            body.velocity = randomDir * idleSpeed * Time.deltaTime;
                            timer = 0f;
                        }
                        break;

                    case (ButlerState.Engaged):
                        DrawLineToPlayer();

                        // calculate distance to move
                        var step = engagedSpeed * Time.deltaTime;
                        transform.position = Vector3.MoveTowards(transform.position,
                            player.transform.position, step);
                        if (timer >= spawnCooldown && rats.Count < ratLimit - 1)
                        {
                            SpawnRats();
                            StartCoroutine(DrawSprite(butler_spawn, 0.01f));
                            
                            //Debug.Log(rats.Count);
                            timer = 0f;
                        }
                        break;

                    case (ButlerState.Dead):

                        Debug.Log("This is butlerstat dead hello!");
                        body.velocity = Vector2.zero;
                        gameObject.layer = LayerMask.NameToLayer("Dead Objects");
                        lineToPlayer.enabled = false;
                        currSprite = butler_dead;
                        sRenderer.sprite = currSprite;
                        break;
                }
            }
        }
    }

    public void SpawnRats()
    {
        GetAliveRats();
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

    public void GetAliveRats()
    {
        foreach(GameObject rat in rats)
        {
            if (rat == null)
                rats.Remove(rat);
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
    
    public IEnumerator DrawSprite(Sprite spriteToDraw, float drawTime)
    {
        currSprite = spriteToDraw;
        sRenderer.sprite = currSprite;
        yield return new WaitForSeconds(drawTime);
    }

}
