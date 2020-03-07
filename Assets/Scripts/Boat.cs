using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    Rigidbody rigidbody = null;

    [Header("Speeds")] [Space(10)]
    [SerializeField] float thrustSpeed = 0;
    [SerializeField] float turnSpeed = 0;

    float thrustAxis, turnAxis;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        thrustAxis = Input.GetAxis("Vertical");
        turnAxis = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        ApplyForces();
    }

    void ApplyForces()
    {
        rigidbody.AddForce(transform.forward * thrustSpeed * thrustAxis * 120 * Time.fixedDeltaTime);

        if (rigidbody.velocity.magnitude > 5.0f)
        { 
            rigidbody.AddTorque(transform.up * turnSpeed * turnAxis * 120 * Time.fixedDeltaTime);
        }
    }
}
