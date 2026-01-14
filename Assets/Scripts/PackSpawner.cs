using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackSpawner : MonoBehaviour
{
    private bool spawned = false;

    [SerializeField] private GameObject ammoPellet;
    [SerializeField] private int minAmmoPellets, maxAmmoPellets;

    [SerializeField] private GameObject healthPellet;
    [SerializeField] private int minHealthPellets, maxHealthPellets;

    private void OnDestroy()
    {
        Spawn();
    }

    public void Spawn()
    {
        if (!spawned)
        {
            Debug.Log("hello spawn here");
            int numAmmoPellets = Random.Range(minAmmoPellets, maxAmmoPellets);
            int numHealthPellets = Random.Range(minHealthPellets, maxHealthPellets);
            for (int i = 0; i < numAmmoPellets; i++)
            {
                GameObject ammoPelletInstance = Instantiate(ammoPellet, this.transform.position, Quaternion.identity);
            }
            for (int i = 0; i < numHealthPellets; i++)
            {
                GameObject healthPelletInstance = Instantiate(healthPellet, this.transform.position, Quaternion.identity);
            }
            spawned = true;
        }
    }
}
