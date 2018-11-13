using UnityEngine;
using System.Collections;

public class RandomForce : MonoBehaviour
{
    Rigidbody rigidBody;
    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Random.Range(0, 360), transform.eulerAngles.z);
        float speed = 20;
        rigidBody.isKinematic = false;
        Vector3 force = transform.forward;
        force = new Vector3(force.x, 1, force.z);
        rigidBody.AddForce(force * speed);
    }
}