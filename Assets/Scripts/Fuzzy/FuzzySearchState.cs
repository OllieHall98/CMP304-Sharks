using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FLS;
using FLS.Rules;
using FLS.MembershipFunctions;

namespace BoatChase
{
    public class FuzzySearchState : FuzzyBaseState
    {
        protected IFuzzyEngine engine_findPlayer;
        protected IFuzzyEngine engine_wallDetection;

        private LinguisticVariable playerDist;
        private LinguisticVariable chanceToFindPlayer;
        private LinguisticVariable distanceToWall;
        private LinguisticVariable turnStrength;

        Quaternion targetRotation;

        public float searchSpeedMultiplier = 1.5f;
        float SearchTime = 0;

        public FuzzySearchState(Monster pawn) : base(pawn)
        {
            engine_findPlayer = new FuzzyEngineFactory().Default();
            engine_wallDetection = new FuzzyEngineFactory().Default();

            distanceToWall = new LinguisticVariable("distanceToWall");
            var farDistance = distanceToWall.MembershipFunctions.AddTrapezoid("WallFar", 70, 70, 90, 120);
            var midDistance = distanceToWall.MembershipFunctions.AddTrapezoid("WallMid", 50, 60, 70, 80);
            var nearDistance = distanceToWall.MembershipFunctions.AddTrapezoid("WallNear", 0, 20, 50, 70);

            turnStrength = new LinguisticVariable("turnStrength");
            var weakTurn = turnStrength.MembershipFunctions.AddTrapezoid("Weak", 1, 4, 8, 12);
            var mediumTurn = turnStrength.MembershipFunctions.AddTrapezoid("Medium", 6, 12, 18, 24);
            var strongTurn = turnStrength.MembershipFunctions.AddTrapezoid("Strong", 15, 30, 60, 80);

            playerDist = new LinguisticVariable("playerDist");
            var near = playerDist.MembershipFunctions.AddTrapezoid("Near", 0, 10, 25, 40);
            var mid = playerDist.MembershipFunctions.AddTrapezoid("Mid", 25, 40, 55, 70);
            var far = playerDist.MembershipFunctions.AddTrapezoid("Far", 55, 70, 85, 100);

            chanceToFindPlayer = new LinguisticVariable("chanceToFindPlayer");
            var high = chanceToFindPlayer.MembershipFunctions.AddRectangle("High", 6, 9);
            var medium = chanceToFindPlayer.MembershipFunctions.AddRectangle("Medium", 3, 6);
            var low = chanceToFindPlayer.MembershipFunctions.AddRectangle("Low", 0, 3);

            var findPlayer_rule1 = Rule.If(playerDist.Is(near)).Then(chanceToFindPlayer.Is(high));
            var findPlayer_rule2 = Rule.If(playerDist.Is(mid)).Then(chanceToFindPlayer.Is(medium));
            var findPlayer_rule3 = Rule.If(playerDist.Is(far)).Then(chanceToFindPlayer.Is(low));

            var wallDetection_rule1 = Rule.If(distanceToWall.Is(farDistance)).Then(turnStrength.Is(weakTurn));
            var wallDetection_rule2 = Rule.If(distanceToWall.Is(midDistance)).Then(turnStrength.Is(mediumTurn));
            var wallDetection_rule3 = Rule.If(distanceToWall.Is(nearDistance)).Then(turnStrength.Is(strongTurn));

            engine_findPlayer.Rules.Add(findPlayer_rule1, findPlayer_rule2, findPlayer_rule3);
            engine_wallDetection.Rules.Add(wallDetection_rule1, wallDetection_rule2, wallDetection_rule3);
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

            // send a raycast to the right, if nothing found, send a raycast left
            if (!RaycastForWall(shark.transform.forward + shark.transform.right / 2f, 100f, -1))
            {
                RaycastForWall(shark.transform.forward - shark.transform.right / 2f, 100f, 1);
            }

            MoveInDirection();

            if (SearchTime >= 4.0f) { return typeof(FuzzyRoamState); }

            if (RaycastHitsBoat()) { return typeof(FuzzyChaseState); }
            else { return typeof(FuzzySearchState); }
        }

        void MoveInDirection()
        {
            shark.transform.rotation = Quaternion.Lerp(shark.transform.rotation, targetRotation, shark.turnSpeed * Time.deltaTime);
            shark.transform.position += shark.transform.forward * shark.thrustSpeed * searchSpeedMultiplier * Time.deltaTime;
        }

        bool RaycastForWall(Vector3 direction, float distance, int side)
        {
            Debug.DrawRay(shark.raycastOrigin.position, direction * distance, Color.white);

            RaycastHit hit;
            if (Physics.Raycast(shark.raycastOrigin.position, direction, out hit, distance))
            {
                if (hit.transform.CompareTag("Wall") || hit.transform.CompareTag("Shark")) // send a raycast out
                {
                    float wallDistance = Vector3.Distance(shark.raycastOrigin.position, hit.point); // if obstacle found, calculate its distance
                    ChangeDirection(wallDistance, side);// call the function with the calculated distance and direction that the raycast was sent
                    return true;
                }
                else return false;
            }
            else return false;
        }

        void ChangeDirection(float distance, int direction)
        {
            // Use our fuzzy wall detection logic to determine how near an obstacle is.
            // The nearer the obstacle, the more the shark turns to avoid it.

            double turnStrength = engine_wallDetection.Defuzzify(new { distanceToWall = (double)distance });

            targetRotation = Quaternion.Euler(shark.transform.eulerAngles.x, shark.transform.eulerAngles.y + (float)turnStrength * direction, shark.transform.eulerAngles.z);
        }

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

        bool Raycast(Vector3 direction, float distance)
        {
            Debug.DrawRay(shark.raycastOrigin.position, direction * distance, Color.yellow);

            RaycastHit hit;
            if (Physics.Raycast(shark.raycastOrigin.position, direction, out hit, distance) && hit.transform.CompareTag("Boat")) // if a raycast hits a boat
            {
                // calculate distance between shark and player
                float playerDistance = Vector3.Distance(shark.raycastOrigin.position, hit.point);

                // Use our fuzzy player detection ruleset to determine if the player is near enough to be detected
                double detectionChance = engine_findPlayer.Defuzzify(new { playerDist = (double)playerDistance });

                // if it is determined that the player is detected, set the shark's target to the player and return true
                if (detectionChance > 5)
                {
                    shark.target = hit.transform;
                    return true;
                }
                else return false;
            }
            else return false;
        }
    }
}

