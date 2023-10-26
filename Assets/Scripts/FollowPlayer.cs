using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    public Camera mainCamera;
    public float cameraMaxDistance = 5f;
    public float cameraMinDistance = 1f;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if(Time.timeScale == 1)
        {
            Vector3 mousePos = GetMouseWorldPosition(Input.mousePosition);
            Vector2 distance = mousePos - mainCamera.transform.position;
            distance = Vector3.ClampMagnitude(distance, cameraMaxDistance);
            mainCamera.transform.position = player.transform.position + (Vector3)distance / 3 + new Vector3(0, 0, -5);
        }
    }

    public Vector3 GetMouseWorldPosition(Vector3 screenPos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        return worldPos;
    }
}
