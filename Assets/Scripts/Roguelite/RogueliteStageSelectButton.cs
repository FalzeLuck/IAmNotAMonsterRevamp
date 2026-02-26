using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Roguelite
{
    public class RogueliteStageSelectButton : MonoBehaviour, IPointerClickHandler
    {
        private static List<RogueliteStageData> _stageDatas;
        private static List<RogueliteRuntimeSelectData> _tempBuffDatas;
        
        private NextStageData _nextStageData;
        
        [Header("UI")]
        public TextMeshProUGUI buffText;
        public Image backgroundImage;
        public Image enemyImage;
        
        private void Awake()
        {
            LoadData();
        }
        
        private void LoadData()
        {
            _stageDatas = new List<RogueliteStageData>(Resources.LoadAll<RogueliteStageData>("Data/Roguelite/SetupData"));
        }

        public async UniTask Setup(RogueliteRuntimeBuffSet buffSet,bool isBoss = false)
        {
            if (_stageDatas == null || _stageDatas.Count == 0)
            {
                LoadData(); 
            }
            
            List<RogueliteStageData> tempStageDatas = new List<RogueliteStageData>(_stageDatas);
            if (!isBoss)
            {
                tempStageDatas.RemoveAll(x => x.stageType == RogueliteStageData.StageType.Boss);
            }
            else
            {
                tempStageDatas.RemoveAll(x => x.stageType != RogueliteStageData.StageType.Boss);
            }
            
            RogueliteRuntimeSelectData buffData = buffSet.tempBuffDatas[Random.Range(0, buffSet.tempBuffDatas.Count)];
            RogueliteStageData stageData = tempStageDatas[Random.Range(0, tempStageDatas.Count)];
            
            _nextStageData = new NextStageData(stageData, buffData);

            buffText.text = buffData.buffText;
            backgroundImage.sprite = stageData.chapterBgImage;
            enemyImage.sprite = stageData.enemySpriteArt;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            RogueliteSelectManager.Instance.OnStageSelected(_nextStageData);
        }
    }
}