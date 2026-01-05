using Cysharp.Threading.Tasks;
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


        private async void Start()
        {
            await setupManager.StartSetup(GameManager.Instance.currentStageData);
            StartInitialize();
        }

        void StartInitialize()
        {
            ChangeState(BattleState.Initialize).Forget();
        }

        public async UniTaskVoid ChangeState(BattleState newState)
        {
            currentState = newState;
            Debug.Log($"State Changed to {newState}");

            switch (newState)
            {
                case  BattleState.Initialize:
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
            playerUnit.AddCost(3);
            enemyUnit.SetCost(enemyUnit.fixedCost);
            
            //OnStartTurnEffect
            await combatManager.OnStartTurn();
            handManager.HideHand(false); //Hide hand after 1 second (1 second = 1 frame)
            
            UpdateAllUI();
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
            SceneManager.LoadScene(winSceneIndex);
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