using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShabuStudio.Gameplay
{
    public class BattleStateManager : MonoBehaviour
    {
        public static BattleStateManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
        [Header("Scene Index")]
        public int winSceneIndex;
        public int loseSceneIndex;
        
        [Header("State Info")]
        private BattleState currentState;
        
        [Header("References")]
        public CombatEntity playerUnit;
        public EnemyCombatEntity enemyUnit;
        [SerializeField] private SetupManager setupManager;
        [SerializeField] private ActionBar actionBar;
        [SerializeField] private ActionManager actionManager;
        [SerializeField] private HandManager handManager;
        [SerializeField] private DeckManager deckManager;
        [SerializeField] private CombatManager combatManager;
        [SerializeField] private DamageTextManager damageTextManager;


        private void Start()
        {
            StartCoroutine(setupManager.StartSetup(GameManager.Instance.currentStageData));
            Invoke("StartInitialize", 0.1f);
        }

        void StartInitialize()
        {
            ChangeState(BattleState.Initialize);
        }

        public void ChangeState(BattleState newState)
        {
            currentState = newState;
            Debug.Log($"State Changed to {newState}");

            switch (newState)
            {
                case  BattleState.Initialize:
                    HandleInitialize();
                    break;
                case BattleState.Start:
                    StartCoroutine(HandleStart());
                    break;
                case BattleState.DrawPhase:
                    StartCoroutine(HandleDrawPhase());
                    break;
                case BattleState.ActionSetupPhase:
                    HandleActionSetupPhase();
                    break;
                case BattleState.ActionPhase:
                    StartCoroutine(HandleActionPhase());
                    break;
                case BattleState.EndPhase:
                    HandleEndPhase();
                    break;
                case BattleState.Win:
                    HandleWin();
                    break;
            }
        }

        

        void HandleInitialize()
        {
            actionBar.Initialize();
            deckManager.Initialize();
            combatManager.Initialize(damageTextManager);
            handManager.Initialize(deckManager);
            actionManager.Initialize(actionBar,combatManager);
            damageTextManager.Initialize();
            
            
            UpdateAllUI();
            ChangeState(BattleState.Start);
        }

        IEnumerator HandleStart()
        {
            playerUnit.AddCost(3);
            enemyUnit.AddCost(3);
            
            //OnStartTurnEffect
            combatManager.OnStartTurn();
            handManager.HideHand(false); //Hide hand after 1 second ( 1 second = 1 frame)
            
            UpdateAllUI();
            yield return null;
            ChangeState(BattleState.DrawPhase);
        }
        
        IEnumerator HandleDrawPhase()
        {
            yield return StartCoroutine(handManager.DrawCardToMax()); //Wait Until Player draw all card.
            ChangeState(BattleState.ActionSetupPhase);
        }

        void HandleActionSetupPhase()
        {
            StartCoroutine(enemyUnit.StartActionSetup(actionBar));
            actionManager.playButton.gameObject.SetActive(true);
            handManager.SetHandCardInteractable(true);
        }

        IEnumerator HandleActionPhase()
        {
            handManager.HideHand(true);
            yield return StartCoroutine(actionManager.StartActionSequence());
            ChangeState(BattleState.EndPhase);
        }

        void HandleEndPhase()
        {
            if (enemyUnit.isDead)
            {
                ChangeState(BattleState.Win);
                return;
            }
            
            if (playerUnit.isDead)
            {
                ChangeState(BattleState.Lose);
                return;
            }
            
            playerUnit.DecreaseBuffTurn(1);
            enemyUnit.DecreaseBuffTurn(1);
            
            ChangeState(BattleState.Start);
        }
        
        private void HandleWin()
        {
            SceneManager.LoadScene(winSceneIndex);
        }

        public void StartActionSequence()
        {
            ChangeState(BattleState.ActionPhase);
            actionManager.playButton.gameObject.SetActive(false);
            handManager.SetHandCardInteractable(false);
            handManager.RemovePlayedCard();
        }

        
        //Update Every UI In the game
        void UpdateAllUI()
        {
            playerUnit.UpdateUI();
            enemyUnit.UpdateUI();
            actionBar.UpdateUI();
        }
    }

    public enum BattleState
    {
        Initialize, //First Time Scene load setup
        Start, //Setup object, Reduce cooldown, Apply any start turn effect
        DrawPhase,
        ActionSetupPhase,
        ActionPhase,
        EndPhase,
        Win,
        Lose
    }
    
}