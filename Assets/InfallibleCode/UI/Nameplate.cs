using UnityEngine;
using UnityEngine.UI;

namespace Com.InfallibleCode.TurnBasedGame
{
    public class Nameplate : MonoBehaviour
    {
        [SerializeField] private Text nameLabel;
        [SerializeField] private Text levelLabel;
        [SerializeField] private Image fillImage;

        private Fighter _fighter;

        public void Initialize(Fighter fighter)
        {
            _fighter = fighter;
            nameLabel.text = _fighter.Name;
            levelLabel.text = _fighter.Level.ToString();
        }

        public void Update()
        {
            UpdateHealthBar();
        }
        
        private void UpdateHealthBar()
        {
            if (_fighter.CurrentHealth == 0)
            {
                fillImage.fillAmount = 0;
            }
            else
            {
                fillImage.fillAmount = (float) _fighter.CurrentHealth / _fighter.TotalHealth;
            }
        }
    }
}