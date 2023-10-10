using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D my_rb;
    public GameObject inventoryObject;
    public float moveSpeed = 5.0f;
    public LayerMask grabbableLayer;
    public float grabDistance = 2.5f;
    private List<GameObject> inventory;
    private int currItem = 0; 

    void Start()
    {
        Inventory g_inventory = inventoryObject.GetComponent<Inventory>();
        inventory = g_inventory.inventoryList;
        currItem = g_inventory.currItem;
        my_rb = GetComponent<Rigidbody2D>();
        grabbableLayer = LayerMask.GetMask("Grabbable Items");
    }

    void Update()
    {
        // Store user input as a movement vector
        Vector3 userInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);

        // Check if grabbable item is in front and grab it
        if(Input.GetKey(KeyCode.E))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, 
                transform.right, grabDistance, grabbableLayer);

            if(hit.collider != null)
            {
                Debug.Log("check");
                if(inventory.Count != 0)
                {
                    Debug.Log("currItem="+currItem);
                    inventory[currItem].SetActive(false);
                }
                Debug.Log(hit.collider.gameObject + " is hit!!");
                GameObject grabbedObject = hit.collider.gameObject;
                grabbedObject.GetComponent<Rigidbody2D>().isKinematic = true;
                grabbedObject.transform.position = transform.position;
                grabbedObject.transform.parent = transform;
                grabbedObject.layer = LayerMask.NameToLayer("Inventory Items");
                inventory.Add(grabbedObject);
                currItem = inventory.Count-1;
                Debug.Log("current item is " + currItem);
            }
            inventory[currItem].SetActive(true);
        }

        // Check if scroll wheel is moved, 
        // then change current item used by player
        if(Input.mouseScrollDelta.y != 0f)
        {
            if(inventory.Count > 0)
            {
                inventory[currItem].SetActive(false);
                if(Input.mouseScrollDelta.y > 0f)
                {
                    currItem = (currItem - 1 + inventory.Count) % inventory.Count;
                }
                else if(Input.mouseScrollDelta.y < 0f)
                {
                    currItem = (currItem + 1) % inventory.Count;
                }
                inventory[currItem].SetActive(true);
            }
        }

        // Debugging button
        if(Input.GetKey(KeyCode.T))
        {
            for(int i = 0; i < inventory.Count; i++)
            {
                Debug.Log("Item " + i + " is " + inventory[i]);
            }
        }

        // Apply the movement vector to the current position, which is
        // multiplied by deltaTime and speed for a smooth MovePosition
        my_rb.MovePosition(transform.position + userInput * moveSpeed * Time.deltaTime);
    }
}
