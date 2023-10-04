using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private float timer = 0f;
    private int spawnCounter = 0;
    [SerializeField]public int spawnLimit = 5;
    [SerializeField]public float timeToSpawn = 5f;
    [SerializeField]public GameObject enemy;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if((timer >= timeToSpawn) && (spawnCounter <= spawnLimit))
        {
            Instantiate(enemy, transform.position, Quaternion.identity);
            spawnCounter++;
            timer = 0f;
        }
    }
}
