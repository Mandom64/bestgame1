using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public enum EnemyState
    {
        Idle,
        Engaged,
        dead,
    };

public class EnemyController : MonoBehaviour
{
    [SerializeField] public float range = 1f;
    [SerializeField] public float idleSpeed = 1f;
    [SerializeField] public float engagedSpeed = 15f;
    private GameObject player;
    [SerializeField] public float timeToFlip = 5f;
    private Rigidbody2D myrb;
    private float timer = 0f;
    
    public EnemyState currentState = EnemyState.Idle;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        myrb = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        
        if(!isPlayerClose())
            currentState = EnemyState.Idle;
        else 
            currentState = EnemyState.Engaged;
        Debug.Log(currentState);

        switch (currentState)
        {
            case(EnemyState.Idle):
                if(timer >= timeToFlip)
                {
                    Vector2 randomDir = RandomVector2(3.1415f*2, 0f);
                    randomDir = randomDir.normalized * idleSpeed * Time.deltaTime;
                    myrb.velocity = randomDir;
                    timer = 0f;
                }
                break;

            case(EnemyState.Engaged):
                var step =  engagedSpeed * Time.deltaTime; // calculate distance to move
                transform.position = Vector3.MoveTowards(transform.position, 
                    player.transform.position, step);
                break;
        }
                
    }

    private bool isPlayerClose()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if(distance >= range)
            return false;
        else 
            return true;
    }

    public Vector2 RandomVector2(float angle, float angleMin)
    {
        float random = UnityEngine.Random.value * angle + angleMin;
        return new Vector2(Mathf.Cos(random), Mathf.Sin(random));
    }
}
