using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D my_rb;
    public float moveSpeed = 5.0f;
    //public LayerMask grabbableLayer;

    void Start()
    {
        my_rb = GetComponent<Rigidbody2D>();
        //grabbableLayer = LayerMask.GetMask("GrabbableItems");
    }

    void Update()
    {
        // Close game if escape is pressed 
        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            Application.Quit();
        }
        
        // NOT WORKING
        // if(Input.GetKey(KeyCode.E))
        // {
        //     RaycastHit2D hit = Physics2D.Raycast(transform.position, 
        //         transform.right, 10f, grabbableLayer);

        //     if(hit.collider != null)
        //     {
        //         Debug.Log(hit.collider.gameObject + " is hit!!");
        //         GameObject grabbedObject = hit.collider.gameObject;
        //         grabbedObject.transform.position = transform.position;
        //         grabbedObject.transform.SetParent(transform);
        //     }
        // }
        // Store user input as a movement vector
        Vector3 userInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);

        // Apply the movement vector to the current position, which is
        // multiplied by deltaTime and speed for a smooth MovePosition
        my_rb.MovePosition(transform.position + userInput * moveSpeed * Time.deltaTime);
    }


}
