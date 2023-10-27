using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Shotgun : MonoBehaviour
{
    Rigidbody2D body;
    private AudioSource fireAudio;
    private Ammo mAmmo;
    [Header("Shotgun parameters")]
    public GameObject pellet;
    public int pelletCount = 5;
    public float pelletSpeed = 1.0f;
    public float pelletRandSpeed = 1.0f;
    public float pelletDeathTimer = 3.0f;
    public float spread = 5.0f;
    public float cooldownTimer = 0.25f;
    private float timer = 0.0f;
    private bool canShoot = true;


    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        fireAudio = gameObject.GetComponent<AudioSource>();
        mAmmo = GetComponent<Ammo>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        Transform parent = transform.parent;
        if(parent != null)
        {
            if(parent.CompareTag("Player") && Time.timeScale == 1)
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
        canShoot = mAmmo.CanShoot();
        if(canShoot)
        {
            Vector3 mousePos = GetMouseWorldPosition(Input.mousePosition);
            Vector2 shootDir = (mousePos - transform.position).normalized;

            for (int i = 0; i < pelletCount; i++)
            {
                if(mAmmo.getAmmo() > 0)
                {
                    // Create random spread by alternating positive and negative angles
                    int randValue = Random.Range(0, 2);
                    if (randValue == 1)
                        shootDir = RotateVector2(shootDir, spread);
                    else
                        shootDir = RotateVector2(shootDir, -spread);
                    GameObject pelletInstance = Instantiate(pellet, transform.position, transform.rotation);
                    Rigidbody2D rb_pellet = pelletInstance.GetComponent<Rigidbody2D>();
                    rb_pellet.AddForce(shootDir * RandomPelletSpeed());
                    Destroy(pelletInstance, pelletDeathTimer);
                    mAmmo.UseAmmo(1);
                }
                else
                {
                    canShoot = false;
                }
            }

            if (fireAudio != null)
            {
                fireAudio.Play();
            }
        }
    }

    private void EnableAiming()
    {
        Vector3 mousePos = GetMouseWorldPosition(Input.mousePosition);
        Vector3 aimDir = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        if (angle > -90f && angle < 90f)
            gameObject.GetComponent<SpriteRenderer>().flipY = false;
        else
            gameObject.GetComponent<SpriteRenderer>().flipY = true;
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

    public float RandomPelletSpeed()
    {
        int randValue = Random.Range(0, 2);

        if (randValue == 1)
            return Random.Range(pelletSpeed, pelletSpeed + pelletRandSpeed);
        else
            return Random.Range(pelletSpeed, pelletSpeed - pelletRandSpeed);

    }

    public void FlipSpriteY(GameObject obj)
    {
        Vector3 mousePos = GetMouseWorldPosition(Input.mousePosition);
        Vector3 aimDir = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        if (angle > -90f && angle < 90f)
            obj.GetComponent<SpriteRenderer>().flipY = false;
        else
            obj.GetComponent<SpriteRenderer>().flipY = true;
    }


}
