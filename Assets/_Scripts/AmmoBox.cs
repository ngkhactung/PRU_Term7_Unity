using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    //Ammo Type
    [Header("Type")]
    public AmmoType ammoType;
    public enum AmmoType
    {
        RifleAmmo,
        PistolAmmo
    }

    //Size of Ammo box
    [Header("Capacity")]
    public int ammoAmount = 200;
}
