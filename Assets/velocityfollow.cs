using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class velocityfollow : MonoBehaviour
{
    Rigidbody rigidbody;
    public Transform target;
    public float speed;
    public float rotation;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        //     pid = new PID(0, 0, Time.deltaTime);
    }

    private void FixedUpdate()
    {
        Vector3 pos = target.position - transform.position;
        rigidbody.velocity = pos * speed;
        rigidbody.angularVelocity = Vector3.zero;
        this.transform.localRotation = target.transform.localRotation * Quaternion.Euler(0, 0, rotation);
    }
}
