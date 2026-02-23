using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace Roguelite
{
    public class RogueliteStageSelectButton : MonoBehaviour, IPointerClickHandler
    {
        private static List<RogueliteStageData> _stageDatas;
        private static List<RogueliteRuntimeSelectData> _tempBuffDatas;
        
        private NextStageData _nextStageData;
        
        private void Awake()
        {
            LoadData();
        }
        
        private void LoadData()
        {
            _stageDatas = new List<RogueliteStageData>(Resources.LoadAll<RogueliteStageData>("Data/Roguelite/SetupData"));
        }

        public async UniTask Setup(RogueliteRuntimeBuffSet buffSet)
        {
            if (_stageDatas == null || _stageDatas.Count == 0)
            {
                LoadData(); 
            }
            
            
            _nextStageData = new NextStageData(
                _stageDatas[Random.Range(0, _stageDatas.Count)],
                buffSet.tempBuffDatas[Random.Range(0, buffSet.tempBuffDatas.Count)]
            );
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            RogueliteSelectManager.Instance.OnStageSelected(_nextStageData);
        }
    }
}