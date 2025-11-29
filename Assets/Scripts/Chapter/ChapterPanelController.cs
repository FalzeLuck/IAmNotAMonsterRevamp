using System;
using ShabuStudio.Camera;
using TMPro;
using UnityEngine;

namespace ShabuStudio.Chapter
{
    public class ChapterPanelController : MonoBehaviour
    {
        [Header("References")]
        public TextMeshProUGUI chapterNameText;
        public TextMeshProUGUI chapterDescriptionText;


        public void Start()
        {
            gameObject.SetActive(false);
        }

        public void ClosePanel()
        {
            gameObject.SetActive(false);
        }

        public void PanelSetup(ChapterData chapterData)
        {
            gameObject.SetActive(true);
            
            
            //SetUp auto localize Text
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
    }
}