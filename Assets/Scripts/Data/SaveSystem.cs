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
        public void SaveAllDecks(List<DeckData> runtimeDecks)
        {
            PlayerSaveData saveData = new PlayerSaveData();

            // Loop through every deck currently in the game
            foreach (DeckData runtimeDeck in runtimeDecks)
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
        public List<DeckData> LoadAllDecks()
        {
            if (!File.Exists(saveFilePath))
            {
                Debug.LogWarning("No save file found. Returning empty list.");
                return new List<DeckData>(); 
            }

            string json = File.ReadAllText(saveFilePath);
            PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(json);
            
            List<DeckData> loadedDecks = new List<DeckData>();

            // Reconstruct the decks
            foreach (SavedDeck diskDeck in saveData.savedDecks)
            {
                // Create a new Runtime Deck
                DeckData newDeck = new DeckData(diskDeck.deckID, diskDeck.deckName);

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
        
        
    }
}