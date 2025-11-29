using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ShabuStudio.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ShabuStudio.Gameplay
{
    
    public class CardDisplay : MonoBehaviour
    {
        public CardData cardData;
        public bool isPlayed = false;
        [SerializeField] private TextMeshProUGUI cardCostText;
        [SerializeField] private TextMeshProUGUI cardSpeedText;
        [SerializeField] private Canvas canvas;
        [SerializeField] private CardMovement cardMovement;
        [SerializeField] private Image cardImage;
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
            UpdateText();
            InitializeSortOrder(sortOrder);
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