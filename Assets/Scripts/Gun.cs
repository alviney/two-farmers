using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float bulletForce = 1f;
    public Transform spawnPoint;
    public GameObject bulletPrefab;
    public void Fire()
    {
        GameObject prefab = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);

        Rigidbody rb = prefab.GetComponent<Rigidbody>();

        rb.AddForce(spawnPoint.forward * bulletForce);
    }
}
