using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Shotgun : MonoBehaviour
{
    private Ammo mAmmo;
    private float timer = 0.0f;
    private bool canShoot = true;

    [Header("Shotgun parameters")]
    [SerializeField] public GameObject pellet;
    [SerializeField] public int pelletCount = 5;
    [SerializeField] public float pelletSpeed = 1.0f;
    [SerializeField] public float pelletRandSpeed = 1.0f;
    [SerializeField] public float pelletDeathTimer = 3.0f;
    [SerializeField] public float spread = 5.0f;
    [SerializeField] public float cooldownTimer = 0.25f;
    [SerializeField] public AudioSource fireAudio;

    void Start()
    {
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
            Vector3 mousePos = Utils.GetMouseWorldPosition(Input.mousePosition);
            Vector2 shootDir = (mousePos - transform.position).normalized;

            for (int i = 0; i < pelletCount; i++)
            {
                if(mAmmo.getAmmo() > 0)
                {
                    // Create random spread by alternating positive and negative angles
                    int randValue = Random.Range(0, 2);
                    if (randValue == 1)
                        shootDir = Utils.RotateVector2(shootDir, spread);
                    else
                        shootDir = Utils.RotateVector2(shootDir, -spread);
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
        Vector3 mousePos = Utils.GetMouseWorldPosition(Input.mousePosition);
        Vector3 aimDir = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        if (angle > -90f && angle < 90f)
            gameObject.GetComponent<SpriteRenderer>().flipY = false;
        else
            gameObject.GetComponent<SpriteRenderer>().flipY = true;
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    public float RandomPelletSpeed()
    {
        int randValue = Random.Range(0, 2);

        if (randValue == 1)
            return Random.Range(pelletSpeed, pelletSpeed + pelletRandSpeed);
        else
            return Random.Range(pelletSpeed, pelletSpeed - pelletRandSpeed);
    }

}
