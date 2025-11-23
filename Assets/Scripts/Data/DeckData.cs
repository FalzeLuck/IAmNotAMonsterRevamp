using System.Collections.Generic;
using UnityEngine;

namespace ShabuStudio.Data
{
    [System.Serializable] 
    public class DeckData 
    {
        public string deckName;
        public string deckID;

        public List<CardData> allCards = new List<CardData>();

        // Constructor for creating a new empty deck easily.
        public DeckData(string id, string name)
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
    }
}