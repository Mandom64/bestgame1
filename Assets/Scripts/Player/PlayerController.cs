using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D my_rb;
    public float moveSpeed = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        my_rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Close game if escape is pressed 
        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            Application.Quit();
        }
        
        // Store user input as a movement vector
        Vector3 userInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);

        // Apply the movement vector to the current position, which is
        // multiplied by deltaTime and speed for a smooth MovePosition
        my_rb.MovePosition(transform.position + userInput * moveSpeed * Time.deltaTime);
    }

}
