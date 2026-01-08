using System;
using ShabuStudio.Camera;
using UnityEngine;

namespace ShabuStudio.Chapter
{
    public class ChapterSelectManager : MonoBehaviour
    {
        public static ChapterSelectManager Instance { get; private set; }
        
        
        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }
        
        public Canvas transitionCanvas;



        public void SelectChapter(ChapterData chapterData)
        {
            //Update Data
            GameManager.Instance.currentChapterData = chapterData;
            
            //Load Scene
            SceneLoader.LoadSceneWithTransitionCanvas(chapterData.chapterSceneName, transitionCanvas);
        }
        
        
    }
}