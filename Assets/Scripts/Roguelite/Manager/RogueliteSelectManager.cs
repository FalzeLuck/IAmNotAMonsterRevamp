using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Roguelite
{
    public class RogueliteSelectManager : MonoBehaviour
    {
        public static RogueliteSelectManager Instance;

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
            
            //Setup all Button
            List<UniTask> tasks = new List<UniTask>();
            foreach (var button in stageButtons)
            {
                UniTask task = button.Setup();
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