using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    Rigidbody rb;

    [Header("Speeds")] [Space(10)]
    [SerializeField] float thrustSpeed = 0;
    [SerializeField] float turnSpeed = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        ApplyForces();
    }

    void ApplyForces()
    {
        float thrustValue = Input.GetAxis("Vertical");
        float turnValue = Input.GetAxis("Horizontal");

        rb.AddForce(transform.forward * thrustSpeed * thrustValue);

        if (rb.velocity.magnitude > 5.0f)
        { 
            rb.AddTorque(transform.up * turnSpeed * turnValue);
        }
    }
}
