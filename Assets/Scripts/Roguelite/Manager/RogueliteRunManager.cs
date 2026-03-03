using System;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ShabuStudio.Gameplay;

namespace Roguelite
{
    public class RogueliteRunManager  : MonoBehaviour
    {
        public static RogueliteRunManager Instance;

        public RogueliteSelectManager selectUI;
        public RogueliteRunState currentRunState;
        
        private UniTaskCompletionSource<bool> _battleFinishSource;
        
        private void Awake() { Instance = this; DontDestroyOnLoad(gameObject); }
        
        public async UniTaskVoid StartRogueliteRun()
        {
            currentRunState = new RogueliteRunState(RogueliteBattleStateManager.Instance.playerUnit);
            int abyssFloor = 1;

            while (currentRunState.CurrentHP > 0)
            {
                //Wait for Select Stage
                NextStageData selection = await selectUI.RunSelectSequence(abyssFloor, currentRunState);
                //Apply Stage Buff
                selection.buffData.ApplyEffect(currentRunState);
                
                var battleFinishSource = new UniTaskCompletionSource<bool>();
                
                RogueliteBattleStateManager.Instance.StartInitialize(abyssFloor,selection.stageSetupData,battleFinishSource).Forget();
                
                bool isWin = await battleFinishSource.Task;
                
                if(isWin)
                {
                    Destroy(RogueliteBattleStateManager.Instance.enemyUnit.gameObject);
                    abyssFloor++;
                }
                else
                {
                    SceneLoader.LoadSceneWithTransitionCanvas("Scene_MainMenu",SceneLoader.Instance.defaultTransitionCanvas);
                    break;
                }
            }
        }
    }
    
    [System.Serializable]
    public class RogueliteRunState
    {
        [Header("Run State Properties")]
        public int CurrentHP;
        public int CurrentDeckIndex;
        public CombatEntity currentPlayerEntity;
        public RogueliteStageData CurrentStage;

        public RogueliteRunState(CombatEntity playerEntity)
        {
            currentPlayerEntity = playerEntity;
            CurrentHP = playerEntity.Stats.MaxHealth;
        }
    }
}