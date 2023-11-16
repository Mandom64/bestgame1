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

    private float timer = 0f;
    private Rigidbody2D mBody;
    private Animator mAnimator;
    private HealthSystem mHealth;

    [Header("Player parameters")]
    [SerializeField] private GameObject inventoryObj;
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float baseWallDamage = 1f;
    [SerializeField] private float wallDamageMultiplier = 10f;

    [Header("Audio sounds")]
    [SerializeField] public AudioSource dashSound;
    [SerializeField] public AudioSource itemTakenSound;
    [SerializeField] private List<GameObject> inventory;
    [SerializeField] public int currItem = 0;

    [Header("Grabbing parameters")]
    [SerializeField] private LayerMask grabbableLayer;
    [SerializeField] private float grabDistance = 2.5f;
    [SerializeField] private float grabCooldown = 1f;
    [SerializeField] private Vector2 grabRayThickness = new Vector2(0.5f, 0.5f);

    [Header("Dashing parameters")]
    [SerializeField] private float dashForce = 50.0f;
    [SerializeField] private float dashTime = 0.25f;
    [SerializeField] private float dashCooldown = 3f;
    private bool isDashing = false;

    void Start()
    {
        Application.targetFrameRate = 144;
        mBody = GetComponent<Rigidbody2D>();
        mAnimator = GetComponent<Animator>();
        mHealth = gameObject.GetComponent<HealthSystem>();
        Inventory g_inventory = inventoryObj.GetComponent<Inventory>();
        inventory = g_inventory.inventoryList;
        grabbableLayer = LayerMask.GetMask("Grabbable Items");
        Utils.AnimationState(mAnimator, "idle");
    }

    void FixedUpdate()
    {
        if(!isDashing)
        {
            MovePlayer();
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

    void Update()
    {
        timer += Time.deltaTime;

        // When player dies load the game over scene
        if(mHealth.getHealth() <= 0f)
        {
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
        {
            mBody.velocity = movement * moveSpeed;
            Utils.AnimationState(mAnimator, "run");
        }
        else
        {
            mBody.velocity = Vector2.zero;
            Utils.AnimationState(mAnimator, "idle");
        }
    }

    public void FlipSprites()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        if(horizontalInput > 0f)
            mBody.GetComponent<SpriteRenderer>().flipX = true;
        else if(horizontalInput < 0f)
            mBody.GetComponent<SpriteRenderer>().flipX = false;
    }

    public void GrabObject()
    {
        // Check if grabbable item is around and grab it
        RaycastHit2D hit = Physics2D.BoxCast(transform.position,
            grabRayThickness, 0.0f, transform.up, grabDistance, grabbableLayer);
        if(hit.collider == null)
            hit = Physics2D.BoxCast(transform.position,
            grabRayThickness, 0.0f, transform.right, grabDistance, grabbableLayer);
        if(hit.collider == null)
            hit = Physics2D.BoxCast(transform.position,
            grabRayThickness, 0.0f, -transform.up, grabDistance, grabbableLayer);
        if (hit.collider == null)
            hit = Physics2D.BoxCast(transform.position,
            grabRayThickness, 0.0f, -transform.right, grabDistance, grabbableLayer);

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

    // Handle damage from bullets and collisions
    public void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject objectHit = collision.gameObject;
        if (objectHit.layer == LayerMask.NameToLayer("Enemy Projectiles"))
        {
            if (objectHit.CompareTag("Bullet"))
            {
                
                float damageAmount = objectHit.gameObject.GetComponent<Damage>().damage;
                if (mHealth != null)
                {
                    mHealth.Damage(damageAmount);
                    Destroy(objectHit);
                    Debug.Log(mHealth + " took " + damageAmount + " from a Bullet");
                }
            }
        }
        if(objectHit.layer == LayerMask.NameToLayer("Enemies"))
        {
            if(objectHit.CompareTag("Rat"))
            {
                float damageAmount = objectHit.gameObject.GetComponent<Damage>().damage;
                if (mHealth != null)
                {
                    mHealth.Damage(damageAmount);
                    Debug.Log(mHealth + " took " + damageAmount + " from a Rat");
                }
            }
        }
    }
    
    private IEnumerator Dash()
    {
        isDashing = true;
        mBody.AddForce(mBody.velocity * dashForce);
        yield return new WaitForSeconds(dashTime);
        if(dashSound != null)
            dashSound.Play();
        isDashing = false;
    }
}
