using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float timer;
    [Header("Boss Parameters")]
    [SerializeField] public float overallRange = 20f;
    [SerializeField] public float closeRange = 2.5f;
    [SerializeField] public float mediumRange = 12.5f;
    [SerializeField] public float longRange = 15f;
    [SerializeField] public float timeToMove = 1f;
    [SerializeField] public float engagedSpeed = 25f;
    [SerializeField] public bool canShoot = true;
    [Header("Medium Range Bullet")]
    [SerializeField] public GameObject bullet;
    [SerializeField] public float bulletSpeed = 50f;
    [SerializeField] public float bulletDeathTimer = 3f;
    [SerializeField] public float bulletCooldown = 3f;
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
        timer += Time.deltaTime;
        if (this.gameObject == null)
            return;
        Move();
        HandleBehavior();
        FlipSprites();
        HandleBehavior();
    }

    void Update()
    {
        if (this.gameObject == null)
            return;
        
        //Debug.Log(currentState);
        //Debug.Log(attackState);
    }
    private void Move()
    {
        switch (currentState)
        {
            case BossState.Idle:
                mBody.velocity = Vector2.zero;
                break;
            case BossState.Engaged:
                if (timer >= timeToMove)
                {
                    Vector3 playerDir = (player.transform.position - transform.position).normalized;
                    Vector3 randomDir = RandomVector2(3.1415f * 2, 0f).normalized;
                    Vector3 moveDir = playerDir + randomDir;
                    moveDir.Normalize();
                    mBody.velocity = moveDir * engagedSpeed * Time.fixedDeltaTime;
                    timer = 0f;
                }
                break;
            case BossState.Dead:
                mBody.velocity = Vector2.zero;
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

        if(currentState == BossState.Engaged)
        {
            switch (attackState)
            {
                case AttackType.CloseRange:
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
    public IEnumerator Cooldown(float cooldownTime)
    {
        canShoot = false;
        yield return new WaitForSeconds(cooldownTime);
        canShoot = true;
    }
    public Vector2 RandomVector2(float angle, float angleMin)
    {
        float random = UnityEngine.Random.value * angle + angleMin;
        return new Vector2(Mathf.Cos(random), Mathf.Sin(random));
    }
    private void MediumShoot(GameObject objToShoot)
    {
        if (objToShoot != null)
        {
            StartCoroutine(Cooldown(bulletCooldown));
            Vector2 shootDir = (objToShoot.transform.position - transform.position).normalized;
            GameObject bulletInstance = Instantiate(bullet, transform.position, transform.rotation);
            Rigidbody2D bulletBody = bulletInstance.GetComponent<Rigidbody2D>();
            bulletInstance.layer = LayerMask.NameToLayer("Enemy Projectiles");
            bulletBody.AddForce(shootDir * bulletSpeed);
            Destroy(bulletInstance, bulletDeathTimer);
        }
    }
    private void LongShoot(GameObject objToShoot)
    {
        if (objToShoot != null)
        {
            StartCoroutine(Cooldown(lanceCooldown));
            Vector2 shootDir = (objToShoot.transform.position - transform.position).normalized;
            GameObject lanceInstance = Instantiate(lance, transform.position, transform.rotation);
            Rigidbody2D lanceBody = lanceInstance.GetComponent<Rigidbody2D>();
            lanceInstance.layer = LayerMask.NameToLayer("Enemy Projectiles");
            lanceBody.AddForce(shootDir * lanceSpeed);
            Destroy(lanceInstance, lanceDeathTimer);
        }
    }

    public void FlipSprites() { this.gameObject.GetComponent<SpriteRenderer>().flipX = (player.transform.position.x > this.transform.position.x);}
    public float PlayerDistance() { return Vector3.Distance(this.transform.position, player.transform.position);}
}
