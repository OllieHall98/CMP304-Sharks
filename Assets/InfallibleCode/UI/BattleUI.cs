﻿using UnityEngine;
using UnityEngine.UI;

namespace Com.InfallibleCode.TurnBasedGame
{
    public class BattleUI : MonoBehaviour
    {
        [SerializeField] private GameObject fighterPrefab;
        [SerializeField] private Transform playerParent;
        [SerializeField] private Transform enemyParent;
        [SerializeField] private Nameplate playerNameplate;
        [SerializeField] private Nameplate enemyNameplate;
        [SerializeField] private Text dialogText;
        [SerializeField] private GameObject PauseScreen;

        public void Initialize(Fighter player, Fighter enemy)
        {
            InitializePlayer(player);
            InitializeEnemy(enemy);
        }

        public void SetDialogText(string text)
        {
            dialogText.text = text;
        }

        public void ShowPauseMenu()
        {
            PauseScreen.SetActive(true);
        }
        
        public void HidePauseMenu()
        {
            PauseScreen.SetActive(false);
        }

        private void InitializePlayer(Fighter fighter)
        {
            playerNameplate.Initialize(fighter);

            var go = Instantiate(fighterPrefab, playerParent, false);
            var image = go.GetComponent<Image>();
            image.color = fighter.Color;
        }
        
        private void InitializeEnemy(Fighter fighter)
        {
            enemyNameplate.Initialize(fighter);
            
            var go = Instantiate(fighterPrefab, enemyParent, false);
            go.transform.localScale = Vector3.one * 0.8f;
            var image = go.GetComponent<Image>();
            image.color = fighter.Color;
        }
    }
}
