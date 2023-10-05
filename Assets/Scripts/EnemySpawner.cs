using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private float timer = 0f;
    private List<GameObject> enemies = new List<GameObject>();
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
