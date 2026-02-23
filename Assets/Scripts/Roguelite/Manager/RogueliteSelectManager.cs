using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Roguelite
{
    public class RogueliteSelectManager : MonoBehaviour
    {
        public static RogueliteSelectManager Instance;
        
        public RogueliteRuntimeBuffSet buffSet1;
        public RogueliteRuntimeBuffSet buffSet2;
        public RogueliteRuntimeBuffSet buffSet3;
        public RogueliteRuntimeBuffSet buffSet4;
        public RogueliteRuntimeBuffSet buffSet5;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
            
            selectPanel.SetActive(false);
        }
        

        
        [SerializeField]private List<RogueliteStageSelectButton> stageButtons;
        
        public GameObject selectPanel;
        private UniTaskCompletionSource<NextStageData> _selectionSource;

        public async UniTask<NextStageData> RunSelectSequence(int stageIndex,RogueliteRunState runState)
        {
            RogueliteRuntimeBuffSet setHolder;

            switch (stageIndex)
            {
                case >=0 and <= 10: setHolder = buffSet1; break;
                case >=11 and <= 20: setHolder = buffSet2; break;
                case >=21 and <= 30: setHolder = buffSet3; break;
                case >=31 and <= 40: setHolder = buffSet4; break;
                case >=41 and <= 50: setHolder = buffSet5; break;
                default: throw new ArgumentOutOfRangeException(nameof(stageIndex), stageIndex, null);
            }
            
            //Setup all Button
            List<UniTask> tasks = new List<UniTask>();
            foreach (var button in stageButtons)
            {
                UniTask task = button.Setup(setHolder);
                tasks.Add(task);
            }
            
            await UniTask.WhenAll(tasks);
            
            selectPanel.SetActive(true);
            
            _selectionSource = new UniTaskCompletionSource<NextStageData>();
            
            NextStageData result = await _selectionSource.Task;
            
            
            selectPanel.SetActive(false);
            return result;
        }
        
        
        public void OnStageSelected(NextStageData data)
        { 
            _selectionSource.TrySetResult(data);
        }
    }
    
    public struct NextStageData 
    {
        public RogueliteRuntimeSelectData buffData;
        public RogueliteStageData stageSetupData;
        
        public NextStageData(RogueliteStageData stageSetupData,RogueliteRuntimeSelectData buffData)
        {
            this.buffData = buffData;
            this.stageSetupData = stageSetupData;
        }
    }
}