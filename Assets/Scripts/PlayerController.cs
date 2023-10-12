using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    private KeyCode[] keyboardNumbers = {
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
    public Rigidbody2D my_rb;
    public GameObject inventoryObject;
    public float moveSpeed = 5.0f;
    [Header("Audio sounds")]
    public AudioSource dashSound;
    public AudioSource itemTakenSound;
    private List<GameObject> inventory;
    private int currItem = 0;
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
                                                                
    void Start()
    {
        Application.targetFrameRate = 9999;
        Inventory g_inventory = inventoryObject.GetComponent<Inventory>();
        inventory = g_inventory.inventoryList;
        currItem = g_inventory.currItem;
        my_rb = GetComponent<Rigidbody2D>();
        grabbableLayer = LayerMask.GetMask("Grabbable Items");
    }

    void FixedUpdate()
    {
        if(!isDashing)
            MovePlayer();

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
        for (int i = 0; i < keyboardNumbers.Length; i++)
        {
            if (Input.GetKeyDown(keyboardNumbers[i]))
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
            my_rb.velocity = movement * moveSpeed;
        else
            my_rb.velocity = Vector2.zero;
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

    private IEnumerator Dash()
    {
        isDashing = true;
        my_rb.AddForce(my_rb.velocity * dashForce);
        yield return new WaitForSeconds(dashTime);
        if(dashSound != null)
            dashSound.Play();
        isDashing = false;
    }
}
