using UnityEngine;

namespace ShabuStudio.Chapter
{
    [CreateAssetMenu(fileName = "StageData", menuName = "Stage/StageData")]
    public class StageData : ScriptableObject
    {
        [Header("Stage Info")] 
        public string stageID; // Ex. 1-1
        public string stageName;
        [TextArea] public string stageDescription;
    
        [Header("Requirement")]
        public StageData prerequisiteStage;
    }
}