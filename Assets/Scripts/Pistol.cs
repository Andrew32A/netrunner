using System.Collections;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float bulletForce = 20f;
    public int maxAmmo = 7;
    private int currentAmmo;
    public int reserveAmmo = 7;
    private bool isReloading = false;

    void Start()
    {
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        if (isReloading)
            return;

        if (Input.GetButtonDown("Fire1") && currentAmmo > 0)
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && reserveAmmo > 0)
        {
            StartCoroutine(Reload());
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(shootPoint.forward * bulletForce, ForceMode.Impulse);
        currentAmmo--;
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

        transform.localRotation = Quaternion.identity;

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
