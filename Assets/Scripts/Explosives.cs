using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Explosives : MonoBehaviour
{
    public float damage = 50f;
    public float splashRange = 1f;
    private HealthSystem mHealthSystem;
    private Rigidbody2D body;
    public Animator mAnimator;
    public List<LayerMask> damageLayers = new List<LayerMask>();
    private bool hasExploded = false;

    private void Start()
    {
        mHealthSystem = GetComponent<HealthSystem>();
        mAnimator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        damageLayers.Add(LayerMask.NameToLayer("Enemies"));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Barrel health is " + mHealthSystem.getHealth());
        if(splashRange > 0 && mHealthSystem.getHealth() <= 50 && !hasExploded)
        {
            Explode();
        }
        
    }

    public void Explode()
    {
        foreach (LayerMask mask in damageLayers)
        {
            var hitColliders = Physics2D.OverlapCircleAll(this.transform.position, splashRange, mask);
            foreach (var hitCollider in hitColliders)
            {
                var closestPoint = hitCollider.ClosestPoint(this.transform.position);
                var distance = Vector3.Distance(closestPoint, this.transform.position);
                var damagePercent = Mathf.InverseLerp(splashRange, 0, distance);
                HealthSystem enemyHP = hitCollider.gameObject.GetComponent<HealthSystem>();
                if (enemyHP != null)
                {
                    EnemyHealthbar enemyHealthbar = hitCollider.gameObject.GetComponentInChildren<EnemyHealthbar>();
                    if (enemyHealthbar != null)
                        enemyHealthbar.UpdateHealthbarUI(enemyHP.getHealth(), enemyHP.getHealthMax());
                    enemyHP.Damage(damagePercent * damage);
                    Debug.Log(enemyHP.name + " took " + damagePercent * damage);
                }
            }
        }
        hasExploded = true;
        mAnimator.SetBool("explode", true);
        body.velocity = Vector2.zero;
        Destroy(this.gameObject, 1f);
    }
}
