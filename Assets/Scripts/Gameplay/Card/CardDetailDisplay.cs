using System;
using DG.Tweening;
using ShabuStudio.Data;
using TMPro;
using UnityEngine;

namespace ShabuStudio.Gameplay
{
    public class CardDetailDisplay : MonoBehaviour
    {
        public Vector3 moveOffset;
        private Vector3 originalPosition;
        
        [Header("References")] 
        public TextMeshProUGUI cardNameText;
        public TextMeshProUGUI cardDescriptionText;

        private CardData previousCardData;
        public static CardData CurrentCardData;

        private void Start()
        {
            originalPosition = transform.position;
            ShowPanel(false);
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
                ShowPanel(false);
                previousCardData.cardName.StringChanged -= UpdateCardNameText;
                previousCardData.cardDescription.StringChanged -= UpdateCardDescriptionText;
                return;
            }
            else
            {
                cardData.cardName.StringChanged -= UpdateCardNameText;
                cardData.cardDescription.StringChanged -= UpdateCardDescriptionText;
                ShowPanel(true);
                cardData.cardName.StringChanged += UpdateCardNameText;
                cardData.cardDescription.StringChanged += UpdateCardDescriptionText;
                
            }


        }

        void ShowPanel(bool show)
        {
            if (!show)
            {
                transform.DOMove(transform.position + moveOffset, 0.2f).SetEase(Ease.OutBack);
            }
            else
            {
                transform.DOMove(originalPosition, 0.2f).SetEase(Ease.OutBack);
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