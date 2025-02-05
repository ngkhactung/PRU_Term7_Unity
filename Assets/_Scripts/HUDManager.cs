using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; set; }

    [Header("Ammo")]
    public TextMeshProUGUI magazineAmmoUI;
    public TextMeshProUGUI totalAmmoUI;
    public Image ammoTypeUI;
    public Image magazineTypeUI;

    [Header("Weapon")]
    public Image activeWeaponUI;
    public Image unActiveWeaponUI;

    [Header("Throwables")]
    public TextMeshProUGUI lethalAmountUI;
    public Image lethalUI;

    public TextMeshProUGUI taticalAmountUI;
    public Image taticalUI;

    public Sprite emptyWeapon;
    public Sprite emptySprite;
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

    private void Update()
    {
        Weapon activeWeapon = WeaponManager.Instance.activeWeaponSlot.GetComponentInChildren<Weapon>();
        Weapon unActiveWeapon = GetUnActiveWeaponSlot().GetComponentInChildren<Weapon>();

        if (activeWeapon != null)
        {
            magazineAmmoUI.text = $"{activeWeapon.bulletsLeft / activeWeapon.bulletsPerBurst}";
            totalAmmoUI.text = $"{activeWeapon.CheckAmmoLeftOfCurrentWeapon() / activeWeapon.bulletsPerBurst}";

            magazineTypeUI.sprite = GetMagazineSprite(activeWeapon.currentWeaponModel);
            ammoTypeUI.sprite = GetAmmoSprite(activeWeapon.currentWeaponModel);

            activeWeaponUI.sprite = GetWeaponSprite(activeWeapon.currentWeaponModel);
        }
        else
        {
            magazineAmmoUI.text = "0";
            totalAmmoUI.text = "0";

            magazineTypeUI.sprite = Resources.Load<GameObject>("Magazine").GetComponent<SpriteRenderer>().sprite;
            ammoTypeUI.sprite = Resources.Load<GameObject>("Ammo").GetComponent<SpriteRenderer>().sprite;
            activeWeaponUI.sprite = emptyWeapon;
        }

        if (unActiveWeapon != null)
        {
            unActiveWeaponUI.sprite = GetWeaponSprite(unActiveWeapon.currentWeaponModel);
        }
        else
        {
            unActiveWeaponUI.sprite = emptySprite;
        }
    }

    private GameObject GetUnActiveWeaponSlot()
    {
        foreach (var weaponSlot in WeaponManager.Instance.weaponSlots)
        {
            if (weaponSlot != WeaponManager.Instance.activeWeaponSlot)
                return weaponSlot;
        }
        return null;
    }

    private Sprite GetAmmoSprite(Weapon.WeaponModel model)
    {
        switch (model)
        {
            case Weapon.WeaponModel.PistolGray:
                return Resources.Load<GameObject>("Pistol_Ammo").GetComponent<SpriteRenderer>().sprite;
            case Weapon.WeaponModel.M4A1:
                return Resources.Load<GameObject>("Rifle_Ammo").GetComponent<SpriteRenderer>().sprite;
            default:
                return null;
        }
    }

    private Sprite GetMagazineSprite(Weapon.WeaponModel model)
    {
        switch (model)
        {
            case Weapon.WeaponModel.PistolGray:
                return Resources.Load<GameObject>("Pistol_Magazine").GetComponent<SpriteRenderer>().sprite;
            case Weapon.WeaponModel.M4A1:
                return Resources.Load<GameObject>("Rifle_Magazine").GetComponent<SpriteRenderer>().sprite;
            default:
                return null;
        }
    }

    private Sprite GetWeaponSprite(Weapon.WeaponModel model)
    {
        switch (model)
        {
            case Weapon.WeaponModel.PistolGray:
                return Resources.Load<GameObject>("Pistol_Weapon").GetComponent<SpriteRenderer>().sprite;
            case Weapon.WeaponModel.M4A1:
                return Resources.Load<GameObject>("M4A1_Weapon").GetComponent<SpriteRenderer>().sprite;
            default:
                return null;
        }
    }
}
