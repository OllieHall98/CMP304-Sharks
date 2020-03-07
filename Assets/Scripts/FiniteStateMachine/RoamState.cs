using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BoatChase
{
    public class RoamState : BaseState
    {
        Vector3 destination = Vector3.zero;
        Vector3 direction = Vector3.zero;
        Quaternion targetRotation;

        float roamTime = 0;

        public RoamState(Monster pawn) : base(pawn)
        {
        }

        public override void OnEnter()
        {
            shark.stateImage.color = Color.clear;
            shark.animator.SetBool("Chasing", false);

            roamTime = 0;
        }

        public override Type Update()
        {
            TickRoamTime();
            if (NeedToChangeDirection()) { ChangeDirection(); }
            DrawRay();
            MoveInDirection();

            if (RaycastHitsBoat()) { return typeof(ChaseState); }
            else { return typeof(RoamState); }
        }

        void DrawRay() { Debug.DrawRay(shark.transform.position, direction * 20f, Color.cyan); }

        void MoveInDirection()
        {
            // The shark looks and moved towards currently determine direction

            shark.transform.rotation = Quaternion.Lerp(shark.transform.rotation, targetRotation, shark.turnSpeed * Time.deltaTime);
            shark.transform.position += shark.transform.forward * shark.thrustSpeed * Time.deltaTime;
        }
        bool NeedToChangeDirection() // Change direction if a certain amount of time has passed
        {
            if (roamTime > UnityEngine.Random.Range(0.5f, 4.0f)) { ChangeDirection(); roamTime = 0; return true; }
            return false;
        }
        void TickRoamTime() { roamTime += Time.deltaTime; }
        bool RaycastHitsBoat() // Send a cone of raycasts to see whats ahead
        {
            bool canSeePlayer = false;

            if (
                  (Raycast(shark.transform.forward - shark.transform.right * 2, shark.viewDistance * 0.4f)) ||
                  (Raycast(shark.transform.forward - shark.transform.right, shark.viewDistance * 0.6f)) ||
                  (Raycast(shark.transform.forward - (shark.transform.right / 2), shark.viewDistance * 0.8f)) ||
                  (Raycast(shark.transform.forward, shark.viewDistance)) ||
                  (Raycast(shark.transform.forward + (shark.transform.right / 2), shark.viewDistance * 0.8f)) ||
                  (Raycast(shark.transform.forward + (shark.transform.right / 2), shark.viewDistance * 0.8f)) ||
                  (Raycast(shark.transform.forward + shark.transform.right, shark.viewDistance * 0.6f)) ||
                  (Raycast(shark.transform.forward + shark.transform.right * 2, shark.viewDistance * 0.4f))
               )
            { canSeePlayer = true; }

            return canSeePlayer; // if the player is seen in any of the raycasts, transition to chase state
        }
        bool Raycast(Vector3 direction, float distance)
        {
            RaycastHit hit;
            if (Physics.Raycast(shark.raycastOrigin.position, direction, out hit, distance))
            {
                if (hit.transform.tag == "Boat")
                {
                    shark.target = hit.transform; // if raycast finds player, set it as the shark's target
                    return true;
                }
                else
                {
                    ChangeDirection(-hit.transform.position); // if raycast finds wall or another shark, call this function
                    return false;
                }
            }
            else
            {
                Debug.DrawRay(shark.raycastOrigin.position, direction * distance, Color.green);
                return false;
            }
        }
        void ChangeDirection()
        {
            int direction = UnityEngine.Random.Range(1, 3);
            float turnStrength = 0;

            // Pick a random direction
            if (direction == 1) { turnStrength = UnityEngine.Random.Range(-1, -60); }
            else if (direction == 2) { turnStrength = UnityEngine.Random.Range(1, 60); }

            // set the target rotation to newly determined angle
            targetRotation = Quaternion.Euler(shark.transform.eulerAngles.x, shark.transform.eulerAngles.y + turnStrength, shark.transform.eulerAngles.z);
        }

        void ChangeDirection(Vector3 oppositeDirection)
        {
            // This turns the shark to face in the opposite direction

            destination = new Vector3(oppositeDirection.x, 1f, oppositeDirection.z);
            direction = Vector3.Normalize(destination - shark.transform.position);
            direction = new Vector3(direction.x, 0f, direction.z);
            targetRotation = Quaternion.LookRotation(direction);
        }
    }
}

