using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ShabuStudio.Data
{
    //Represents 1 deck.
    [System.Serializable]
    public class SavedDeck
    {
        public string deckName;
        public string deckID;
        public List<string> cardIDs = new List<string>();
    }
    //List of decks to save.
    [System.Serializable]
    public class PlayerSaveData
    {
        public List<SavedDeck> savedDecks = new List<SavedDeck>();
    }
    
    public class SaveSystem : MonoBehaviour
    {
        private string saveFilePath;

        private void Awake()
        {
            // Set save file path
            saveFilePath = Path.Combine(Application.persistentDataPath, "playerData.json");
        }


        //Main Save Function.
        //Saves all player's decks in the game.
        public void SaveAllDecks(List<DeckDataHolder> runtimeDecks)
        {
            PlayerSaveData saveData = new PlayerSaveData();

            // Loop through every deck currently in the game
            foreach (DeckDataHolder runtimeDeck in runtimeDecks)
            {
                // Create a "Save Format" deck
                SavedDeck diskDeck = new SavedDeck();
                diskDeck.deckName = runtimeDeck.deckName;
                diskDeck.deckID = runtimeDeck.deckID;

                // Convert the Card objects into String IDs
                foreach (CardData card in runtimeDeck.allCards)
                {
                    diskDeck.cardIDs.Add(card.cardID);
                }

                // Add to the main save file
                saveData.savedDecks.Add(diskDeck);
            }

            // Write to JSON
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(saveFilePath, json);
            
            Debug.Log($"Saved {runtimeDecks.Count} decks to {saveFilePath}");
        }
        
        // Main load function.
        public List<DeckDataHolder> LoadAllDecks()
        {
            if (!File.Exists(saveFilePath))
            {
                Debug.LogWarning("No save file found. Returning empty list.");
                return new List<DeckDataHolder>(); 
            }

            string json = File.ReadAllText(saveFilePath);
            PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(json);
            
            List<DeckDataHolder> loadedDecks = new List<DeckDataHolder>();

            // Reconstruct the decks
            foreach (SavedDeck diskDeck in saveData.savedDecks)
            {
                // Create a new Runtime Deck
                DeckDataHolder newDeck = new DeckDataHolder(diskDeck.deckID, diskDeck.deckName);

                // Convert IDs back to Card Objects using your Database
                foreach (string id in diskDeck.cardIDs)
                {
                    CardData card = CardDatabase.Instance.GetCardByID(id);
                    if (card != null)
                    {
                        newDeck.allCards.Add(card);
                    }
                }

                loadedDecks.Add(newDeck);
            }

            Debug.Log("Decks loaded successfully.");
            return loadedDecks;
        }
        
        #if UNITY_EDITOR
        public void SaveAllDecks(List<DeckDataHolder> runtimeDecks,string customPath)
        {
            PlayerSaveData saveData = new PlayerSaveData();

            // Loop through every deck currently in the game
            foreach (DeckDataHolder runtimeDeck in runtimeDecks)
            {
                // Create a "Save Format" deck
                SavedDeck diskDeck = new SavedDeck();
                diskDeck.deckName = runtimeDeck.deckName;
                diskDeck.deckID = runtimeDeck.deckID;

                // Convert the Card objects into String IDs
                foreach (CardData card in runtimeDeck.allCards)
                {
                    diskDeck.cardIDs.Add(card.cardID);
                }

                // Add to the main save file
                saveData.savedDecks.Add(diskDeck);
            }

            // Write to JSON
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(customPath, json);
            
            Debug.Log($"Saved {runtimeDecks.Count} decks to {customPath}");
        }
        public List<DeckDataHolder> LoadAllDecks(string customPath)
        {
            if (!File.Exists(customPath))
            {
                Debug.LogWarning("No save file found. Returning empty list.");
                return new List<DeckDataHolder>(); 
            }

            string json = File.ReadAllText(customPath);
            PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(json);
            
            List<DeckDataHolder> loadedDecks = new List<DeckDataHolder>();

            // Reconstruct the decks
            foreach (SavedDeck diskDeck in saveData.savedDecks)
            {
                // Create a new Runtime Deck
                DeckDataHolder newDeck = new DeckDataHolder(diskDeck.deckID, diskDeck.deckName);

                // Convert IDs back to Card Objects using your Database
                foreach (string id in diskDeck.cardIDs)
                {
                    CardData[] allCards = Resources.LoadAll<CardData>("Data/CardData");
                    CardData card = Array.Find(allCards, x => x.cardID == id);
                    if (card != null)
                    {
                        newDeck.allCards.Add(card);
                    }
                }

                loadedDecks.Add(newDeck);
            }

            Debug.Log("Decks loaded successfully.");
            return loadedDecks;
        }
        #endif
    }
}