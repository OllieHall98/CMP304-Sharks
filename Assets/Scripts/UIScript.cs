using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace BoatChase
{
    public class UIScript : MonoBehaviour
    {
        public static UIScript UI;
        [SerializeField] Text stateText = null;
        [SerializeField] Button FSMButton = null;
        [SerializeField] Button FuzzyButton = null;

        void Start()
        {
            UI = this;

            SwitchToFSM();
        }

        public void SwitchToFSM()
        {
            FSMButton.interactable = false;
            FuzzyButton.interactable = true;
            Monster.AItechnique = Monster.AITechnique.FiniteStateMachine;
        }

        public void SwitchToFuzzyLogic()
        {
            FSMButton.interactable = true;
            FuzzyButton.interactable = false;
            Monster.AItechnique = Monster.AITechnique.FuzzyLogic;
        }

        public void setStateText(string state)
        {
            stateText.text = "State: " + state;
        }
    }
}
