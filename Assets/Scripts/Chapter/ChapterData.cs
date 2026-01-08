using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace ShabuStudio.Chapter
{
    [CreateAssetMenu(fileName = "ChapterData", menuName = "Stage/ChapterData", order = 1)]
    public class ChapterData : ScriptableObject
    {
        [Header("Chapter Info")]
        public string chapterPrefix;
        public string chapterSceneName;
        public Texture2D chapterBgImage;
        public GameObject chapterBg3DObject;
        public LocalizedString chapterName;
        public LocalizedString chapterDescription;
    }
}