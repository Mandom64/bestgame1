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
    private GameObject playerObject;
    public TextMeshProUGUI fpsCounter;
    public TextMeshProUGUI healthBarText;
    public Slider healthBar;
    public GameObject inventoryObject;
    public Image InventoryImage;

    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        updateFPScounterUI();
        updateHealthBarUI();  
        updateInventoryImgUI(); 
    }

    public void updateFPScounterUI()
    {
        // update FPS counter 
        timer += Time.deltaTime;
        if(timer >= updateInterval)
        {
            currentFrameRate = 1.0f / Time.deltaTime;
            fpsCounter.text = currentFrameRate.ToString("0.00");
            timer = 0f;
        }
    }
    public void updateHealthBarUI()
    {
        // Healthbar slider and text update
        if(playerObject != null)
        {
            HealthSystem playerHealth = playerObject.GetComponent<HealthSystem>();
            if(playerHealth != null)
            {
                healthBarText.text = playerHealth.getHealth().ToString();
            }
            float healthPercentage = (float)playerHealth.getHealth() / (float)playerHealth.getHealthMax();
            healthBar.value = healthPercentage;          
        }
    }

    public void updateInventoryImgUI()
    {
        Inventory inventory = inventoryObject.GetComponent<Inventory>();
        if(inventory != null && inventory.inventoryList.Count > 0)
        {
            SpriteRenderer currItemImage = 
                inventory.inventoryList[inventory.currItem].GetComponent<SpriteRenderer>();
            InventoryImage.sprite = currItemImage.sprite;
        }
    }
}
