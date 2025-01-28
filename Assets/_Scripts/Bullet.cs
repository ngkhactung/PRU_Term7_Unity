using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision objectWeHit)
    {
        //Collide with target objects
        if (objectWeHit.gameObject.CompareTag("Target")){
            print("hit " + objectWeHit.gameObject.name + "!");

            CreateBulletImpactEffect(objectWeHit);

            Destroy(gameObject);
        }
        
        //Collide with target objects
        if (objectWeHit.gameObject.CompareTag("Wall")){
            print("hit " + objectWeHit.gameObject.name + "!");

            CreateBulletImpactEffect(objectWeHit);

            Destroy(gameObject);
        }

        void CreateBulletImpactEffect(Collision objectWeHit)
        {
            ContactPoint contact = objectWeHit.GetContact(0);
            GameObject hole = Instantiate(
                    GlobalReferences.Instance.bulletImpactEffectPrefab,
                    contact.point,
                    Quaternion.LookRotation(contact.normal)
                );

            hole.transform.SetParent(objectWeHit.gameObject.transform);
        }
    }
}
