using UnityEngine;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    public AudioSource ShootingChannel;
    public AudioSource ReloadChannel;
    public AudioSource ThrowableChannel;
    public AudioSource EmptyMagazine;

    public AudioClip GrenadeExplosion;
    public AudioClip SmokeGrenadeExplosion;
    public AudioClip PistolGrayShot;
    public AudioClip PistolGrayReload;
    public AudioClip M4A1Shot;
    public AudioClip M4A1Reload;
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

    public void PlayShootingSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.PistolGray:
                ShootingChannel.PlayOneShot(PistolGrayShot); break;
            case WeaponModel.M4A1:
                ShootingChannel.PlayOneShot(M4A1Shot); break;
        }
    }

    public void PlayReloadSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.PistolGray:
                ShootingChannel.PlayOneShot(PistolGrayReload); break;
            case WeaponModel.M4A1:
                ShootingChannel.PlayOneShot(M4A1Reload); break;
        }
    }

    public void PlayThrowableSound(Throwable.ThrowableType throwableType)
    {
        switch (throwableType)
        {
            case Throwable.ThrowableType.Grenade:
                ThrowableChannel.PlayOneShot(GrenadeExplosion); break;
            case Throwable.ThrowableType.SmokeGrenade:
                ThrowableChannel.PlayOneShot(SmokeGrenadeExplosion); break;
        }
    }
}
