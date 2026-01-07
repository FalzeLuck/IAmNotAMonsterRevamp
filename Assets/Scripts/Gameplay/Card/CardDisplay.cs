using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ShabuStudio.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ShabuStudio.Gameplay
{
    
    public class CardDisplay : MonoBehaviour
    {
        public CardData cardData;
        public bool isPlayed = false;
        [SerializeField] private CardDisplayPresetSprite cardDisplayPreset;
        [SerializeField] private TextMeshProUGUI cardCostText;
        [SerializeField] private TextMeshProUGUI cardSpeedText;
        [SerializeField] private TextMeshProUGUI cardNameText;
        [SerializeField] private TextMeshProUGUI cardTypeText;
        [SerializeField] private Image cardImageBackground;
        [SerializeField] private Image cardImageSprite;
        [SerializeField] private Canvas canvas;
        [SerializeField] private CardMovement cardMovement;
        [SerializeField] private Image cardImage;
        [SerializeField] private Image cardFloatingIcon;
        private Material _cardDynamicMaterial;
        private int _dissolveAmountID; 
        private CanvasGroup _canvasGroup;

        public int currentSpeed {get; private set;}
        private void Start()
        {
            if (cardData == null)
            {
                cardData = ScriptableObject.CreateInstance<CardData>();
                InitializeCard(cardData);
            }
            
            _cardDynamicMaterial = new Material(cardImage.material);
            cardImage.material = _cardDynamicMaterial;
            _dissolveAmountID = Shader.PropertyToID("_DissolveAmount");
            
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        //Set card data and update text.
        public void InitializeCard(CardData card, int sortOrder = 0)
        {
            cardData = card;
            currentSpeed = card.cardSpeed;
            cardTypeText.text = card.cardType.ToString();
            cardImageSprite.sprite = card.cardImage;


            if (card.cardFloatIcon != null)
            {
                cardFloatingIcon.sprite = card.cardFloatIcon;
                cardFloatingIcon.gameObject.transform.localPosition = card.cardFloatIconPosition;
                cardFloatingIcon.gameObject.SetActive(true);
            }
            else
            {
                cardFloatingIcon.gameObject.SetActive(false);
            }
            
            //Change BG according to card type.
            switch (card.cardType)
            {
                case CardType.Attack:
                    cardImageBackground.sprite = cardDisplayPreset.attackCardBG;
                    break;
                case CardType.Buff:
                    cardImageBackground.sprite = cardDisplayPreset.buffCardBG;
                    break;
                case CardType.Debuff:
                    cardImageBackground.sprite = cardDisplayPreset.debuffCardBG;
                    break;
            }
            
            
            card.cardName.StringChanged -= HandleNameText;
            card.cardName.StringChanged += HandleNameText;
            UpdateText();
            if(cardMovement != null)
                InitializeSortOrder(sortOrder);
        }

        void HandleNameText(string localizedText)
        {
            cardNameText.text = localizedText;
        }

        //For Initilize card sort
        void InitializeSortOrder(int orderIndex)
        {
            canvas.sortingOrder = orderIndex;
            cardMovement.defaultCardOrder = orderIndex;
        }

        public void UpdateDefaultCardOrder(int orderIndex)
        {
            cardMovement.defaultCardOrder = orderIndex;
            UpdateCardOrder(orderIndex);
        }

        public void UpdateCardOrder(int orderIndex)
        {
            canvas.sortingOrder = orderIndex;
        }


        
        //Update Every text related in cards.
        void UpdateText()
        {
            cardCostText.text = cardData.cardCost.ToString("F0");
            cardSpeedText.text = currentSpeed.ToString();
        }


        public void RemoveCard()
        {
            transform.SetParent(transform.parent.parent);
            DissolveCard();
        }

        void DissolveCard()
        {
            float duration = 0.5f;
            Ease ease = Ease.Linear;
            
            _cardDynamicMaterial.DOFloat(1f, _dissolveAmountID, duration)
                .SetEase(ease);

            _canvasGroup.DOFade(0f, duration)
                .SetEase(ease)
                .OnComplete(() =>
                {
                    DOTween.Kill(gameObject);
                    Destroy(gameObject);
                });
        }

    }
}