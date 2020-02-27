using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BoatChase
{
    public class Monster : MonoBehaviour
    {
        private StateMachine stateMachine = new StateMachine();

        static public RoamStateParameters RoamStateParameters;
        static public ChaseStateParameters ChaseStateParameters;
        static public SearchStateParameters SearchStateParameters;

        [SerializeField] private RoamStateParameters roamStateParameters = null;
        [SerializeField] private ChaseStateParameters chaseStateParameters = null;
        [SerializeField] private SearchStateParameters searchStateParameters = null;

        [SerializeField] public float thrustSpeed = 10f;
        [SerializeField] public Transform raycastOrigin;
        [SerializeField] public Transform target;
        [SerializeField] public Light DetectionLight;

        public enum AITechnique { FiniteStateMachine, FuzzyLogic };
        public static AITechnique AItechnique;

        void Start()
        {
            AItechnique = AITechnique.FiniteStateMachine;

            RoamStateParameters = roamStateParameters;
            ChaseStateParameters = chaseStateParameters;
            SearchStateParameters = searchStateParameters;

            Dictionary<Type, BaseState> states = new Dictionary<Type, BaseState>()
            {
                {typeof(RoamState), new RoamState(this) },
                {typeof(ChaseState), new ChaseState(this) },
                {typeof(SearchState), new SearchState(this) }
            };

            stateMachine.Initialize(states, typeof(RoamState));

        }

        // Update is called once per frame
        void Update()
        {
            if(AItechnique == AITechnique.FiniteStateMachine) { stateMachine.Update(); }
            else if (AItechnique == AITechnique.FuzzyLogic) {  }

        }
    }
}
