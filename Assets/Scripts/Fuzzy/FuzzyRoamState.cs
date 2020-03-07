using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FLS;
using FLS.Rules;
using FLS.MembershipFunctions;

namespace BoatChase
{
    public class FuzzyRoamState : FuzzyBaseState
    {
        protected IFuzzyEngine engine_wallDetection;
        protected IFuzzyEngine engine_playerDetection;

        private LinguisticVariable distanceToWall;
        private LinguisticVariable turnStrength;
        private LinguisticVariable playerDistance;
        private LinguisticVariable playerDetected;

        Vector3 direction = Vector3.zero;
        Quaternion targetRotation;

        float roamTime = 0;

        public FuzzyRoamState(Monster pawn) : base(pawn)
        {
            engine_wallDetection = new FuzzyEngineFactory().Default();
            engine_playerDetection = new FuzzyEngineFactory().Default();

            distanceToWall = new LinguisticVariable("distanceToWall");
            var farDistance = distanceToWall.MembershipFunctions.AddTrapezoid("WallFar", 70, 70, 90, 120);
            var midDistance = distanceToWall.MembershipFunctions.AddTrapezoid("WallMid", 50, 60, 70, 80);
            var nearDistance = distanceToWall.MembershipFunctions.AddTrapezoid("WallNear", 0, 20, 50, 70);

            turnStrength = new LinguisticVariable("turnStrength");
            var weak = turnStrength.MembershipFunctions.AddTrapezoid("Weak", 1, 4, 8, 12);
            var medium = turnStrength.MembershipFunctions.AddTrapezoid("Medium", 6, 12, 18, 24);
            var strong = turnStrength.MembershipFunctions.AddTrapezoid("Strong", 15, 30, 60, 80);

            playerDetected = new LinguisticVariable("playerDetected");
            var detected = playerDetected.MembershipFunctions.AddRectangle("Detected", 0, 1);
            var notDetected = playerDetected.MembershipFunctions.AddRectangle("NotDetected", -1, 0);

            playerDistance = new LinguisticVariable("playerDistance");
            var playerNear = playerDistance.MembershipFunctions.AddTrapezoid("PlayerNear", 0, 10, 25, 30);
            var playerMid = playerDistance.MembershipFunctions.AddTrapezoid("PlayerMid", 20, 30, 40, 50);
            var playerFar = playerDistance.MembershipFunctions.AddTrapezoid("PlayerFar", 40, 50, 60, 70);

            var wallDetection_rule1 = Rule.If(distanceToWall.Is(farDistance)).Then(turnStrength.Is(weak));
            var wallDetection_rule2 = Rule.If(distanceToWall.Is(midDistance)).Then(turnStrength.Is(medium));
            var wallDetection_rule3 = Rule.If(distanceToWall.Is(nearDistance)).Then(turnStrength.Is(strong));

            var playerDetection_rule1 = Rule.If(playerDistance.Is(playerNear).Or(playerDistance.Is(playerMid))).Then(playerDetected.Is(detected));
            var playerDetection_rule2 = Rule.If(playerDistance.Is(playerFar)).Then(playerDetected.Is(notDetected));

            engine_wallDetection.Rules.Add(wallDetection_rule1, wallDetection_rule2, wallDetection_rule3);
            engine_playerDetection.Rules.Add(playerDetection_rule1, playerDetection_rule2);

        }

        public override void OnEnter()
        {
            shark.stateImage.color = Color.clear;
            shark.animator.SetBool("Chasing", false);

            roamTime = 0;
        }

        public override Type Update()
        {
            // send a raycast to the right, if nothing found, send a raycast left
            if (!RaycastForWall(shark.transform.forward + shark.transform.right / 2f, 100f, -1)) 
            {
                RaycastForWall(shark.transform.forward - shark.transform.right / 2f, 100f, 1);
            }

            TickRoamTime();

            if (NeedToChangeDirection()) ChangeDirection();
            MoveInDirection();

            DrawRay();

            if (RaycastHitsBoat()) return typeof(FuzzyChaseState);
            else return typeof(FuzzyRoamState);
        }

        void DrawRay() { Debug.DrawRay(shark.transform.position, direction * 20f, Color.cyan); }

        void MoveInDirection()
        {
            shark.transform.rotation = Quaternion.Lerp(shark.transform.rotation, targetRotation, 0.01f * shark.turnSpeed);
            shark.transform.position += shark.transform.forward * shark.thrustSpeed * Time.deltaTime;
        }
        bool NeedToChangeDirection()// Change direction if a certain amount of time has passed
        {
            if (roamTime > UnityEngine.Random.Range(0.5f, 4.0f)) { ChangeDirection(); roamTime = 0; return true; }
            else return false;
        }
        void TickRoamTime() { roamTime += Time.deltaTime; }
        bool RaycastHitsBoat() 
        {
            // Send a cone of raycasts to see whats ahead

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

        bool RaycastForWall(Vector3 direction, float distance, int side)
        {
            Debug.DrawRay(shark.raycastOrigin.position, direction * distance, Color.white);

            RaycastHit hit;
            if (Physics.Raycast(shark.raycastOrigin.position, direction, out hit, distance)) // send a raycast out
            {
                if (hit.transform.CompareTag("Wall") || hit.transform.CompareTag("Shark"))
                {
                    float obstacleDistance = Vector3.Distance(shark.raycastOrigin.position, hit.point); // if obstacle found, calculate its distance
                    ChangeDirection(obstacleDistance, side); // call the function with the calculated distance and direction that the raycast was sent
                    return true;
                }
                else return false;
            }
            else return false;
        }

        bool Raycast(Vector3 direction, float distance)
        {
            Debug.DrawRay(shark.raycastOrigin.position, direction * distance, Color.green);

            RaycastHit hit;

            if (Physics.Raycast(shark.raycastOrigin.position, direction, out hit, distance) && (hit.transform.CompareTag("Boat")))
            {
                // calculate distance between shark and player
                float playerDist = Vector3.Distance(shark.raycastOrigin.position, hit.point);

                // Use our fuzzy player detection ruleset to determine if the player is near enough to be detected
                double detected = engine_playerDetection.Defuzzify(new { playerDistance = (double)playerDist }); 

                // if it is determined that the player is detected, set the shark's target to the player and return true
                if (detected > 0)
                {
                    shark.target = hit.transform; return true;
                }
                else return false;
            }
            else return false;
        }

        void ChangeDirection()
        {
            // select a random direction between 0 and 1
            int direction = UnityEngine.Random.Range(0, 2);
            float turnStrength = 0;

            // depending on the outcome of the RNG, turn in a direction
            if (direction == 0) { turnStrength = UnityEngine.Random.Range(-1, -60); }
            else if (direction == 1) { turnStrength = UnityEngine.Random.Range(1, 60); }

            // set the target rotation to the determined rotation after the turnStrength is applied
            targetRotation = Quaternion.Euler(shark.transform.eulerAngles.x, shark.transform.eulerAngles.y + turnStrength, shark.transform.eulerAngles.z);
        }

        void ChangeDirection(float distance, int direction)
        {
            // Use our fuzzy wall detection logic to determine how near an obstacle is.
            // The nearer the obstacle, the more the shark turns to avoid it.

            double turnStrength = engine_wallDetection.Defuzzify(new { distanceToWall = (double)distance });

            targetRotation = Quaternion.Euler(shark.transform.eulerAngles.x, shark.transform.eulerAngles.y + (float)turnStrength * direction, shark.transform.eulerAngles.z);
        }
    }
}

