using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

enum BossState
{
    Idle,
    Engaged,
    Struck,
    Dead,
};

enum AttackType
{
    None,
    CloseRange,
    MediumRange,
    LongRange,
};

public class BossController : MonoBehaviour
{
    private BossState currentState = BossState.Idle;
    private AttackType attackState = AttackType.None;
    private Rigidbody2D mBody;
    private Animator mAnimator;
    private GameObject player;
    private float timer, timer2;
    private float lastHP;
    [Header("Boss Parameters")]
    [SerializeField] public float overallRange = 20f;
    [SerializeField] public float closeRange = 2.5f;
    [SerializeField] public float mediumRange = 12.5f;
    [SerializeField] public float longRange = 15f;
    [SerializeField] public float timeToMove = 1f;
    [SerializeField] public float engagedSpeed = 25f;
    [SerializeField] public bool canShoot = true;
    [Header("Close Range Attack")]
    [SerializeField] public AudioSource dashSound;
    [SerializeField] public float dashSpeed = 2000f;
    [SerializeField] public float dashTime = 0.5f;
    [SerializeField] public float dashCooldown = 1f;
    [SerializeField] public float pounceTime = 0.25f;
    private bool isDashing = false;
    [Header("Medium Range Bullet")]
    [SerializeField] public GameObject bullet;
    [SerializeField] public float bulletSpeed = 50f;
    [SerializeField] public float bulletDeathTimer = 3f;
    [SerializeField] public float bulletCooldown = 3f;
    [SerializeField] public int bulletColumns = 6;
    [SerializeField] public int bulletRows = 6;
    [SerializeField] public float columnDistance = 0.5f;
    [SerializeField] public float bulletSpread = 2.5f;
    [Header("Long Range Lance")]
    [SerializeField] public GameObject lance;
    [SerializeField] public float lanceSpeed = 50f;
    [SerializeField] public float lanceDeathTimer = 3f;
    [SerializeField] public float lanceCooldown = 3f;

    void Start()
    {
        mBody = GetComponent<Rigidbody2D>();
        mAnimator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
        timer = 0f;
    }

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        timer2 += Time.fixedDeltaTime;
        
        if (this.gameObject == null)
            return;
        if(!isDashing)
            Move();
        HandleBehavior();
        FlipSprites();
    }

    void Update()
    {
        if (this.gameObject == null)
            return;
        Debug.Log(attackState);
    }

    private void Move()
    {
        switch (currentState)
        {
            case BossState.Idle:
                mBody.velocity = Vector2.zero;
                break;
            case BossState.Engaged:
                if (timer >= timeToMove && !isDashing)
                {
                    Vector3 playerDir = (player.transform.position - transform.position).normalized;
                    Vector3 randomDir = RandomVector2(3.1415f * 2, 0f).normalized;
                    Vector3 moveDir = playerDir + randomDir;
                    moveDir.Normalize();
                    mBody.velocity = moveDir * engagedSpeed * Time.fixedDeltaTime;
                    timer = 0f;
                }
                break;
            case BossState.Struck:
                mBody.velocity = Vector2.zero;
                break;
            case BossState.Dead:
                mBody.velocity = Vector2.zero;
                this.gameObject.layer = LayerMask.NameToLayer("Dead Objects");
                break;
        }
    }

    private void HandleBehavior()
    {
        float playerDist = PlayerDistance();
        if (playerDist <= overallRange)
        {
            currentState = BossState.Engaged;
            if (playerDist > 0 && playerDist <= closeRange)
                attackState = AttackType.CloseRange;
            else if (playerDist > closeRange && playerDist <= mediumRange)
                attackState = AttackType.MediumRange;
            else if (playerDist > mediumRange && playerDist <= longRange)
                attackState = AttackType.LongRange;
        }
        else
        {
            currentState = BossState.Idle;
            attackState = AttackType.None;
        }

        float mHP = mBody.GetComponent<HealthSystem>().getHealth();
        if (mHP != lastHP)
            currentState = BossState.Struck;
        lastHP = mHP;
        if (mHP <= 0)
            currentState = BossState.Dead;

        if (currentState == BossState.Engaged)
        {
            switch (attackState)
            {
                case AttackType.CloseRange:
                    CloseCharge();

                    break;
                case AttackType.MediumRange:
                    if (canShoot)
                    {
                        MediumShoot(player);
                    }
                    break;
                case AttackType.LongRange:
                    if (canShoot)
                    {
                        LongShoot(player);
                    }
                    break;
            }
        }

    }

    private void CloseCharge()
    {
        if (!isDashing && timer2 >= dashCooldown)
        {
            StartCoroutine(Dash());
            timer2 = 0f;
        }
    }

    private void MediumShoot(GameObject objToShoot)
    {
        if (objToShoot != null)
        {
            StartCoroutine(ShootCooldown(bulletCooldown));
            float startAngle = -(bulletColumns / 2 * bulletSpread);
            float angle;
            for(int i = 0; i < bulletColumns; i++)
            {
                angle = startAngle;
                StartCoroutine(ShootCooldown(columnDistance));
                for(int j = 0; j < bulletRows; j++)
                {
                    Debug.Log(startAngle);
                    Vector2 shootDir = (objToShoot.transform.position - transform.position).normalized;
                    shootDir = RotateVector2(shootDir, angle);
                    angle += bulletSpread;
                    GameObject bulletInstance = Instantiate(bullet, transform.position, transform.rotation);
                    Rigidbody2D bulletBody = bulletInstance.GetComponent<Rigidbody2D>();
                    bulletInstance.layer = LayerMask.NameToLayer("Enemy Projectiles");
                    bulletBody.AddForce(shootDir * bulletSpeed);
                    Destroy(bulletInstance, bulletDeathTimer);
                }
            }
        }
    }

    private void LongShoot(GameObject objToShoot)
    {
        if (objToShoot != null)
        {
            StartCoroutine(ShootCooldown(lanceCooldown));
            Vector2 shootDir = (objToShoot.transform.position - transform.position).normalized;
            GameObject lanceInstance = Instantiate(lance, transform.position, transform.rotation);
            Rigidbody2D lanceBody = lanceInstance.GetComponent<Rigidbody2D>();
            lanceInstance.layer = LayerMask.NameToLayer("Enemy Projectiles");
            lanceBody.AddForce(shootDir * lanceSpeed);
            Destroy(lanceInstance, lanceDeathTimer);
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        Vector2 attackDir = (player.transform.position - transform.position).normalized;
        mBody.AddForce(attackDir * dashSpeed);
        yield return new WaitForSeconds(dashTime);

        if (dashSound != null)
            dashSound.Play();
        isDashing = false;
    }

    public IEnumerator ShootCooldown(float cooldownTime)
    {
        canShoot = false;
        yield return new WaitForSeconds(cooldownTime);
        canShoot = true;
    }

    public Vector2 RandomVector2(float angle, float angleMin)
    {
        float random = Random.value * angle + angleMin;
        return new Vector2(Mathf.Cos(random), Mathf.Sin(random));
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

    public void FlipSprites() { this.gameObject.GetComponent<SpriteRenderer>().flipX = (player.transform.position.x > this.transform.position.x);}
    
    public float PlayerDistance() { return Vector3.Distance(this.transform.position, player.transform.position);}

    private void OnDrawGizmos()
    {
        // Draw a line between the two positions
        Gizmos.color = Color.gray;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(overallRange, 0, 0));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(longRange, 0, 0));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(mediumRange, 0, 0));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(closeRange, 0, 0));

    }
}
