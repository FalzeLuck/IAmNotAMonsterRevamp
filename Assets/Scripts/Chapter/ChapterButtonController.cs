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
        public TextMeshProUGUI chapterNameText;
        public TextMeshProUGUI chapterDescriptionText;
        private List<StageData> _stageDatas;
        
        private void Start()
        {
            Initialize();
        }

        void Initialize()
        {
            _stageDatas = StageDatabase.Instance.GetAllStagesByPrefix(chapterData.chapterPrefix);
            
            
            //Setup Auto Localization Change
            chapterData.chapterName.StringChanged -= UpdateNameText;
            chapterData.chapterDescription.StringChanged -= UpdateDescriptionText;
            
            chapterData.chapterName.StringChanged += UpdateNameText;
            chapterData.chapterDescription.StringChanged += UpdateDescriptionText;
        }

        void UpdateNameText(string localizedText)
        {
            chapterNameText.text = localizedText;
        }
        
        void UpdateDescriptionText(string localizedText)
        {
            chapterDescriptionText.text = localizedText;
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