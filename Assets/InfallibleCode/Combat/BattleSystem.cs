using System.Collections;
using UnityEngine;

namespace Com.InfallibleCode.TurnBasedGame.Combat
{
    public class BattleSystem : MonoBehaviour
    {
        private BattleState _state;
        
        [SerializeField] private BattleUI ui;
        [SerializeField] private Fighter player;
        [SerializeField] private Fighter enemy;
        
        public Fighter Player => player;
        public Fighter Enemy => enemy;
        public BattleUI Interface => ui;

        private void Start()
        {
            Interface.Initialize(player, enemy);

            _state = BattleState.Beginning;
            StartCoroutine(BeginBattle());
        }

        public void OnAttackButton()
        {
            if (_state != BattleState.PlayerTurn) return;
            StartCoroutine(PlayerAttack());
        }

        public void OnHealButton()
        {
            if (_state != BattleState.PlayerTurn) return;
            StartCoroutine(PlayerHeal());
        }

        private IEnumerator BeginBattle()
        {
            Interface.SetDialogText($"A wild {Enemy.Name} appeared!");
            
            yield return new WaitForSeconds(2f);

            _state = BattleState.PlayerTurn;
            StartCoroutine(PlayerTurn());
        }

        private IEnumerator PlayerTurn()
        {
            Interface.SetDialogText("Choose an action.");
            yield break;
        }

        private IEnumerator PlayerAttack()
        {
            var isDead = Enemy.Damage(Player.Attack);
        
            yield return new WaitForSeconds(1f);

            if (isDead)
            {
                _state = BattleState.Won;
                StartCoroutine(EndGame());
            }
            else
            {
                _state = BattleState.EnemyTurn;
                StartCoroutine(EnemyTurn());
            }
        }

        private IEnumerator PlayerHeal()
        {
            Interface.SetDialogText($"{Player.Name} feels renewed strength!");
            
            Player.Heal(5);
        
            yield return new WaitForSeconds(1f);

            _state = BattleState.EnemyTurn;
            StartCoroutine(EnemyTurn());
        }

        private IEnumerator EnemyTurn()
        {
            Interface.SetDialogText($"{Enemy.Name} attacks!");
        
            var isDead = Player.Damage(Enemy.Attack);

            yield return new WaitForSeconds(1f);

            if (isDead)
            {
                _state = BattleState.Lost;
                StartCoroutine(EndGame());
            }
            else
            {
                _state = BattleState.PlayerTurn;
                StartCoroutine(PlayerTurn());
            }
        }

        private IEnumerator EndGame()
        {
            switch (_state)
            {
                case BattleState.Won:
                    Interface.SetDialogText("You won the battle!");
                    break;
                case BattleState.Lost:
                    Interface.SetDialogText("You were defeated.");
                    break;
                default:
                    Interface.SetDialogText("The match was a stalemate!");
                    break;
            }
            yield break;
        }
    }
}