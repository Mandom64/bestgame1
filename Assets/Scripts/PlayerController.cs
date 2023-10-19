using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private KeyCode[] inventoryNumbers = {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
        KeyCode.Alpha0,
    };

    [Header("Player parameters")]
    public Rigidbody2D body;
    public GameObject inventoryObject;
    public float moveSpeed = 5.0f;
    public float baseWallDamage = 1f;
    public float wallDamageMultiplier = 10f;
    [Header("Audio sounds")]
    public AudioSource dashSound;
    public AudioSource itemTakenSound;
    private List<GameObject> inventory;
    public int currItem = 0;
    [Header("Grabbing parameters")]
    public LayerMask grabbableLayer;
    public float grabDistance = 2.5f;
    public float grabCooldown = 1f;
    [Header("Dashing parameters")]
    public float dashForce = 50.0f;
    public float dashTime = 0.25f;
    public float dashCooldown = 3f;
    private bool isDashing = false;
    private float timer = 0f;
    private Animator mAnimator;
    HealthSystem playerHP;

    void Start()
    {
        Application.targetFrameRate = 1000;
        Inventory g_inventory = inventoryObject.GetComponent<Inventory>();
        inventory = g_inventory.inventoryList;
        body = GetComponent<Rigidbody2D>();
        playerHP = gameObject.GetComponent<HealthSystem>();
        grabbableLayer = LayerMask.GetMask("Grabbable Items");
        mAnimator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if(!isDashing)
        {
            MovePlayer();
            //if (mAnimator != null)
                //mAnimator.SetTrigger("run");
        }

        // Debugging button
        if (Input.GetKeyDown(KeyCode.T))
        {
            for(int i = 0; i < inventory.Count; i++)
            {
                Debug.Log("Item " + i + " is " + inventory[i]);
            }
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if(playerHP.getHealth() <= 0f)
        {
            // When player dies load the game over scene
           SceneManager.LoadScene(1);
        }

        FlipSprites();

        if (Input.GetKeyDown(KeyCode.Space)
            && !isDashing && timer >= dashCooldown)
        {
            StartCoroutine(Dash());
            timer = 0f;
        }

        if (Input.GetKeyDown(KeyCode.E) && timer >= grabCooldown)
        {
            GrabObject();
            timer = 0f;
        }

        if (Input.mouseScrollDelta.y != 0f)
            ScrollSwapItems();
        for (int i = 0; i < inventoryNumbers.Length; i++)
        {
            if (Input.GetKeyDown(inventoryNumbers[i]))
            {
                if (inventory.Count > 0)
                {
                    inventory[currItem].SetActive(false);
                    inventory[i].SetActive(true);
                    currItem = i;
                    break;
                }
            }
        }
    }

    public void MovePlayer()
    {
        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        movement.Normalize();
        if(movement != Vector2.zero)
            body.velocity = movement * moveSpeed;
        else
            body.velocity = Vector2.zero;
    }

    public void FlipSprites()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        if(horizontalInput > 0f)
            body.GetComponent<SpriteRenderer>().flipX = true;
        else if(horizontalInput < 0f)
            body.GetComponent<SpriteRenderer>().flipX = false;
    }

    public void GrabObject()
    {
        // Check if grabbable item is around and grab it
        RaycastHit2D hit = Physics2D.Raycast(transform.position,
            transform.up, grabDistance, grabbableLayer);
        if(hit.collider == null)
            hit = Physics2D.Raycast(transform.position,
            transform.right, grabDistance, grabbableLayer);
        if(hit.collider == null)
            hit = Physics2D.Raycast(transform.position,
            -transform.up, grabDistance, grabbableLayer);
        if (hit.collider == null)
            hit = Physics2D.Raycast(transform.position,
            -transform.right, grabDistance, grabbableLayer);

        if (hit.collider != null)
        {
            if (inventory.Count != 0)
            {
                Debug.Log("currItem=" + currItem);
                inventory[currItem].SetActive(false);
            }
            Debug.Log(hit.collider.gameObject + " is hit!!");
            GameObject grabbedObject = hit.collider.gameObject;
            grabbedObject.GetComponent<Rigidbody2D>().isKinematic = true;
            grabbedObject.transform.position = transform.position;
            grabbedObject.transform.parent = transform;
            grabbedObject.layer = LayerMask.NameToLayer("Inventory Items");
            inventory.Add(grabbedObject);
            currItem = inventory.Count - 1;
            Debug.Log("current item is " + currItem);

            if(itemTakenSound != null)
                itemTakenSound.Play();
        }
        if(inventory.Count != 0)
            inventory[currItem].SetActive(true);
    }
    public void ScrollSwapItems()
    {
        // Check if scroll wheel is moved, 
        // then change current item used by player
        if (inventory.Count > 0)
        {
            inventory[currItem].SetActive(false);
            if (Input.mouseScrollDelta.y > 0f)
            {
                currItem = (currItem - 1 + inventory.Count) % inventory.Count;
            }
            else if (Input.mouseScrollDelta.y < 0f)
            {
                currItem = (currItem + 1) % inventory.Count;
            }
            inventory[currItem].SetActive(true);
        }
    }

    // Handle damage
    public void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject objectHit = collision.gameObject;
        if (objectHit.layer == LayerMask.NameToLayer("Enemy Projectiles"))
        {
            if (objectHit.CompareTag("Bullet"))
            {
                
                float damageAmount = objectHit.gameObject.GetComponent<Damage>().damage;
                if (playerHP != null)
                {
                    playerHP.Damage(damageAmount);
                    Destroy(objectHit);
                    Debug.Log(playerHP + " took " + damageAmount + " from a Bullet()");
                }
            }
        }
    }
    
    private IEnumerator Dash()
    {
        isDashing = true;
        body.AddForce(body.velocity * dashForce);
        yield return new WaitForSeconds(dashTime);
        if(dashSound != null)
            dashSound.Play();
        isDashing = false;
    }
}
