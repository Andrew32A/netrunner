using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolPickup : MonoBehaviour
{
    [Header("Floating Parameters")]
    public float rotationSpeed; // default: 50.0f
    public float floatAmplitude; // default: 0.5f
    public float floatFrequency; // default: 1.0f

    [Header("Pickup Parameters")]
    public float pickupRadius; // default: 2.0f
    public GameObject playerPistol;
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        FloatAndSpin();
        CheckForPlayerPickup();
    }

    private void FloatAndSpin()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.World);
        transform.position = initialPosition + new Vector3(0, Mathf.Sin(Time.time * floatFrequency) * floatAmplitude, 0);
    }

    private void CheckForPlayerPickup()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                if (playerPistol)
                {
                    playerPistol.SetActive(true);
                    Destroy(gameObject);
                }
            }
        }
    }
}
