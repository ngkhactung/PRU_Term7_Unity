using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        //Collide with target objects
        if (collision.gameObject.CompareTag("Target")){
            print("hit " + collision.gameObject.name + "!");
            Destroy(gameObject);
        }
    }
}
