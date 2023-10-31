using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Parameters")]
    [SerializeField] private GameObject enemy;
    [SerializeField] private int spawnLimit = 5;
    [SerializeField] private float timeToSpawn = 5f;
    private List<GameObject> enemies;
    private float timer = 0f;

    void Start()
    {
        enemies = new List<GameObject>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if((timer >= timeToSpawn) && (enemies.Count < spawnLimit))
        {
            enemies.Add(Instantiate(enemy, transform.position, Quaternion.identity));
            timer = 0f;
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            if(enemies[i] == null) 
            {
                enemies.RemoveAt(i);
            }
        }
        //Debug.Log(enemies.Count);
    }

}
