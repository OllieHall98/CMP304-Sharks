using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BoatChase
{
    public class SearchState : BaseState
    {
        Vector3 destination = Vector3.zero;
        Vector3 direction = Vector3.zero;
        Quaternion targetRotation;

        public float searchSpeedMultiplier = 1.5f;
        float SearchTime = 0;

        public SearchState(Monster pawn) : base(pawn)
        {
        }

        public override void OnEnter()
        {
            shark.stateImage.color = Color.yellow;
            shark.animator.SetBool("Chasing", false);

            shark.target = null;

            SearchTime = 0;
        }

        public override Type Update()
        {
            SearchTime += Time.deltaTime;

            shark.transform.rotation = Quaternion.Lerp(shark.transform.rotation, targetRotation, shark.turnSpeed * Time.deltaTime);
            shark.transform.position += (shark.transform.forward * searchSpeedMultiplier * shark.thrustSpeed) * Time.deltaTime;

            if (SearchTime >= 4.0f) { return typeof(RoamState); } // Go back to roaming after 4 seconds

            if (RaycastHitsBoat()) { return typeof(ChaseState); } // If raycasts find player
            else return typeof(SearchState);
        }

        bool RaycastHitsBoat()
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

            return canSeePlayer;
        }

        void ChangeDirection(Vector3 oppositeDirection) // Turn to the opposite direction
        {
            destination = new Vector3(oppositeDirection.x, 1f, oppositeDirection.z);
            direction = Vector3.Normalize(destination - shark.transform.position);
            direction = new Vector3(direction.x, 0f, direction.z);
            targetRotation = Quaternion.LookRotation(direction);
        }

        bool Raycast(Vector3 direction, float distance)
        {
            Debug.DrawRay(shark.raycastOrigin.position, direction * distance, Color.yellow);

            RaycastHit hit;
            if (Physics.Raycast(shark.raycastOrigin.position, direction, out hit, distance))
            {
                if (hit.transform.tag == "Boat")
                {
                    shark.target = hit.transform;
                    return true;
                }
                else
                {
                    ChangeDirection(-hit.transform.position); // if wall or shark seen, avoid
                    return false;
                }
            }
            else return false;
        }
    }
}

