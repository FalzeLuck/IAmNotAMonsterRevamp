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
        
        [SerializeField]private ChapterPanelController chapterPanelController;
        private ChapterData currentChapterData;



        public void SelectChapter(ChapterData chapterData, Transform lookAtPoint)
        {
            //Update Data
            currentChapterData = chapterData;
            chapterPanelController.PanelSetup(currentChapterData);
            
            //Update Camera
            CameraManager.Instance.SwitchCamera("Selected Chapter Camera");
            CameraManager.Instance.ChangeCameraTarget(lookAtPoint);
        }
        
        public void DeselectChapter()
        {
            //Update Camera
            CameraManager.Instance.SwitchCamera("Chapter Camera");
            currentChapterData = null;
            chapterPanelController.ClosePanel();
        }
        
    }
}