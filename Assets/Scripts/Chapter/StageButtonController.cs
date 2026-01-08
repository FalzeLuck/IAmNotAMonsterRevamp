using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ShabuStudio.Chapter
{
    public class StageButtonController : MonoBehaviour, IPointerClickHandler
    {
        public StageData stageData;
        private bool isUnlocked;
        
        
        public Canvas transitionCanvas;

        private void Start()
        {
            isUnlocked = CheckIfUnlocked(stageData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isUnlocked)
            {
                GameManager.Instance.currentStageData = stageData;
                SceneLoader.LoadSceneWithTransitionCanvas("Scene_Gameplay", transitionCanvas);
            }
        }
        
        bool CheckIfUnlocked(StageData stage)
        {
            if (stage.prerequisiteStage == null) return true; //No prerequisite

            return PlayerPrefs.GetInt("StageComplete_" + stage.prerequisiteStage.stageID, 0) == 1;
        }
    }
}