using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShabuStudio.Data
{
    [System.Serializable] 
    public class DeckDataHolder 
    {
        public string deckName;
        public string deckID;

        public List<CardData> allCards = new List<CardData>();

        // Constructor for creating a new empty deck easily.
        public DeckDataHolder(string id, string name)
        {
            this.deckID = id;
            this.deckName = name;
        }
        
        
        //--------------------------
        //Gameplay Section
        //-------------------------
        
        public List<CardData> availableCards = new List<CardData>();
        public List<CardData> droppedCards = new List<CardData>();
        
        
        //Reset available cards.
        public void ResetAvailableCards()
        {
            availableCards = new List<CardData>(allCards);
            droppedCards.Clear();
        }

        public void ResetAvailableCards(CardType type)
        {
            List<CardData> currentAddingCards = new List<CardData>(allCards);
            
            currentAddingCards.RemoveAll(x => x.cardType != type);
            
            availableCards.AddRange(currentAddingCards);
            
            droppedCards.RemoveAll(x => x.cardType == type);
        }

        public CardData DrawRandomCard()
        {
            if(availableCards.Count == 0) ResetAvailableCards();
            
            int index = Random.Range(0, availableCards.Count);
            
            //Get card Data
            CardData drawnCard = availableCards[index];
            
            //Remove card from list.
            availableCards.RemoveAt(index);
            
            //Add card to dropped cards.
            droppedCards.Add(drawnCard);
            
            return drawnCard;
        }

        public CardData DrawRandomCardFixedType(CardType type)
        {
            //Reset specific type if no more card of that type available
            if(availableCards.Count(x => x.cardType == type) == 0) ResetAvailableCards(type);
            
            //Create a list that have only specific cards
            List<CardData> specificCardList = availableCards.Where(x => x.cardType == type).ToList();
            
            //Draw random card from list
            int index = Random.Range(0, specificCardList.Count);
            CardData drawnCard = specificCardList[index];
            
            //Remove card from list.
            availableCards.Remove(drawnCard);
            
            //Add card to dropped cards.
            droppedCards.Add(drawnCard);
            
            return drawnCard;
        }
    }
}