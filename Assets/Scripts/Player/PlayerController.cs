using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Rigidbody2D player;
    public float runningSpeed = 20.0f;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Player movement
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 tempVect = new Vector3(h, v, 0);
        tempVect = tempVect.normalized * runningSpeed * Time.deltaTime;
        player.MovePosition(player.transform.position + tempVect);

        // Shoot bullet
        Vector2 playerPos = player.transform.position;


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision involves the "Objects" layer.
        if (collision.gameObject.tag == "Walls")
        {
            Debug.Log("Player collided with an object!");
        }
    }
}
