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

    [Header("Middle Point")]
    public GameObject middlePoint;

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
        CheckWeaponUI();

        CheckThrowableUI();
    }

    #region --- Weapon and Ammo ---
    private void CheckWeaponUI()
    {
        Weapon activeWeapon = WeaponManager.Instance.activeWeaponSlot.GetComponentInChildren<Weapon>();
        Weapon unActiveWeapon = GetUnActiveWeaponSlot().GetComponentInChildren<Weapon>();

        if (activeWeapon != null)
        {
            magazineAmmoUI.text = $"{activeWeapon.bulletsLeft / activeWeapon.bulletsPerBurst}";
            totalAmmoUI.text = $"{activeWeapon.CheckAmmoLeftOfCurrentWeapon() / activeWeapon.bulletsPerBurst}";

            activeWeaponUI.sprite = GetWeaponSprite(activeWeapon.currentWeaponModel);
            magazineTypeUI.sprite = GetMagazineSprite(activeWeapon.currentWeaponModel);
            ammoTypeUI.sprite = GetAmmoSprite(activeWeapon.currentWeaponModel);
        }
        else
        {
            magazineAmmoUI.text = "0";
            totalAmmoUI.text = "0";

            activeWeaponUI.sprite = emptyWeapon;
            magazineTypeUI.sprite = ResourceLoad("Magazine");
            ammoTypeUI.sprite = ResourceLoad("Ammo");
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

    private Sprite GetWeaponSprite(Weapon.WeaponModel model)
    {
        switch (model)
        {
            case Weapon.WeaponModel.PistolGray:
                return ResourceLoad("Pistol_Weapon");
            case Weapon.WeaponModel.M4A1:
                return ResourceLoad("M4A1_Weapon");
            default:
                return null;
        }
    }

    private Sprite GetAmmoSprite(Weapon.WeaponModel model)
    {
        switch (model)
        {
            case Weapon.WeaponModel.PistolGray:
                return ResourceLoad("Pistol_Ammo");
            case Weapon.WeaponModel.M4A1:
                return ResourceLoad("Rifle_Ammo");
            default:
                return null;
        }
    }

    private Sprite GetMagazineSprite(Weapon.WeaponModel model)
    {
        switch (model)
        {
            case Weapon.WeaponModel.PistolGray:
                return ResourceLoad("Pistol_Magazine");
            case Weapon.WeaponModel.M4A1:
                return ResourceLoad("Rifle_Magazine");
            default:
                return null;
        }
    }
    #endregion

    #region --- Throwable ---
    private void CheckThrowableUI()
    {
        lethalUI.sprite = ResourceLoad("Grenade");
        lethalAmountUI.text = $"{WeaponManager.Instance.totalGrenade}";

        taticalUI.sprite = ResourceLoad("SmokeGrenade");
        taticalAmountUI.text = $"{WeaponManager.Instance.totalSmokeGrenade}";
    }
    #endregion

    private Sprite ResourceLoad(string objectName)
    {
        return Resources.Load<GameObject>(objectName).GetComponent<SpriteRenderer>().sprite;
    }
}
