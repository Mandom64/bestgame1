using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;

    // Update is called once per frame
    void Update()
    {
        // Close game if escape is pressed 
        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            Application.Quit();
        }
        
        transform.position = player.transform.position + new Vector3(0, 0, -5);
    }
}
