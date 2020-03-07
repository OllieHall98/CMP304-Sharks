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

        [SerializeField] Camera DynamicCamera = null;
        [SerializeField] Camera StaticCamera = null;


        void Start()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;

            UI = this;

            GameObject MenuState = GameObject.Find("StateManager");

            if (MenuState != null)
            {
                if (MenuState.GetComponent<MenuScript>().mode == MenuScript.Mode.FSM) { SwitchToFSM(); }
                else if (MenuState.GetComponent<MenuScript>().mode == MenuScript.Mode.Fuzzy) { SwitchToFuzzyLogic(); }

                Destroy(MenuState);
            }
            else SwitchToFSM();
        }

        public void SwitchCamera()
        {
            if(DynamicCamera.isActiveAndEnabled) 
            { 
                DynamicCamera.enabled = false;
                DynamicCamera.tag = null;

                StaticCamera.enabled = true;
                StaticCamera.tag = "MainCamera";
            }
            else 
            { 
                StaticCamera.enabled = false;
                StaticCamera.tag = null;


                DynamicCamera.enabled = true;
                DynamicCamera.tag = "MainCamera";
            }
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
