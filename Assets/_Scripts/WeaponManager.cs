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
    }

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

    public void PickUpWeapon(GameObject pickedWeapon)
    {
        DropCurrentWeapon(pickedWeapon);

        pickedWeapon.GetComponent<Animator>().enabled = true;
        pickedWeapon.GetComponent<Outline>().enabled = false;
        pickedWeapon.transform.SetParent(activeWeaponSlot.transform, false);
        Weapon weapon = pickedWeapon.GetComponent<Weapon>();
        pickedWeapon.transform.localPosition = new Vector3(weapon.spawnPosition.x, weapon.spawnPosition.y, weapon.spawnPosition.z);
        pickedWeapon.transform.localRotation = Quaternion.Euler(weapon.spawnRotation.x, weapon.spawnRotation.y, weapon.spawnRotation.z);
        weapon.isActiveWeapon = true;
    }

    private void DropCurrentWeapon(GameObject pickedWeapon)
    {
        if(activeWeaponSlot.transform.childCount > 0)
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
        if(activeWeaponSlot.transform.childCount > 0)
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
}
