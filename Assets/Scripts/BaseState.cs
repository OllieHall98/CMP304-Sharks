using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace BoatChase
{

    public abstract class BaseState
    {
        protected Monster monster;
        
        public BaseState(Monster pawn)
        {
            monster = pawn;
        }

        public abstract Type Update();
        public abstract void OnEnter();
    }

    public class RoamState : BaseState
    {
        Rigidbody rigidbody;
        GameObject boat;
        Vector3 boatPosition;

        Vector3 destination = Vector3.zero;
        Vector3 direction   = Vector3.zero;
        Quaternion targetRotation;

        float roamTime = 0;

        public RoamState(Monster pawn) : base(pawn) 
        {
            rigidbody = monster.GetComponent<Rigidbody>();
            boat = GameObject.FindGameObjectWithTag("Boat");
            
        }

        public override void OnEnter()
        {
            Debug.Log(monster + " has started roaming");

            monster.DetectionLight.color = new Color(1, 1, 1);

            roamTime = 0;

        }

        public override Type Update()
        {
            boatPosition = boat.transform.position;

            TickRoamTime();
            if (NeedToChangeDirection()) { ChangeDirection(); }
            DrawRay();
            MoveInDirection();

            if (RaycastHitsBoat()) { return typeof(ChaseState); }
            else { return typeof(RoamState) ; }

        }

        void DrawRay(){ Debug.DrawRay(monster.transform.position, direction * 20f, Color.cyan); }


        void MoveInDirection()
        {
            monster.transform.rotation = Quaternion.Lerp(monster.transform.rotation, targetRotation, 0.005f);
            rigidbody.AddForce(monster.transform.forward * Time.deltaTime * monster.thrustSpeed * 100);
        }
        bool NeedToChangeDirection()
        {
            if (destination == Vector3.zero) { ChangeDirection(); return true; }
            if (roamTime > UnityEngine.Random.Range(0.8f, 4.0f)){ roamTime = 0; return true; }
            return false;
        }
        void TickRoamTime() { roamTime += Time.deltaTime; }
        bool RaycastHitsBoat()
        {
            bool canSeePlayer = false;

            if (
                  //(Vector2.Distance(monster.transform.position, boatPosition) < 35)                                                     ||
                  (Raycast(monster.transform.forward - monster.transform.right * 2, Monster.RoamStateParameters.coneOfVision * 0.4f))   ||
                  (Raycast(monster.transform.forward - monster.transform.right, Monster.RoamStateParameters.coneOfVision * 0.6f))       ||
                  (Raycast(monster.transform.forward - (monster.transform.right / 2), Monster.RoamStateParameters.coneOfVision * 0.8f)) ||
                  (Raycast(monster.transform.forward, Monster.RoamStateParameters.coneOfVision))                                        ||
                  (Raycast(monster.transform.forward + (monster.transform.right / 2), Monster.RoamStateParameters.coneOfVision * 0.8f)) ||
                  (Raycast(monster.transform.forward + (monster.transform.right / 2), Monster.RoamStateParameters.coneOfVision * 0.8f)) ||
                  (Raycast(monster.transform.forward + monster.transform.right, Monster.RoamStateParameters.coneOfVision * 0.6f))       ||
                  (Raycast(monster.transform.forward + monster.transform.right * 2, Monster.RoamStateParameters.coneOfVision * 0.4f))
               )
            { canSeePlayer = true; }

            return canSeePlayer;
        }
        bool Raycast(Vector3 direction, float distance)
        {
            RaycastHit hit;
            if (Physics.Raycast(monster.raycastOrigin.position, direction, out hit, distance))
            {
                if (hit.transform.tag == "Boat")
                {
                    Debug.DrawRay(monster.raycastOrigin.position, direction * distance, Color.red);
                    monster.target = hit.transform;
                    return true;
                }
                else
                {
                    ChangeDirection(-hit.transform.position); // if finds wall
                    return false;
                }
            }
            else
            {
                Debug.DrawRay(monster.raycastOrigin.position, direction * distance, Color.white);
                return false;
            }
        }
        void ChangeDirection()
        {
            Vector3 testPosition = (monster.transform.position + new Vector3(UnityEngine.Random.Range(-5.0f, 5.0f), 0f, UnityEngine.Random.Range(-5.0f, 5.0f)));

            destination = new Vector3(testPosition.x, 1f, testPosition.z);
            direction = Vector3.Normalize(destination - monster.transform.position);
            direction = new Vector3(direction.x, 0f, direction.z);
            targetRotation = Quaternion.LookRotation(direction);
        }

        void ChangeDirection(Vector3 oppositeDirection)
        {
            destination = new Vector3(oppositeDirection.x, 1f, oppositeDirection.z);
            direction = Vector3.Normalize(destination - monster.transform.position);
            direction = new Vector3(direction.x, 0f, direction.z);
            targetRotation = Quaternion.LookRotation(direction);
        }
    }

    public class ChaseState : BaseState
    {
        private Rigidbody rigidbody;

        public ChaseState(Monster pawn) : base(pawn) 
        {
            rigidbody = monster.GetComponent<Rigidbody>();
        }

        public override void OnEnter()
        {
            Debug.Log(monster + " has started chasing the player");

            monster.DetectionLight.color = new Color(1, 0, 0);
        }

        public override Type Update()
        {
            if(monster.target == null) { return typeof(RoamState); }

            chaseBoat();

            if(boatEscaped()) { return typeof(SearchState); }
            else { return typeof(ChaseState); }
        }

        void chaseBoat()
        {
            Debug.DrawRay(monster.transform.position, monster.target.position - monster.transform.position, Color.red);

            Quaternion targetRotation = Quaternion.LookRotation(monster.target.position - monster.transform.position);
            targetRotation.x = 0;
            targetRotation.z = 0;

            monster.transform.rotation = Quaternion.Lerp(monster.transform.rotation, targetRotation, 0.1f);
            rigidbody.AddForce(monster.transform.forward * Time.deltaTime * monster.thrustSpeed * (100 * Monster.ChaseStateParameters.chaseSpeedMultiplier));
        }

        bool boatEscaped()
        {
            if (Vector2.Distance(monster.transform.position, monster.target.position) > Monster.ChaseStateParameters.chaseRange) 
            {
                return true; 
            }
            else { return false; }
        }
    }

    public class SearchState : BaseState
    {
        private Rigidbody rigidbody;
        private Transform LastPlayerLocation;
        private Transform SearchTarget;

        float PlayerTrackerTimer = 0;
        float PlayerTrackClockRate = 1.0f;

        float SearchTime = 0;

        public SearchState(Monster pawn) : base(pawn) 
        {
            rigidbody = monster.GetComponent<Rigidbody>();
        }

        public override void OnEnter()
        {
            Debug.Log(monster + " is searching for the player");

            LastPlayerLocation = monster.target;
            SearchTarget = monster.target;
            PlayerTrackerTimer = 0;
            SearchTime = 0;

            monster.DetectionLight.color = new Color(1, 0.8f, 0);

        }

        public override Type Update()
        {
            PlayerTrackerTimer += Time.deltaTime;
            SearchTime += Time.deltaTime;

            TrackPlayer();
            GoToLastPlayerLocation();

            if(SearchTime >= 4.0f) { return typeof(RoamState); }

            if (RaycastHitsBoat()) { return typeof(ChaseState); }
            else { return typeof(SearchState); }
        }

        void GoToLastPlayerLocation()
        {
            Debug.DrawRay(monster.transform.position, SearchTarget.position - monster.transform.position, Color.yellow);

            Quaternion targetRotation = Quaternion.LookRotation(SearchTarget.position - monster.transform.position);
            targetRotation.x = 0;
            targetRotation.z = 0;

            monster.transform.rotation = Quaternion.Lerp(monster.transform.rotation, targetRotation, 0.1f);
            rigidbody.AddForce(monster.transform.forward * Time.deltaTime * monster.thrustSpeed * (100 * Monster.SearchStateParameters.searchSpeedMultiplier));
        }

        void TrackPlayer()
        {
            if(PlayerTrackerTimer > PlayerTrackClockRate)
            {
                SearchTarget = LastPlayerLocation;
                LastPlayerLocation = monster.target;


                PlayerTrackerTimer = 0;

                
            } 
        }

        bool RaycastHitsBoat()
        {
            bool canSeePlayer = false;

            if (
                  //(Vector2.Distance(monster.transform.position, SearchTarget.position) < 35)                                            ||
                  (Raycast(monster.transform.forward - monster.transform.right * 2, Monster.RoamStateParameters.coneOfVision * 0.2f))   ||
                  (Raycast(monster.transform.forward - monster.transform.right, Monster.RoamStateParameters.coneOfVision * 0.4f))       ||
                  (Raycast(monster.transform.forward - (monster.transform.right / 2), Monster.RoamStateParameters.coneOfVision * 0.6f)) ||
                  (Raycast(monster.transform.forward, Monster.RoamStateParameters.coneOfVision * 0.8f))                                 ||
                  (Raycast(monster.transform.forward + (monster.transform.right / 2), Monster.RoamStateParameters.coneOfVision * 0.6f)) ||
                  (Raycast(monster.transform.forward + (monster.transform.right / 2), Monster.RoamStateParameters.coneOfVision * 0.4f)) ||
                  (Raycast(monster.transform.forward + monster.transform.right, Monster.RoamStateParameters.coneOfVision * 0.2f))       ||
                  (Raycast(monster.transform.forward + monster.transform.right * 2, Monster.RoamStateParameters.coneOfVision * 0.2f))
               )
            { canSeePlayer = true; }

            return canSeePlayer;
        }

        bool Raycast(Vector3 direction, float distance)
        {
            RaycastHit hit;
            if (Physics.Raycast(monster.raycastOrigin.position, direction, out hit, distance))
            {
                if (hit.transform.tag == "Boat")
                {
                    Debug.DrawRay(monster.raycastOrigin.position, direction * distance, Color.red);
                    monster.target = hit.transform;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Debug.DrawRay(monster.raycastOrigin.position, direction * distance, Color.white);
                return false;
            }
        }
    }

    [CreateAssetMenu(fileName = "Roam State Parameters", menuName = "State Parameters/Roam State Parameters", order = 0)]
    public class RoamStateParameters : ScriptableObject 
    {
        public float coneOfVision = 20f;
    }

    [CreateAssetMenu(fileName = "Chase State Parameters", menuName = "State Parameters/Chase State Parameters", order = 1)]
    public class ChaseStateParameters : ScriptableObject
    {
        public float chaseSpeedMultiplier = 3.0f;
        public float chaseRange = 50f;
    }

    [CreateAssetMenu(fileName = "Search State Parameters", menuName = "State Parameters/Search State Parameters", order = 1)]
    public class SearchStateParameters : ScriptableObject
    {
        public float searchSpeedMultiplier = 1.5f;
    }


}
