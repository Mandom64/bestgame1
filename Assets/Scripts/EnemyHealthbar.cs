using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbar : MonoBehaviour
{
    public Slider healthbar;

    void Start()
    {
        healthbar = GetComponent<Slider>();
    }

    public void UpdateHealthbarUI(float currHP, float maxHP)
    {
        if(currHP <= 0f)
            healthbar.gameObject.SetActive(false);
        else
            healthbar.value = currHP / maxHP;
    }
}
