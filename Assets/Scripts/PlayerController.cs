using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player parameters")]
    public Rigidbody2D my_rb;
    public GameObject inventoryObject;
    public float moveSpeed = 5.0f;
    private List<GameObject> inventory;
    private int currItem = 0;
    [Header("Grabbing parameters")]
    public LayerMask grabbableLayer;
    public float grabDistance = 2.5f;
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
        timer += Time.deltaTime;
        if(!isDashing)
            MovePlayer();

        if (Input.GetKey(KeyCode.E))
            GrabObject();
        if (Input.mouseScrollDelta.y != 0f)
            SwapItems();
        if (Input.GetKey(KeyCode.Space) 
            && !isDashing && timer >= dashCooldown)
        {
            StartCoroutine(Dash());
            timer = 0f;
        }

        // Debugging button
        if (Input.GetKey(KeyCode.T))
        {
            for(int i = 0; i < inventory.Count; i++)
            {
                Debug.Log("Item " + i + " is " + inventory[i]);
            }
        }
        Debug.Log(isDashing);
    }

    public void MovePlayer()
    {
        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        movement.Normalize();
        Debug.Log(movement);
        if(movement != Vector2.zero)
            my_rb.velocity = movement * moveSpeed;
        else
            my_rb.velocity = Vector2.zero;
    }

    public void GrabObject()
    {
        // Check if grabbable item is in front and grab it
        RaycastHit2D hit = Physics2D.Raycast(transform.position,
            transform.right, grabDistance, grabbableLayer);

        if (hit.collider != null)
        {
            Debug.Log("check");
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
        }
        inventory[currItem].SetActive(true);
    }
    public void SwapItems()
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

    //method to dash
    private IEnumerator Dash()
    {
        isDashing = true;
        my_rb.AddForce(my_rb.velocity * dashForce);
        yield return new WaitForSeconds(dashTime);
        Debug.Log("dashed");
        isDashing = false;
    }
}
