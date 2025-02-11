using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Active Weapon")]
    public bool isActiveWeapon;

    //Animation
    private Animator animator;

    //ADS mode
    private bool isAdsMode = false;

    //Weapon models
    [Header("WeaponModel")]
    public WeaponModel currentWeaponModel;
    public enum WeaponModel
    {
        PistolGray,
        M4A1
    }

    //Shooting
    [Header("Shooting")]
    public float shootingDelay = 2f;
    private bool isShooting;
    private bool readyToShoot;
    private bool allowReset;

    //Bullet
    [Header("Bullet")]
    public GameObject butlletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 100f;
    public float bulletPerfabLifeTime = 3f;

    //Reloading
    [Header("Reloading")]
    public float reloadTime;
    public int magazineSize;
    internal int bulletsLeft;
    private bool isReloading;

    //Spread 
    [Header("Spread")]
    public float hipSpreadIntensity;
    public float adsSpreadIntensity;
    private float spreadIntensity;

    //Spawn position in player's hand
    [Header("Spawn Position")]
    public Vector3 spawnPosition;
    public Vector3 spawnRotation;

    //Burst
    [Header("Burst")]
    public int bulletsPerBurst = 3;
    public float burstShootingDelay = 0.2f;
    private int burstBulletsLeft;

    //Muzzle 
    [Header("Muzzle")]
    public GameObject muzzleEffect;

    //Shooting mode
    [Header("Shooting Mode")]
    public ShootingMode currentShootingMode;
    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }

    private void Awake()
    {
        readyToShoot = false;
        allowReset = true;
        animator = GetComponent<Animator>();

        burstBulletsLeft = bulletsPerBurst;
        spreadIntensity = hipSpreadIntensity;
    }

    // Update is called once per frame
    void Update()
    {
        // If the weapon is not active, exit the function early.
        if (isActiveWeapon == false) return;

        // Disable the white outline effect on the currently active weapon.
        GetComponent<Outline>().enabled = false;

        // Toggle ADS (Aim Down Sights) mode
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            isAdsMode = !isAdsMode;
            ToggleAdsMode(isAdsMode);
        }

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

        // Handle reloading
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize
            && CheckAmmoLeftOfCurrentWeapon() > 0 && isReloading == false)
        {
            Reload();
        }

        // Fire the weapon
        if (readyToShoot && isShooting) FireWeapon();

        // Play an empty magazine sound when trying to shoot with no bullets left.
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

        //Shooting effect
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

    private void ToggleAdsMode(bool isAdsMode)
    {
        if (isAdsMode)
        {
            animator.SetTrigger("EnterADS");
            HUDManager.Instance.middlePoint.SetActive(false);
            spreadIntensity = adsSpreadIntensity;
        }
        else
        {
            animator.SetTrigger("ExitADS");
            HUDManager.Instance.middlePoint.SetActive(true);
            spreadIntensity = hipSpreadIntensity;
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

        float z = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        //Returning the shooting direction and spread
        return direction + new Vector3(0, y, z);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float bulletPerfabLifeTime)
    {
        yield return new WaitForSeconds(bulletPerfabLifeTime);
        Destroy(bullet);
    }
}
