using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool isActiveWeapon;

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

    //Reloading
    public float reloadTime;
    public int magazineSize;
    public int bulletsLeft;
    public bool isReloading;

    //Spawn position in player's hand
    public Vector3 spawnPosition;
    public Vector3 spawnRotation;

    //Muzzle 
    public GameObject muzzleEffect;

    //Animation
    private Animator animator;

    //Weapon models
    public enum WeaponModel
    {
        PistolGray,
        M4A1
    }
    public WeaponModel currentWeaponModel;

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
        readyToShoot = false;
        allowReset = true;
        animator = GetComponent<Animator>();

        burstBulletsLeft = bulletsPerBurst;
        //bulletsLeft = magazineSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActiveWeapon == false) return;

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

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize
            && CheckAmmoLeftOfCurrentWeapon() > 0 && isReloading == false)
        {
            Reload();
        }

        if (readyToShoot && isShooting) FireWeapon();

        if (bulletsLeft == 0 && isShooting)
            SoundManager.Instance.EmptyMagazine.Play();

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
        bulletsLeft--;

        //Create muzzle effect
        muzzleEffect.GetComponent<ParticleSystem>().Play();
        animator.SetTrigger("RECOIL");
        SoundManager.Instance.PlayShootingSound(currentWeaponModel);

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
        allowReset = true;
        if (bulletsLeft > 0 && isReloading == false) readyToShoot = true;
        //if (currentShootingMode == ShootingMode.Burst)
        //{
        //    burstBulletsLeft = bulletsPerBurst;
        //}
    }

    private void Reload()
    {
        isReloading = true;
        readyToShoot = false;
        SoundManager.Instance.PlayReloadSound(currentWeaponModel);
        animator.SetTrigger("RELOAD");
        Invoke("ReloadCompleted", reloadTime);
    }

    private void ReloadCompleted()
    {
        //Calculate the loaded bullets and the total remaining ammo.
        CalculateBulletLeft();
        readyToShoot = true;
        isReloading = false;
    }

    public int CheckAmmoLeftOfCurrentWeapon()
    {
        switch (currentWeaponModel)
        {
            case WeaponModel.PistolGray:
                return WeaponManager.Instance.totalPistolAmmo;
            case WeaponModel.M4A1:
                return WeaponManager.Instance.totalRifleAmmo;
            default:
                return 0;
        }
    }

    private void CalculateBulletLeft()
    {
        // Calculate the remaining ammo in inventory after reloading
        int remainingTotalAmmo = bulletsLeft + CheckAmmoLeftOfCurrentWeapon() - magazineSize;

        // If there is enough ammo to fully reload the magazine
        if (remainingTotalAmmo >= 0)
        {
            bulletsLeft = magazineSize; // Fill the magazine to its full capacity
            DecreaseTotalAmmo(remainingTotalAmmo);
        }
        else
        {
            bulletsLeft += CheckAmmoLeftOfCurrentWeapon(); // Add all available ammo to the magazine
            DecreaseTotalAmmo(0);
        }
    }

    private void DecreaseTotalAmmo(int remainingTotalAmmo)
    {
        switch (currentWeaponModel)
        {
            case WeaponModel.PistolGray:
                WeaponManager.Instance.totalPistolAmmo = remainingTotalAmmo;
                break;
            case WeaponModel.M4A1:
                WeaponManager.Instance.totalRifleAmmo = remainingTotalAmmo;
                break;
        }
    }

    private Vector3 CalculateDirectionAndSpread()
    {
        //Shooting from the middle of the screen to check where are we pointing at 
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
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
