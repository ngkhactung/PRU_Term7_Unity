using UnityEngine;

public class Throwable : MonoBehaviour
{
    //Countdown to explosion
    private float countdown;
    internal bool hasBeenThrown = false;

    //Throwable Type
    [Header("Type")]
    public ThrowableType throwableType;
    public enum ThrowableType
    {
        Grenade,
        SmokeGrenade
    }

    //Technical specifications 
    [Header("Specifications")]
    [SerializeField] float delay = 3f;
    [SerializeField] float damageRadius = 3f;
    [SerializeField] float explosionForce = 1200f;

    //Explosion Effect
    [Header("Effect")]
    public GameObject explosionEffect;

    private void Awake()
    {
        countdown = delay;
    }

    private void Update()
    {
        if (hasBeenThrown)
        {
            countdown -= Time.deltaTime;
            if(countdown <= 0)
            {
                Explode();
                Destroy(gameObject);
            }
        }
    }

    private void Explode()
    {
        switch (throwableType)
        {
            case ThrowableType.Grenade:
                GrenadeEffect();
                break;
            case ThrowableType.SmokeGrenade:
                SmokeGrenadeEffect();
                break;
        }
    }

    private void GrenadeEffect()
    {
        //Visual Effect
        Instantiate(explosionEffect, transform.position, transform.rotation);

        SoundManager.Instance.PlayThrowableSound(throwableType);

        //Physical Effect
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach(Collider objectInRange in colliders)
        {
            Rigidbody rigidbody = objectInRange.GetComponent<Rigidbody>();
            if(rigidbody != null)
            {
                rigidbody.AddExplosionForce(explosionForce, transform.position, damageRadius);
            }
        }
    }

    private void SmokeGrenadeEffect()
    {
        //Visual Effect
        Instantiate(explosionEffect, transform.position, transform.rotation);

        SoundManager.Instance.PlayThrowableSound(throwableType);

        //Physical Effect
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (Collider objectInRange in colliders)
        {
            Rigidbody rigidbody = objectInRange.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
            }
        }
    }
}
