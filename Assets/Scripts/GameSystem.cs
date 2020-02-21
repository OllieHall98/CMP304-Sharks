using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoatChase;

namespace BoatChase
{
    public class GameSystem : MonoBehaviour
    {
        private BaseState state;

        private Dictionary<string, int> numbers;

        void Start()
        {
            ///// Dictionary Example /////

            numbers = new Dictionary<string, int>();

            numbers.Add("Uno", 1);
            numbers.Add("Due", 2);
            numbers.Add("Tre", 3);

            ////////////////////
        }

        void Roam()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}
