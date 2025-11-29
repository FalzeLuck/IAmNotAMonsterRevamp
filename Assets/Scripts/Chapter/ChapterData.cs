using UnityEngine;
using UnityEngine.Localization;

namespace ShabuStudio.Chapter
{
    [CreateAssetMenu(fileName = "ChapterData", menuName = "Stage/ChapterData", order = 1)]
    public class ChapterData : ScriptableObject
    {
        [Header("Chapter Info")]
        public string chapterPrefix;
        public LocalizedString chapterName;
        public LocalizedString chapterDescription;
    }
}