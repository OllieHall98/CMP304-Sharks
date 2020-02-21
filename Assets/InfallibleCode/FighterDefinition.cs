using System;
using UnityEngine;

namespace Com.InfallibleCode.TurnBasedGame
{
    public class FighterDefinition : ScriptableObject
    {
        public const int MaxHealth = 100;
        
        public string Name;
        public int Level;
        public int Health;

        private void OnValidate()
        {
            Health = Math.Min(MaxHealth, Health);
        }
    }
}