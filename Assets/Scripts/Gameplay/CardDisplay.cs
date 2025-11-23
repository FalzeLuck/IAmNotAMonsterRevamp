using System;
using ShabuStudio.Data;
using TMPro;
using UnityEngine;
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

        public int currentSpeed {get; private set;}
        private void Start()
        {
            if (cardData == null)
            {
                cardData = ScriptableObject.CreateInstance<CardData>();
                InitializeCard(cardData);
            }
        }

        //Set card data and update text.
        public void InitializeCard(CardData card, int sortOrder = 0)
        {
            cardData = card;
            RandomSpeed();
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

        void RandomSpeed()
        {
            currentSpeed = Random.Range(cardData.cardMinSpeed,cardData.cardMaxSpeed + 1);
        }
        
        

    }
}