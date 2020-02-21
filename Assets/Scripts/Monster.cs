using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoatChase
{
    public class Monster : MonoBehaviour
    {

        [Space(10)]
        [SerializeField] bool playerControlled = false;
        [SerializeField] Transform raycastOrigin;

        [Header("Movement")] [Space(10)]
        [SerializeField] float thrustSpeed = 10f;
        [SerializeField] float turnSpeed = 10f;
        [SerializeField] float chaseSpeedMultiplier = 3.0f;
        

        [Header("Detection/Chasing")] [Space(10)]
        [SerializeField] float chaseRange = 50f;
        [SerializeField] float coneOfVision = 20f;

        private BaseState currentState;

        private float thrustValue = 0;
        private float turnValue = 0;
        private Rigidbody rb;

        float roamTime = 0;

        Vector3 destination = Vector3.zero;
        Vector3 direction = Vector3.zero;
        Quaternion targetRotation;

        Transform target;

        [SerializeField] private UIScript UI;

       // public UIScript Interface => UIscript;


        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            UI = UIScript.UI;
        }

        // Update is called once per frame
        void Update()
        {
            if (playerControlled)
            {
                GetPlayerControls();
            }
            else
            {
                AIControls();
            }

        }

        void GetPlayerControls()
        {
            thrustValue = Input.GetAxis("Vertical");
            turnValue = Input.GetAxis("Horizontal");

            ApplyForces();
        }

        void AIControls()
        {
            switch(currentState)
            {
                case BaseState.Roam:
                {
                        roamTime += Time.deltaTime;

                        if (NeedToChangeDirection())
                        {
                            ChangeDirection();
                        }

                        // Draw a debug raycast representing the target direction
                        Debug.DrawRay(transform.position, direction * 20f, Color.cyan);

                        // Move around
                        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.005f);
                        rb.AddForce(transform.forward * Time.deltaTime * thrustSpeed * 100);

                        // If a boat is spotted, begin chase state
                        if (SendRaycasts())
                        {
                            SetCurrentState(BaseState.Chase);
                        }
                        break;
                }
                case BaseState.Chase:
                {
                       if(target == null)
                        {
                            SetCurrentState(BaseState.Roam);
                            return;
                        }

                        chaseBoat();

                        if(boatEscaped())
                        {
                            SetCurrentState(BaseState.Roam);
                        }

                        break;
                }
            }
        }

        void SetCurrentState(BaseState state)
        {
            currentState = state;
        }

        void chaseBoat()
        {
            Debug.DrawRay(transform.position, target.position - transform.position, Color.red);

            Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
            targetRotation.x = 0;
            targetRotation.z = 0;


            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.1f);
            rb.AddForce(transform.forward * Time.deltaTime * thrustSpeed * (100 * chaseSpeedMultiplier));
        }

        bool boatEscaped()
        {
            if(Vector2.Distance(transform.position, target.position) > chaseRange)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool SendRaycasts()
        {
            bool canSeePlayer = false;

            if (
                  (Raycast(transform.forward - transform.right * 2, coneOfVision * 0.4f))     ||
                  (Raycast(transform.forward - transform.right, coneOfVision * 0.6f))         ||
                  (Raycast(transform.forward - (transform.right / 2), coneOfVision * 0.8f))   ||
                  (Raycast(transform.forward, coneOfVision))                                  ||
                  (Raycast(transform.forward + (transform.right / 2), coneOfVision * 0.8f))   ||
                  (Raycast(transform.forward + (transform.right / 2), coneOfVision * 0.8f))   ||
                  (Raycast(transform.forward + transform.right, coneOfVision * 0.6f))         ||
                  (Raycast(transform.forward + transform.right * 2, coneOfVision * 0.4f))
               )
            {
                canSeePlayer = true;
            }



            return canSeePlayer;
        }

        bool Raycast(Vector3 direction, float distance)
        {
            RaycastHit hit;
            if (Physics.Raycast(raycastOrigin.position, direction, out hit, distance))
            {
                if (hit.transform.tag == "Boat")
                {
                    Debug.DrawRay(raycastOrigin.position, direction * distance, Color.red);
                    target = hit.transform;
                    return true;
                }
                else
                {
                    ChangeDirection();
                    return false;
                }
            }
            else
            {
                Debug.DrawRay(raycastOrigin.position, direction * distance, Color.white);
                return false;
            }
        }

        private bool NeedToChangeDirection()
        {
            if (destination == Vector3.zero)
                return true;

            if(roamTime > Random.Range(0.8f, 4.0f))
            {
                roamTime = 0;
                return true;
            }

            return false;
        }

        private void ChangeDirection()
        {
            Vector3 testPosition = (transform.position + new Vector3(Random.Range(-5.0f, 5.0f), 0f, Random.Range(-5.0f, 5.0f)));

            

            destination = new Vector3(testPosition.x, 1f, testPosition.z);

            direction = Vector3.Normalize(destination - transform.position);
            direction = new Vector3(direction.x, 0f, direction.z);
            targetRotation = Quaternion.LookRotation(direction);
        }

        void ApplyForces()
        {
            rb.AddForce(transform.forward * thrustSpeed * thrustValue);

            if (thrustValue > 0.1f || thrustValue < -0.1f)
            {
                rb.AddTorque(transform.up * turnSpeed * turnValue);
            }
        }
    }
}
