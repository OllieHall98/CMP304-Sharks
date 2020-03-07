using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BoatChase
{
    public class ChaseState : BaseState
    {
        public float chaseSpeedMultiplier = 3.0f;

        public ChaseState(Monster pawn) : base(pawn)
        {
        }

        public override void OnEnter()
        {
            shark.stateImage.color = Color.red;
            shark.animator.SetBool("Chasing", true);
        }

        public override Type Update()
        {
            if (shark.target == null) { return typeof(RoamState); }

            chaseBoat();

            if (boatEscaped()) { return typeof(SearchState); }
            else { return typeof(ChaseState); }
        }

        void chaseBoat()
        {
            // Chase player at variable speeds

            Debug.DrawRay(shark.transform.position, shark.target.position - shark.transform.position, Color.red);

            Quaternion targetRotation = Quaternion.LookRotation(shark.target.position - shark.transform.position); targetRotation.x = 0; targetRotation.z = 0;

            if      (Vector3.Distance(shark.transform.position, shark.target.position) > 80) { chaseSpeedMultiplier = 2.5f; }
            else if (Vector3.Distance(shark.transform.position, shark.target.position) > 60) { chaseSpeedMultiplier = 1.7f; }
            else if (Vector3.Distance(shark.transform.position, shark.target.position) > 40) { chaseSpeedMultiplier = 1.0f; }
            else if (Vector3.Distance(shark.transform.position, shark.target.position) > 25) { chaseSpeedMultiplier = 0.8f; }
            else if (Vector3.Distance(shark.transform.position, shark.target.position) < 25) { chaseSpeedMultiplier = 0.2f; }

            shark.transform.rotation = Quaternion.Lerp(shark.transform.rotation, targetRotation, shark.turnSpeed * Time.deltaTime);
            shark.transform.position += shark.transform.forward * chaseSpeedMultiplier * shark.thrustSpeed * Time.deltaTime;
        }

        bool boatEscaped()
        {
            // If player escapes fixed range

            if (Vector2.Distance(shark.transform.position, shark.target.position) > shark.chaseRange)
                return true;
            else 
                return false;
        }
    }
}

