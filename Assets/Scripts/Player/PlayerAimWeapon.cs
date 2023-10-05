using UnityEngine;

public class PlayerAimWeapon : MonoBehaviour
{
    public GameObject weaponPosition;
    public GameObject bulletPrefab;
    public float bulletSpeed;
    public float deathTimer;
    public float cooldownTimer = 0.5f;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        handleAiming();
        if (Input.GetMouseButton(0) && (timer >= cooldownTimer))
        {
            handleShooting();
            timer = 0f;
        }
    }

    private void handleAiming()
    {
        Vector3 mousePos = GetMouseWorldPosition(Input.mousePosition);
        Vector3 aimDirection = (mousePos - weaponPosition.transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        weaponPosition.transform.eulerAngles = new Vector3(0, 0, angle);
        //  Debug.Log(angle);
    }

    private void handleShooting()
    {
        Vector3 mousePos = GetMouseWorldPosition(Input.mousePosition);
        Vector2 shootDirection = (mousePos - weaponPosition.transform.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, weaponPosition.transform.position, Quaternion.identity);
        Rigidbody2D bulletRigidBody = bullet.GetComponent<Rigidbody2D>();
        bulletRigidBody.velocity = shootDirection * bulletSpeed;
        Destroy(bullet, deathTimer);
    }

    public Vector3 GetMouseWorldPosition(Vector3 screenPosition)
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }
}



    