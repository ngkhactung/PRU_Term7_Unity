using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; set; }

    public List<GameObject> weaponSlots;

    public GameObject activeWeaponSlot;

    [Header("Ammo")]
    public int totalPistolAmmo = 0;
    public int totalRifleAmmo = 0;

    [Header("Throwable General")]
    public float throwForce = 20f;
    public GameObject throwableSpawn;
    public float forceMultiplier = 0;
    public float forceMultiplierLimit = 2f;

    [Header("Grenade")]
    public int maxGrenade = 3;
    public int totalGrenade = 0;
    public GameObject grenadePrefab;

    [Header("SmokeGrenade")]
    public int maxSmokeGrenade = 3;
    public int totalSmokeGrenade = 0;
    public GameObject smokeGrenadePrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        activeWeaponSlot = weaponSlots[0];
    }

    private void Update()
    {
        foreach (var weaponSlot in weaponSlots)
        {
            if (weaponSlot == activeWeaponSlot) weaponSlot.SetActive(true);
            else weaponSlot.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchActiveSlot(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchActiveSlot(1);
        }

        if (Input.GetKey(KeyCode.G) && totalGrenade > 0)
        {
            SetForceMultiplier();
        }

        if (Input.GetKeyUp(KeyCode.G) && totalGrenade > 0)
        {
            ThrowGrenade();
            forceMultiplier = 0;
        }

        if (Input.GetKey(KeyCode.T) && totalSmokeGrenade > 0)
        {
            SetForceMultiplier();
        }

        if (Input.GetKeyUp(KeyCode.T) && totalSmokeGrenade > 0)
        {
            ThrowSmokeGrenade();
            forceMultiplier = 0;
        }
    }

    #region --- Ammo --- 
    public void PickUpAmmoBox(GameObject pickedAmmoBox)
    {
        AmmoBox ammo = pickedAmmoBox.GetComponent<AmmoBox>();

        switch (ammo.ammoType)
        {
            case AmmoBox.AmmoType.PistolAmmo:
                totalPistolAmmo += ammo.ammoAmount;
                break;
            case AmmoBox.AmmoType.RifleAmmo:
                totalRifleAmmo += ammo.ammoAmount;
                break;
        }

        Destroy(pickedAmmoBox);
    }
    #endregion

    #region --- Weapon ---
    public void PickUpWeapon(GameObject pickedWeapon)
    {
        DropCurrentWeapon(pickedWeapon);

        pickedWeapon.GetComponent<Animator>().enabled = true;
        pickedWeapon.transform.SetParent(activeWeaponSlot.transform, false);
        Weapon weapon = pickedWeapon.GetComponent<Weapon>();
        pickedWeapon.transform.localPosition = new Vector3(weapon.spawnPosition.x, weapon.spawnPosition.y, weapon.spawnPosition.z);
        pickedWeapon.transform.localRotation = Quaternion.Euler(weapon.spawnRotation.x, weapon.spawnRotation.y, weapon.spawnRotation.z);
        weapon.isActiveWeapon = true;
    }

    private void DropCurrentWeapon(GameObject pickedWeapon)
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {
            var weaponToDrop = activeWeaponSlot.transform.GetChild(0).gameObject;

            weaponToDrop.GetComponent<Weapon>().isActiveWeapon = false;
            weaponToDrop.GetComponent<Animator>().enabled = false;
            weaponToDrop.transform.SetParent(pickedWeapon.transform.parent, false);
            weaponToDrop.transform.localPosition = pickedWeapon.transform.localPosition;
            weaponToDrop.transform.localRotation = pickedWeapon.transform.localRotation;
        }
    }

    private void SwitchActiveSlot(int SlotNumber)
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon currentWeapon = activeWeaponSlot.transform.GetChild(0).gameObject.GetComponent<Weapon>();
            currentWeapon.isActiveWeapon = false;
        }

        activeWeaponSlot = weaponSlots[SlotNumber];

        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon newWeapon = activeWeaponSlot.transform.GetChild(0).gameObject.GetComponent<Weapon>();
            newWeapon.isActiveWeapon = true;
        }
    }
    #endregion

    #region --- Throwable ---
    public void PickUpThrowable(GameObject pickedThrowable)
    {
        Throwable throwable = pickedThrowable.GetComponent<Throwable>();

        switch (throwable.throwableType)
        {
            case Throwable.ThrowableType.Grenade:
                if (totalGrenade < maxGrenade)
                {
                    totalGrenade += 1;
                    Destroy(pickedThrowable);
                }
                break;
            case Throwable.ThrowableType.SmokeGrenade:
                if (totalSmokeGrenade < maxSmokeGrenade)
                {
                    totalSmokeGrenade += 1;
                    Destroy(pickedThrowable);
                }
                break;
        }
    }

    private void SetForceMultiplier()
    {
        forceMultiplier += Time.deltaTime;
        if (forceMultiplier >= forceMultiplierLimit) forceMultiplier = forceMultiplierLimit;
    }

    private void ThrowGrenade()
    {
        GameObject throwable = Instantiate(grenadePrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);

        Rigidbody rigidbody = throwable.GetComponent<Rigidbody>();

        rigidbody.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);

        throwable.GetComponent<Throwable>().hasBeenThrown = true;

        totalGrenade -= 1;
    }

    private void ThrowSmokeGrenade()
    {
        GameObject throwable = Instantiate(smokeGrenadePrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);

        Rigidbody rigidbody = throwable.GetComponent<Rigidbody>();

        rigidbody.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);

        throwable.GetComponent<Throwable>().hasBeenThrown = true;

        totalSmokeGrenade -= 1;
    }
    #endregion
}
