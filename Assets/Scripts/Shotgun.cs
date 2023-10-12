using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    public Rigidbody2D my_rb;
    private AudioSource fireAudio;
    [Header("Shotgun parameters")]
    public GameObject pellet;
    public int pelletCount = 5;
    public float pelletSpeed = 1.0f;
    public float pelletRandSpeed = 1.0f;
    public float pelletDeathTimer = 3.0f;
    public float spread = 5.0f;
    public float cooldownTimer = 0.25f;
    private float timer = 0.0f;


    void Start()
    {
        my_rb = GetComponent<Rigidbody2D>();
        fireAudio = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        Transform parent = transform.parent;
        if(parent != null)
        {
            if(parent.CompareTag("Player"))
            {
                EnableAiming();
                if(Input.GetMouseButton(0) && (timer >= cooldownTimer))
                {
                    Shoot(pelletCount);
                    timer = 0.0f;
                }
            }
        }
    }

    private void Shoot(int pelletCount)
    {
        Vector3 mousePos = GetMouseWorldPosition(Input.mousePosition);
        Vector2 shootDir = (mousePos - transform.position).normalized;

        for (int i = 0; i < pelletCount; i++)
        {
            // Create random spread by alternating positive and negative angles
            int randValue = Random.Range(0, 2);
            if (randValue == 1)
                shootDir = RotateVector2(shootDir, spread);
            else
                shootDir = RotateVector2(shootDir, -spread);
            GameObject pelletInstance = Instantiate(pellet, transform.position, Quaternion.identity);
            Rigidbody2D rb_pellet = pelletInstance.GetComponent<Rigidbody2D>();
            rb_pellet.AddForce(shootDir * randomPelletSpeed());
            Destroy(pelletInstance, pelletDeathTimer);
        }
        
        if(fireAudio != null)
        {
            fireAudio.Play();
        }
    }
    private void EnableAiming()
    {
        Vector3 mousePos = GetMouseWorldPosition(Input.mousePosition);
        Vector3 aimDir = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    public Vector3 GetMouseWorldPosition(Vector3 screenPos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        return worldPos;
    }

    public Vector2 RotateVector2(Vector2 vector, float angleDegrees)
    {
        // Convert the angle to radians because Mathf.Cos and Mathf.Sin use radians
        float angleRadians = angleDegrees * Mathf.Deg2Rad;

        // Calculate the new rotated vector
        float x = vector.x * Mathf.Cos(angleRadians) - vector.y * Mathf.Sin(angleRadians);
        float y = vector.x * Mathf.Sin(angleRadians) + vector.y * Mathf.Cos(angleRadians);

        return new Vector2(x, y);
    }
    public float randomPelletSpeed()
    {
        int randValue = Random.Range(0, 2);

        if (randValue == 1)
            return Random.Range(pelletSpeed, pelletSpeed + pelletRandSpeed);
        else
            return Random.Range(pelletSpeed, pelletSpeed - pelletRandSpeed);

    }

}
