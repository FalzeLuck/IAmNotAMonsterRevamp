using System.Collections.Generic;
using ShabuStudio.Camera;
using ShabuStudio.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ShabuStudio.Chapter
{
    public class ChapterButtonController : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]private ChapterData chapterData;
        private List<StageData> _stageDatas;
        
        private void Start()
        {
            Initialize();
        }

        void Initialize()
        {
            _stageDatas = StageDatabase.Instance.GetAllStagesByPrefix(chapterData.chapterPrefix);
            
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            OnSelectThisChapter();
        }


        private void OnSelectThisChapter()
        {
            ChapterSelectManager.Instance.SelectChapter(chapterData, transform);
        }
        
    }
}