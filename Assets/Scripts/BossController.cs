using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

public class BossController : MonoBehaviour
{
    private enum State
    {
        Idle,
        Engaged,
        Struck,
        Dead,
    };
    private enum Range
    {
        None,
        CloseRange,
        MediumRange,
        LongRange,
    };

    private State currentState = State.Idle;
    private Range attackState = Range.None;
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
        Utils.FlipSprites(this.gameObject, player);
    }

    void Update()
    {
        if (this.gameObject == null)
            return;
    }

    private void Move()
    {
        switch (currentState)
        {
            case State.Idle:
                mBody.velocity = Vector2.zero;
                break;
            case State.Engaged:
                if (timer >= timeToMove && !isDashing)
                {
                    Vector3 playerDir = (player.transform.position - transform.position).normalized;
                    Vector3 randomDir = Utils.RandomVector2(3.1415f * 2, 0f).normalized;
                    Vector3 moveDir = playerDir + randomDir;
                    moveDir.Normalize();
                    mBody.velocity = moveDir * engagedSpeed * Time.fixedDeltaTime;
                    timer = 0f;
                }
                break;
            case State.Struck:
                mBody.velocity = Vector2.zero;
                break;
            case State.Dead:
                mBody.velocity = Vector2.zero;
                this.gameObject.layer = LayerMask.NameToLayer("Dead Objects");
                break;
        }
    }

    private void HandleBehavior()
    {
        float playerDist = Utils.PlayerDistance(this.gameObject, player);
        if (playerDist <= overallRange)
        {
            currentState = State.Engaged;
            if (playerDist > 0 && playerDist <= closeRange)
                attackState = Range.CloseRange;
            else if (playerDist > closeRange && playerDist <= mediumRange)
                attackState = Range.MediumRange;
            else if (playerDist > mediumRange && playerDist <= longRange)
                attackState = Range.LongRange;
        }
        else
        {
            currentState = State.Idle;
            attackState = Range.None;
        }

        float mHP = mBody.GetComponent<HealthSystem>().getHealth();
        if (mHP != lastHP)
            currentState = State.Struck;
        lastHP = mHP;
        if (mHP <= 0)
            currentState = State.Dead;

        if (currentState == State.Engaged)
        {
            switch (attackState)
            {
                case Range.CloseRange:
                    CloseCharge();

                    break;
                case Range.MediumRange:
                    if (canShoot)
                    {
                        MediumShoot(player);
                    }
                    break;
                case Range.LongRange:
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
                    //Debug.Log(startAngle);
                    Vector2 shootDir = (objToShoot.transform.position - transform.position).normalized;
                    shootDir = Utils.RotateVector2(shootDir, angle);
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
