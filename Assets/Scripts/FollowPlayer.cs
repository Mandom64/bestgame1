using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public Camera mainCamera;
    public float cameraMaxDistance = 2.5f;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            Application.Quit();
        }

        Vector3 mousePos = GetMouseWorldPosition(Input.mousePosition);
        Vector2 distance = mousePos - mainCamera.transform.position;
        distance = Vector3.ClampMagnitude(distance, cameraMaxDistance);
        mainCamera.transform.position = player.transform.position + (Vector3)distance + new Vector3(0, 0, -5);
    }

    public Vector3 GetMouseWorldPosition(Vector3 screenPos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        return worldPos;
    }
}
