using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class pellet : MonoBehaviour
{
    public Rigidbody2D body;
    public float bounceForce = 1f;
    private Vector2 lastVelocity;

    public void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        lastVelocity = body.velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Walls"))
        {
            body.AddForce(-lastVelocity * bounceForce);
        }
    }
}
