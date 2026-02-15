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
        
        public enum GameMode { Story,Roguelite }
        public GameMode currentGameMode;
        public Canvas transitionCanvas;
        
        public ChapterData currentChapterData;
        public StageData currentStageData;

        public void StartRogueliteMode()
        {
            currentGameMode = GameMode.Roguelite;
            SceneLoader.LoadSceneWithTransitionCanvas("Scene_RogueliteGameplay", transitionCanvas);
        }
    }
}