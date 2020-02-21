using System;
using UnityEngine;

namespace Com.InfallibleCode.TurnBasedGame
{
    [CreateAssetMenu]
    public class Fighter : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private int _level;
        [SerializeField] private Color _color;
        [SerializeField] private int _currentHealth;
        [SerializeField] private int _totalHealth;
        [SerializeField] private int _attack;
        [SerializeField] private int _healing;

        public string Name => _name;
        public int Level => _level;
        public Color Color => _color;
        public int CurrentHealth => _currentHealth;
        public int TotalHealth => _totalHealth;
        public int Attack => _attack;
        public int Healing => _healing;

        public bool Damage(int amount)
        {
            _currentHealth = Math.Max(0, _currentHealth - amount);
            return _currentHealth == 0;
        }

        public void Heal(int amount)
        {
            _currentHealth += amount;
        }

        private void OnValidate()
        {
            _currentHealth = Math.Min(_currentHealth, _totalHealth);
        }
    }
}