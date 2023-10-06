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
    public TextMeshProUGUI healthBarText;
    public Slider healthBar;

    // Update is called once per frame
    void Update()
    {
        // update FPS counter 
        timer += Time.deltaTime;
        if(timer >= updateInterval)
        {
            currentFrameRate = 1.0f / Time.deltaTime;
            fpsCounter.text = currentFrameRate.ToString("0.00");
            timer = 0f;
        }
        
        updateHealthBarUI();   
    }

    public void updateHealthBarUI()
    {
        // Healthbar slider and text update
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if(playerObj != null)
        {
            HealthSystem playerHealth = playerObj.GetComponent<HealthSystem>();
            if(playerHealth != null)
            {
                healthBarText.text = playerHealth.getHealth().ToString();
            }
            float healthPercentage = (float)playerHealth.getHealth() / (float)playerHealth.getHealthMax();
            healthBar.value = healthPercentage;          
        }
    }
}
