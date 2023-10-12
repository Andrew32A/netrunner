using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [Header("Camera Bobbing Settings")]
    public bool enableCameraBobbing;
    public float walkBobbingSpeed = 14f;
    public float walkBobbingAmount = 0.05f;
    public float runBobbingSpeed = 24f;
    public float runBobbingAmount = 0.1f;
    private float defaultPosY;
    private float timer = 0f;

    [Header("References")]
    public Transform cameraPosition;

    void Start()
    {
        defaultPosY = cameraPosition.position.y;
    }

    void Update()
    {
        HandleCameraPosition();

        if (enableCameraBobbing)
        {
            HandleCameraBobbing();
        }
    }

    void HandleCameraPosition()
    {
        transform.position = cameraPosition.position;
    }

    void HandleCameraBobbing()
    {
        float currentYPosition = cameraPosition.position.y;

        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f)
        {
            timer += Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? runBobbingSpeed : walkBobbingSpeed);
            float yOffset = Mathf.Sin(timer) * (Input.GetKey(KeyCode.LeftShift) ? runBobbingAmount : walkBobbingAmount);
            transform.position = new Vector3(transform.position.x, currentYPosition + yOffset, transform.position.z);
        }
        else
        {
            timer = 0;
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, currentYPosition, transform.position.z), Time.deltaTime * walkBobbingSpeed);
        }
    }

}
