using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System;

namespace BoatChase
{
    public class Monster : MonoBehaviour
    {
        private StateMachine stateMachine = new StateMachine();
        private StateMachine fuzzyStateMachine = new StateMachine();

        Dictionary<Type, BaseState> states;
        Dictionary<Type, BaseState> fuzzyStates;

        [HideInInspector] public Image stateImage;

        [Header("Movement Attributes")]
        [SerializeField] public float thrustSpeed = 10f;
        [SerializeField] public float turnSpeed = 2;
        [SerializeField] public float chaseRange = 90f;
        [SerializeField] public float viewDistance = 70f;

        [Header("Other")]
        [SerializeField] public Transform raycastOrigin;
        [HideInInspector] public Transform target;
        [SerializeField] public Light DetectionLight;
        [HideInInspector] public Animator animator;

        public enum AITechnique { FiniteStateMachine, FuzzyLogic };
        public static AITechnique AItechnique;

        void Start()
        {
            stateImage = GetComponentInChildren<Image>();
            animator = GetComponentInChildren<Animator>();

            AItechnique = AITechnique.FiniteStateMachine;

            states = new Dictionary<Type, BaseState>()
            {
                {typeof(RoamState), new RoamState(this) },
                {typeof(ChaseState), new ChaseState(this) },
                {typeof(SearchState), new SearchState(this) }
            };

            fuzzyStates = new Dictionary<Type, BaseState>()
            {
                {typeof(FuzzyRoamState), new FuzzyRoamState(this) },
                {typeof(FuzzyChaseState), new FuzzyChaseState(this) },
                {typeof(FuzzySearchState), new FuzzySearchState(this) }
            };

            initialiseStates();
        }

        // Update is called once per frame
        void Update()
        {
            if(AItechnique == AITechnique.FiniteStateMachine) { stateMachine.Update(); }
            else if (AItechnique == AITechnique.FuzzyLogic) { fuzzyStateMachine.Update(); }

            stateImage.rectTransform.position = Camera.main.WorldToScreenPoint(transform.position);
        }

        public void initialiseStates()
        {
            stateMachine.Initialize(states, typeof(RoamState));
            fuzzyStateMachine.Initialize(fuzzyStates, typeof(FuzzyRoamState));
        }
    }
}
