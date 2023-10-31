using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GravityGun : MonoBehaviour
{
    private GameObject player;
    private GameObject objectGrabbed;
    private LineRenderer lineToMouse;
    private float timer = 0f;

    [Header("Prefabs and Layers")]
    public AudioSource grabAudio;
    public AudioSource fireAudio;
    public Transform holdPosition;
    public LayerMask grabbableLayers;

    [Header("Gravity Gun Parameters")]
    [SerializeField] private float grabDistance = 2f;
    [SerializeField] private float shootForce = 50f;
    [SerializeField] private Vector2 rayThickness = new Vector2(0.5f, 0.5f);
    [SerializeField] private float rayDrawTime = 0.1f;
    [SerializeField] private float cooldownTimer = 1f;
    private bool hasGrabbed = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        lineToMouse = GetComponent<LineRenderer>();
        lineToMouse.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        lineToMouse.enabled = false;
    }

    void Update()
    {
        timer += Time.deltaTime;
        Transform parent = this.transform.parent;

        if (parent != null)
        {
            if (parent.CompareTag("Player") && Time.timeScale == 1)
            {
                Utils.EnableAiming(this.gameObject);

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
        Vector3 mousePos = Utils.GetMouseWorldPosition(Input.mousePosition);
        Vector2 rayDir = (mousePos - transform.position).normalized;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position,
            rayThickness, 0.0f, rayDir, grabDistance, grabbableLayers);
        GameObject objectHit;
        foreach (RaycastHit2D hit in hits)
        {
            objectHit = hit.collider.gameObject;
            if (objectHit != null)
            {
                if (objectHit.CompareTag("Walls"))
                {
                    if(!lineToMouse.enabled)
                        StartCoroutine(DrawRay(lineToMouse, mousePos));
                    if (grabAudio != null)
                    {
                        grabAudio.Play();
                    }
                    break;
                }
                else
                {
                    Debug.Log(objectHit.layer);
                    objectGrabbed = objectHit;
                    objectGrabbed.transform.parent = transform;
                    hasGrabbed = true;
                    Debug.Log("gravity gun got " + objectGrabbed.name);
                    if (grabAudio != null)
                    {
                        grabAudio.Play();
                    }
                    break;
                }
            }
        }
    }

    private void Shoot()
    {
        if(objectGrabbed != null)
        {
            Vector3 mousePos = Utils.GetMouseWorldPosition(Input.mousePosition);
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
    private IEnumerator DrawRay(LineRenderer hitRay, Vector3 at)
    {
        hitRay.enabled = true;
        hitRay.SetPosition(0, this.transform.position);
        hitRay.SetPosition(1, at);
        yield return new WaitForSeconds(rayDrawTime);
        hitRay.enabled = false;
    }
}
