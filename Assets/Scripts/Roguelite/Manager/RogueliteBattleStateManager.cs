using Cysharp.Threading.Tasks;
using ShabuStudio;
using ShabuStudio.Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Roguelite
{
    public class RogueliteBattleStateManager : MonoBehaviour
    {
        public static RogueliteBattleStateManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
        
        [Header("State Info")]
        private BattleState currentState;
        
        [Header("References")]
        public CombatEntity playerUnit;
        public EnemyCombatEntity enemyUnit;
        [SerializeField] private RogueliteSetupManager setupManager;
        [SerializeField] private RogueliteActionBar actionBar;
        [SerializeField] private RogueliteActionManager actionManager;
        [SerializeField] private RogueliteHandManager handManager;
        [SerializeField] private RogueliteDeckManager deckManager;
        [SerializeField] private RogueliteCombatManager combatManager;
        [SerializeField] private DamageTextManager damageTextManager;
        
        private UniTaskCompletionSource<bool> _activeBattleSource;


        private async void Start()
        {
            RogueliteRunManager.Instance.StartRogueliteRun().Forget();
        }

        public async UniTaskVoid StartInitialize(int stageIndex,RogueliteStageData stageData,UniTaskCompletionSource<bool> source)
        {
            _activeBattleSource = source;
            await setupManager.StartSetup(stageData,stageIndex);
            ChangeState(BattleState.Initialize).Forget();
        }

        public async UniTaskVoid ChangeState(BattleState newState)
        {
            currentState = newState;
            Debug.Log($"State Changed to {newState}");

            switch (newState)
            {
                case BattleState.Initialize:
                    HandleInitialize();
                    break;
                case BattleState.Start:
                    await HandleStart();
                    break;
                case BattleState.DrawPhase:
                    await HandleDrawPhase();
                    break;
                case BattleState.ActionSetupPhase:
                    HandleActionSetupPhase();
                    break;
                case BattleState.ActionPhase:
                    await HandleActionPhase();
                    break;
                case BattleState.EndPhase:
                    HandleEndPhase();
                    break;
                case BattleState.Win:
                    HandleWin();
                    break;
                case BattleState.Lose:
                    HandleLose();
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

        async UniTask HandleStart()
        {
            playerUnit.AddCost(5);
            enemyUnit.SetCost(enemyUnit.fixedCost);
            
            //OnStartTurnEffect
            await combatManager.OnStartTurn();
            handManager.HideHand(false); //Hide hand after 1 second (1 second = 1 frame)
            
            UpdateAllUI();
            if (enemyUnit.isDead)
            {
                ChangeState(BattleState.Win).Forget();
                return;
            }

            if (playerUnit.isDead)
            {
                ChangeState(BattleState.Lose).Forget();
                return;
            }
            await UniTask.NextFrame();
            ChangeState(BattleState.DrawPhase).Forget();
        }
        
        async UniTask HandleDrawPhase()
        {
            await handManager.DrawCardToMax(); //Wait Until Player draw all card.
            ChangeState(BattleState.ActionSetupPhase).Forget();
        }

        void HandleActionSetupPhase()
        {
            enemyUnit.StartActionSetup(actionBar).Forget();
            actionManager.playButton.gameObject.SetActive(true);
            handManager.SetHandCardInteractable(true);
        }

        async UniTask HandleActionPhase()
        {
            handManager.HideHand(true);
            await actionManager.StartActionSequence();
            ChangeState(BattleState.EndPhase).Forget();
        }

        void HandleEndPhase()
        {
            if (enemyUnit.isDead)
            {
                ChangeState(BattleState.Win).Forget();
                return;
            }
            
            if (playerUnit.isDead)
            {
                ChangeState(BattleState.Lose).Forget();
                return;
            }
            
            playerUnit.OnTurnEnd();
            enemyUnit.OnTurnEnd();
            
            ChangeState(BattleState.Start).Forget();
        }
        
        private void HandleWin()
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            
            playerUnit.OnTurnEnd();
            
            _activeBattleSource?.TrySetResult(true);
        }
        
        private void HandleLose()
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            
            _activeBattleSource?.TrySetResult(false);
        }

        public void StartActionSequence()
        {
            ChangeState(BattleState.ActionPhase).Forget();
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
    
}