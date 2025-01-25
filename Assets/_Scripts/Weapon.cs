using System;
using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Camera playerCamera;

    //Shooting
    public bool isShooting;
    public bool readyToShoot;
    public bool allowReset;
    public float shootingDelay = 2f;

    //Burst
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;
    public float burstShootingDelay = 0.2f;

    //Spread 
    public float spreadIntensity;

    //Bullet
    public GameObject butlletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 100f;
    public float bulletPerfabLifeTime = 3f;

    //Shooting mode
    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }
    public ShootingMode currentShootingMode;

    private void Awake()
    {
        readyToShoot = true;
        allowReset = true;
        burstBulletsLeft = bulletsPerBurst;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentShootingMode == ShootingMode.Auto)
        {
            //Holding Down Left Mouse Button
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
        {
            //Clicking Left Mouse Button Once
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if (readyToShoot && isShooting)
        {
            FireWeapon();
        }

    }

    private void FireWeapon()
    {
        //Avoid firing twice with one trigger pull.
        readyToShoot = false;

        Vector3 shootingDirection = CalculateDirectionAndSpread();

        // Instantiate the bullet
        GameObject bullet = Instantiate(butlletPrefab, bulletSpawn.position, Quaternion.identity);

        //Poiting the bullet to face the shooting direction
        bullet.transform.forward = shootingDirection;

        //Shoot the bullet
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection.normalized * bulletVelocity, ForceMode.Impulse);

        //Destroy the bullet after the some time
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPerfabLifeTime));

        //Used to distinguish the Burst Mode from other modes
        if (allowReset)
        {
            allowReset = false;
            Invoke("ResetShot", shootingDelay);
        }

        //Burst Mode
        if (currentShootingMode == ShootingMode.Burst)
        {
            burstBulletsLeft--;
            if (burstBulletsLeft >= 1) Invoke("FireWeapon", burstShootingDelay);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
        if (currentShootingMode == ShootingMode.Burst)
        {
            burstBulletsLeft = bulletsPerBurst;
        }
    }

    private Vector3 CalculateDirectionAndSpread()
    {
        //Shooting from the middle of the screen to check where are we pointing at 
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //Hitting something
            targetPoint = hit.point;
        }
        else
        {
            //Shooting at the air
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        //Returning the shooting direction and spread
        return direction + new Vector3(x, y, 0);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float bulletPerfabLifeTime)
    {
        yield return new WaitForSeconds(bulletPerfabLifeTime);
        Destroy(bullet);
    }
}
