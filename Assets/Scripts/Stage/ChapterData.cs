using UnityEngine;

namespace ShabuStudio.Stage
{
    [CreateAssetMenu(fileName = "ChapterData", menuName = "Stage/ChapterData", order = 1)]
    public class ChapterData : ScriptableObject
    {
        [Header("Chapter Info")]
        public string chapterID;
        public string chapterName;
        [TextArea]public string chapterDescription;
        public StageData[] stages;
    }
}