using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waitandpush : MonoBehaviour
{
    public float force;
    public float waitTime;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("Push", waitTime);
    }

    public void Push()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.AddForce(-transform.forward * force);
    }
}
