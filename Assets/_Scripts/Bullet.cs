using UnityEngine;

public class Bullet : MonoBehaviour
{
    //Bullet Impact Effect
    [Header("Bullet Impact Effect")]
    public GameObject bulletImpactEffectPrefab;

    private void OnCollisionEnter(Collision objectWeHit)
    {
        //Collide with target objects
        if (objectWeHit.gameObject.CompareTag("Target"))
        {
            CreateBulletImpactEffect(objectWeHit);
            Destroy(gameObject);
        }

        //Collide with target objects
        if (objectWeHit.gameObject.CompareTag("Wall"))
        {
            CreateBulletImpactEffect(objectWeHit);
            Destroy(gameObject);
        }
    }

    private void CreateBulletImpactEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.GetContact(0);
        GameObject hole = Instantiate(bulletImpactEffectPrefab, contact.point, Quaternion.LookRotation(contact.normal));
        hole.transform.SetParent(objectWeHit.gameObject.transform);
    }
}
