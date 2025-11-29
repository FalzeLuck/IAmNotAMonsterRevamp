using UnityEngine;

namespace ShabuStudio.Chapter
{
    public class StageManager : MonoBehaviour
    {
        bool CheckIfUnlocked(StageData stage)
        {
            if (stage.prerequisiteStage == null) return true;

            return PlayerPrefs.GetInt("StageComplete_" + stage.prerequisiteStage.stageID, 0) == 1;
        }
    }
}