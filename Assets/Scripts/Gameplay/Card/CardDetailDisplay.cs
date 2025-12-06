using System;
using ShabuStudio.Data;
using TMPro;
using UnityEngine;

namespace ShabuStudio.Gameplay
{
    public class CardDetailDisplay : MonoBehaviour
    {
        [Header("References")] 
        public TextMeshProUGUI cardNameText;
        public TextMeshProUGUI cardDescriptionText;

        private CardData previousCardData;
        public static CardData CurrentCardData;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void Reload()
        {
            if (CurrentCardData != previousCardData)
            {
                Initialize(CurrentCardData);
                previousCardData = CurrentCardData;
            }
        }


        void Initialize(CardData cardData)
        {
            if (cardData == null)
            {
                previousCardData.cardName.StringChanged -= UpdateCardNameText;
                previousCardData.cardDescription.StringChanged -= UpdateCardDescriptionText;
                gameObject.SetActive(false);
                return;
            }
            else
            {
                gameObject.SetActive(true);
                cardData.cardName.StringChanged -= UpdateCardNameText;
                cardData.cardDescription.StringChanged -= UpdateCardDescriptionText;

                cardData.cardName.StringChanged += UpdateCardNameText;
                cardData.cardDescription.StringChanged += UpdateCardDescriptionText;
                
            }


        }
        
        void UpdateCardNameText(string localizedText)
        {
            if(localizedText == null) return;
            cardNameText.text = localizedText;
        }
        
        void UpdateCardDescriptionText(string localizedText)
        {
            if(localizedText == null) return;
            cardDescriptionText.text = localizedText;
        }

    }
}