using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    public Transform shootPoint;
    public float raycastRange = 100f;
    public int maxAmmo = 7;
    private int currentAmmo;
    public int reserveAmmo = 7;
    private bool isReloading = false;

    private float kickbackAmount = 5f;
    private float returnSpeed = 1.5f;
    private Vector3 originalRotation;

    void Start()
    {
        currentAmmo = maxAmmo;
        originalRotation = transform.localEulerAngles;
    }

    void Update()
    {
        if (isReloading)
            return;

        if (Input.GetButtonDown("Fire1") && currentAmmo > 0)
        {
            Shoot();
            Kickback();
        }

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && reserveAmmo > 0)
        {
            StartCoroutine(Reload());
        }

        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(originalRotation), returnSpeed * Time.deltaTime);
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(shootPoint.position, shootPoint.forward, out hit, raycastRange))
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                // check to see what enemy we hit
                Debug.Log("Hit enemy: " + hit.transform.name);

                // destroy enemy
                Destroy(hit.transform.gameObject);

                // TODO: instead of destroying enemy, call enemy death function once it's implemented
            }
        }
        currentAmmo--;
    }

    void Kickback()
    {
        transform.localRotation = Quaternion.Euler(originalRotation.x - kickbackAmount, originalRotation.y, originalRotation.z);
    }

    IEnumerator Reload()
    {
        isReloading = true;
        float reloadTime = 0.2f;
        float elapsedTime = 0;
        float spinSpeed = 360f / reloadTime;

        while (elapsedTime < reloadTime)
        {
            transform.Rotate(spinSpeed * Time.deltaTime, 0, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = Quaternion.Euler(originalRotation);

        int ammoNeeded = maxAmmo - currentAmmo;
        if (reserveAmmo < ammoNeeded)
        {
            currentAmmo += reserveAmmo;
            reserveAmmo = 0;
        }
        else
        {
            currentAmmo = maxAmmo;
            reserveAmmo -= ammoNeeded;
        }

        isReloading = false;
    }
}
