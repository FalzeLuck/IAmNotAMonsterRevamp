using System.Collections.Generic;
using UnityEngine;

namespace ShabuStudio.Data
{
    public class CardDatabase : MonoBehaviour
    {
        // Singleton instance for easy access.
        public static CardDatabase Instance { get; private set; }

        // Dictionary for fast lookup of cards by ID
        private Dictionary<string, CardData> cardLookup = new Dictionary<string, CardData>();
        private bool isInitialized = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject); // To keep alive across scenes

            InitializeDatabase();
        }

        void InitializeDatabase()
        {
            if (isInitialized) return;

            // 1. Load all CardData assets from "Assets/Resources/Cards"
            // The string "Cards" is the path inside the Resources folder
            CardData[] allCards = Resources.LoadAll<CardData>("Data/CardData");

            // 2. Populate the Dictionary for fast lookup
            foreach (CardData card in allCards)
            {
                if (!cardLookup.ContainsKey(card.cardID))
                {
                    cardLookup.Add(card.cardID, card);
                }
                else
                {
                    Debug.LogWarning($"Duplicate Card ID found: {card.cardID}. Check card: {card.name}");
                }
            }

            isInitialized = true;
            Debug.Log($"Database Loaded: {cardLookup.Count} cards found.");
        }

        // Helper method to get a card by id.
        public CardData GetCardByID(string id)
        {
            if (cardLookup.TryGetValue(id, out CardData card))
            {
                return card;
            }
        
            Debug.LogError($"Card with ID '{id}' not found in Database!");
            return null;
        }
    }
}