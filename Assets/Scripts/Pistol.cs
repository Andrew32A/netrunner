using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    [Header("Range")]
    public float raycastRange; // default: 100f

    [Header("Ammo")]
    public int currentAmmo;
    public int reserveAmmo; // default: 7 or 999999
    public int maxAmmo; // default: 7

    [Header("Reload")]
    public float reloadTime = 0.4f; // default: 0.4f
    private bool isReloading = false;

    [Header("Recoil")]
    public float kickbackAmount; // default: 25f
    public float returnSpeed; // default: 40f
    public float slideZDistance; // default: -0.2f

    [Header("Gun Slinger Animation")]
    public float sideTurnDuration; // default: 0.05f
    public float horizontalSpinDuration; // default: 0.2f
    public float verticalSpinDuration; // default: 0.25f
    public float step3TransitionDuration; // default 0.1f
    private bool isAnimatingGunSlinger = false;

    [Header("References")]
    public Transform shootPoint;
    public Transform slideTransform;
    private Vector3 originalRotation;
    private Vector3 originalSlidePosition;

    void Start()
    {
        currentAmmo = maxAmmo;

        // store original pistol rotation and slide position
        originalRotation = transform.localEulerAngles;
        originalSlidePosition = slideTransform.localPosition;

        StartCoroutine(GunSlingerTrick());
    }

    void Update()
    {
        if (isReloading || isAnimatingGunSlinger)
            return;

        if (Input.GetButtonDown("Fire1") && currentAmmo > 0)
        {
            Shoot();
            Kickback();
            MoveSlide();
        }

        // reload if pistol ammo is not full and reserve ammo is available
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && reserveAmmo > 0)
        {
            StartCoroutine(Reload());
        }

        // play gun slinger animation if pistol ammo is full
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo == maxAmmo)
        {
            StartCoroutine(GunSlingerTrick());
        }

        // slowly return pistol to original rotation
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(originalRotation), returnSpeed * Time.deltaTime);

        // Check ammo count to decide slide position
        if (currentAmmo == 0)
        {
            LockSlideBack();
        }
        else
        {
            // slowly return slide to original position
            slideTransform.localPosition = Vector3.Lerp(slideTransform.localPosition, originalSlidePosition, returnSpeed * Time.deltaTime);
        }
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

    void MoveSlide()
    {
        slideTransform.localPosition += new Vector3(0, 0, slideZDistance);
    }

    void LockSlideBack()
    {
        slideTransform.localPosition = new Vector3(originalSlidePosition.x, originalSlidePosition.y, originalSlidePosition.z + slideZDistance);
    }

    IEnumerator Reload()
    {
        isReloading = true;
        float elapsedTime = 0;
        float spinSpeed = 720f / reloadTime; // spins for 2 rotations

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

    IEnumerator GunSlingerTrick()
    {
        isAnimatingGunSlinger = true;

        // 1. rotate the pistol's z-axis to 90 degrees
        Quaternion sideRotation = Quaternion.Euler(originalRotation.x, originalRotation.y, originalRotation.z + 90);
        float startTime = Time.time;
        while (Time.time < startTime + sideTurnDuration)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, sideRotation, (Time.time - startTime) / sideTurnDuration);
            yield return null;
        }

        // 2. spin the pistol horizontally twice
        startTime = Time.time;
        while (Time.time < startTime + horizontalSpinDuration)
        {
            transform.Rotate(720f / horizontalSpinDuration * Time.deltaTime, 0, 0);
            yield return null;
        }

        // 3. rotate the pistol's z-axis back to 0
        Quaternion endRotationOfStep3 = Quaternion.Euler(originalRotation);
        startTime = Time.time;
        while (Time.time < startTime + step3TransitionDuration)
        {
            float t = (Time.time - startTime) / step3TransitionDuration;
            transform.localRotation = Quaternion.Lerp(sideRotation, endRotationOfStep3, t);
            yield return null;
        }

        // 4. spin the pistol vertically
        startTime = Time.time;
        while (Time.time < startTime + verticalSpinDuration)
        {
            transform.Rotate(360f / verticalSpinDuration * Time.deltaTime, 0, 0);
            yield return null;
        }

        // reset the pistol's rotation to the original position
        transform.localRotation = Quaternion.Euler(originalRotation);

        isAnimatingGunSlinger = false;
    }
}

// *********************************************************************************************************************
// Code that was removed but may be useful later for upgrades:

// auto reload when out of ammo
// if (Input.GetButtonDown("Fire1") && currentAmmo == 0 && reserveAmmo > 0)
// {
//     StartCoroutine(Reload());
// }
