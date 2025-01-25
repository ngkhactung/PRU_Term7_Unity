using System;
using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject butlletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30f;
    public float bulletPerfabLifeTime = 3f;


    // Update is called once per frame
    void Update()
    {
        //Left mouse click (fired)
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            FireWeapon();
        }
    }

    private void FireWeapon()
    {
        // Instantiate the bullet
        GameObject bullet = Instantiate(butlletPrefab, bulletSpawn.position, Quaternion.identity);
    
        //Shoot the bullet
        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward.normalized * bulletVelocity, ForceMode.Impulse);

        //Destroy the bullet after the some time
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPerfabLifeTime));
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float bulletPerfabLifeTime)
    {
        yield return new WaitForSeconds(bulletPerfabLifeTime);
        Destroy(bullet);
    }
}
