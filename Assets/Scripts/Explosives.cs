using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Explosives : MonoBehaviour
{
    private Rigidbody2D mBody;
    private Animator mAnimator;
    private HealthSystem mHealth;
    private bool hasExploded = false;

    [Header("Explosive Parameters")]
    [SerializeField] private float damage = 50f;
    [SerializeField] private float explosionForce = 100f;
    [SerializeField] private float splashRange = 1f;
    [SerializeField] private LayerMask damageLayers;
    public AudioSource explosionSound;

    private void Start()
    {
        mHealth = GetComponent<HealthSystem>();
        mAnimator = GetComponent<Animator>();
        mBody = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(splashRange > 0 && mHealth.getHealth() <= 50)
        {
            Explode();
        }
    }

    public void Explode()
    {
        if (!hasExploded)
        {
            var hits = Physics2D.OverlapCircleAll(this.transform.position, splashRange, damageLayers);
            foreach (var hit in hits)
            {
                GameObject objectHit = null;
                if (hit.gameObject != null)
                    objectHit = hit.gameObject;
                if (objectHit != null && objectHit != this.gameObject)
                {
                    Rigidbody2D objectHitBody = objectHit.GetComponent<Rigidbody2D>();
                    if(objectHitBody != null)
                    {
                        var forceDir = (objectHit.transform.position - this.transform.position).normalized;
                        objectHitBody.AddForce(explosionForce * forceDir, ForceMode2D.Impulse);
                    }
                    var closestPoint = hit.ClosestPoint(this.transform.position);
                    var distance = Vector3.Distance(closestPoint, this.transform.position);
                    var damagePercent = Mathf.InverseLerp(splashRange, 0, distance);
                    HealthSystem enemyHP = objectHit.GetComponent<HealthSystem>();
                    if (enemyHP != null)
                    {
                        enemyHP.Damage(damagePercent * damage);
                        EnemyHealthbar enemyHealthbar = objectHit.GetComponentInChildren<EnemyHealthbar>();
                        if (enemyHealthbar != null)
                            enemyHealthbar.UpdateHealthbarUI(enemyHP.getHealth(), enemyHP.getHealthMax());
                        Debug.Log(enemyHP.name + " took " + damagePercent * damage + " from a barrel!!!");
                    }
                }
            }
        
            hasExploded = true;
            mAnimator.SetBool("explode", true);
            mBody.velocity = Vector2.zero;
            if (explosionSound != null)
                explosionSound.Play();
            Destroy(this.gameObject, 1f);
        }
    }
}
