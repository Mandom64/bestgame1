using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    private float currentFrameRate;
    public float updateInterval = 0.5f;
    private float timer = 0f;
    public TextMeshProUGUI fpsCounter;
    public TextMeshProUGUI healthBar;
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= updateInterval)
        {
            currentFrameRate = 1.0f / Time.deltaTime;
            fpsCounter.text = currentFrameRate.ToString("0.00");
            timer = 0f;
        }
        GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
        if(playerGameObject != null)
        {
            HealthSystem playerHealth = playerGameObject.GetComponent<HealthSystem>();
            if(playerHealth != null)
            {
                healthBar.text = playerHealth.getHealth().ToString();
            }
        }
    }
}
