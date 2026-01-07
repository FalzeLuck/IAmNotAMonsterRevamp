using ShabuStudio.Chapter;
using UnityEngine;

namespace ShabuStudio
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        public ChapterData currentChapterData;
        public StageData currentStageData;
    }
}