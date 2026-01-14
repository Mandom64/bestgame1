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
    private GameObject player;
    public TextMeshProUGUI fpsCounter;
    public GameObject inventoryObject;
    public Image InventoryImage;
    public Canvas options;
    public Slider audioSlider;
    public Image healthbar;
    public TextMeshProUGUI ammo;
    bool pauseGame = false;

    void Start()
    {
        InventoryImage.enabled = false;
        player = GameObject.FindGameObjectWithTag("Player");
        options.gameObject.SetActive(false);
        audioSlider.value = 0.5f;
        AudioListener.volume = audioSlider.value;
    }

    // Update is called once per frame
    void Update()
    {
        if(pauseGame)
        {
            AudioListener.volume = audioSlider.value;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                pauseGame = false;
                Time.timeScale = 1;
                options.gameObject.SetActive(false);
            }
        }
        else
        {
            updateFPScounterUI();
            updateHealthBarUI();  
            updateInventoryImgUI(); 
            if(Input.GetKeyDown(KeyCode.Escape)) 
            {
                pauseGame = true;
                Time.timeScale = 0;
                options.gameObject.SetActive(true);
            }

        }
    }

    public void updateFPScounterUI()
    {
        // update FPS counter 
        timer += Time.deltaTime;
        if(timer >= updateInterval)
        {
            currentFrameRate = 1.0f / Time.deltaTime;
            fpsCounter.text = "FPS: " + currentFrameRate.ToString("0.00");
            timer = 0f;
        }
    }
    public void updateHealthBarUI()
    {
        // Healthbar slider and text update
        if(player != null)
        {
            HealthSystem playerHealth = player.GetComponent<HealthSystem>();
            if(playerHealth != null)
            {
                float healthPercentage = (float)playerHealth.getHealth() / (float)playerHealth.getHealthMax();
                healthbar.fillAmount = healthPercentage;
            }
        }
    }

    public void updateInventoryImgUI()
    {
        Inventory inventory = inventoryObject.GetComponent<Inventory>();
        if(inventory != null && inventory.inventoryList.Count > 0)
        {
            InventoryImage.enabled = true;
            int itemToShow = player.GetComponent<PlayerController>().currItem;
            SpriteRenderer currItemImage = 
                inventory.inventoryList[itemToShow].GetComponent<SpriteRenderer>();
            InventoryImage.sprite = currItemImage.sprite;
            if(inventory.inventoryList[itemToShow].name != "GravityGun")
            {
                ammo.enabled = true;
                ammo.text = inventory.inventoryList[itemToShow].GetComponent<Ammo>().getAmmo().ToString();
            }
        }
        else if (inventory.inventoryList.Count == 0)
        {
            InventoryImage.enabled = false;
            ammo.enabled = false;
        }
    }

    public void OnQuitButton()
    {
       Application.Quit();
    }
}
