using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GravityGun : MonoBehaviour
{
    private float timer = 0f;
    public AudioSource grabAudio;
    public AudioSource fireAudio;
    public Transform holdPosition;
    public LayerMask grabbableLayer;
    public float grabDistance = 2f;

    [Header("Gravity Gun Parameters")]
    public float cooldownTimer = 1f;
    public float shootForce = 50f;
    private bool hasGrabbed = false;
    private GameObject objectGrabbed;
    LineRenderer lineToMouse;

    void Start()
    {
        grabbableLayer = LayerMask.GetMask("Enemies");
        lineToMouse = GetComponent<LineRenderer>();   
    }

    void Update()
    {
        timer += Time.deltaTime;
        Transform parent = transform.parent;
        lineToMouse.enabled = false;
        if (parent != null)
        {
            if (parent.CompareTag("Player") && Time.timeScale == 1)
            {
                EnableAiming();
                if(Input.GetMouseButtonDown(0) && hasGrabbed)
                {
                    Shoot();
                    Debug.Log("shoot");
                }
                else if (Input.GetMouseButtonDown(0) && (timer >= cooldownTimer) && !hasGrabbed)
                {
                    Grab();
                    timer = 0.0f;
                    Debug.Log("grab");
                }
                if(objectGrabbed != null )
                {
                    objectGrabbed.transform.position = holdPosition.position;
                }
            }
        }
    }

    private void Grab()
    {
        Vector3 mousePos = GetMouseWorldPosition(Input.mousePosition);
        Vector2 rayDir = (mousePos - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position,
            rayDir, grabDistance, grabbableLayer);

        Debug.Log(hit.collider.gameObject.layer);
        if(hit.collider != null) 
        {
            objectGrabbed = hit.collider.gameObject;
            objectGrabbed.transform.parent = transform;
            hasGrabbed = true;
            Debug.Log("gravity gun got " +  objectGrabbed.name);
        }
        else
        {
            DrawLineToMouse();
        }

        if (fireAudio != null)
        {
            grabAudio.Play();
        }
    }

    private void Shoot()
    {
        if(objectGrabbed != null)
        {
            Vector3 mousePos = GetMouseWorldPosition(Input.mousePosition);
            Vector2 shootDir = (mousePos - transform.position).normalized;
            objectGrabbed.isStatic = false;
            objectGrabbed.transform.parent = null;
            objectGrabbed.GetComponent<Rigidbody2D>().AddForce(shootDir * shootForce, ForceMode2D.Impulse);
            objectGrabbed = null;
            hasGrabbed = false;

            if (fireAudio != null)
            {
                fireAudio.Play();
            }
        }
        else
        {
            hasGrabbed= false;
        }
    }
    public void DrawLineToMouse()
    {
        Vector3 mousePos = GetMouseWorldPosition(Input.mousePosition);
        Vector2 shootDir = (mousePos - transform.position).normalized;
        lineToMouse.enabled = true;
        lineToMouse.material = new Material(Shader.Find("Sprites/Default"));
        lineToMouse.material.color = Color.red;
        lineToMouse.SetPosition(0, transform.position);
        lineToMouse.SetPosition(1, shootDir);
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
}
