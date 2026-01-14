using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using DeckBuild;
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
    public class DeckSaveData
    {
        public List<SavedDeck> savedDecks = new List<SavedDeck>();
    }
    
    [System.Serializable]
    public class InventorySaveData
    {
        public List<InventorySaveEntry> inventoryItems = new List<InventorySaveEntry>();
    }
    
    public static class SaveSystem
    {
        public static string DeckPath => Path.Combine(Application.persistentDataPath, "deckData.json");
        public static string InventoryPath => Path.Combine(Application.persistentDataPath, "inventoryData.json");



        public static void SaveInventory(List<InventorySaveEntry> inventoryData)
        {
            // Wrap the list in the class
            InventorySaveData wrapper = new InventorySaveData();
            wrapper.inventoryItems = inventoryData;

            string json = JsonUtility.ToJson(wrapper, true);

            // Write to disk
            File.WriteAllText(InventoryPath, json);
            
            Debug.Log($"Inventory saved to {InventoryPath}");
        }
        
        public static List<InventorySaveEntry> LoadInventory()
        {
            if (!File.Exists(InventoryPath))
            {
                Debug.LogWarning("No inventory save found. Starting fresh.");
                return new List<InventorySaveEntry>();
            }
            
            string json = File.ReadAllText(InventoryPath);
            
            InventorySaveData wrapper = JsonUtility.FromJson<InventorySaveData>(json);
            
            return wrapper.inventoryItems;
        }
        
        //Saves all player's decks in the game.
        public static async UniTask SaveAllDecks(List<DeckDataHolder> runtimeDecks)
        {
            DeckSaveData saveData = new DeckSaveData();

            // Loop through every deck currently in the game
            foreach (DeckDataHolder runtimeDeck in runtimeDecks)
            {
                // Create a "Save Format" deck
                SavedDeck diskDeck = new SavedDeck();
                diskDeck.deckName = runtimeDeck.deckName;
                diskDeck.deckID = runtimeDecks.IndexOf(runtimeDeck).ToString();

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
            await File.WriteAllTextAsync(DeckPath, json);
            
            Debug.Log($"Saved {runtimeDecks.Count} decks to {DeckPath}");
        }
        
        // Main load function.
        public static async UniTask<List<DeckDataHolder>> LoadAllDecks()
        {
            if (!File.Exists(DeckPath))
            {
                Debug.LogWarning("No save file found. Returning empty list.");
                return new List<DeckDataHolder>(); 
            }

            string json = await File.ReadAllTextAsync(DeckPath);
            DeckSaveData saveData = JsonUtility.FromJson<DeckSaveData>(json);
            
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

            Debug.Log($"Decks loaded successfully. Loaded {loadedDecks.Count} decks");
            return loadedDecks;
        }
        
        #if UNITY_EDITOR
        public static void SaveAllDecks(List<DeckDataHolder> runtimeDecks,string customPath)
        {
            DeckSaveData saveData = new DeckSaveData();

            // Loop through every deck currently in the game
            foreach (DeckDataHolder runtimeDeck in runtimeDecks)
            {
                // Create a "Save Format" deck
                SavedDeck diskDeck = new SavedDeck();
                diskDeck.deckName = runtimeDeck.deckName;
                diskDeck.deckID = runtimeDecks.IndexOf(runtimeDeck).ToString();

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
        public static List<DeckDataHolder> LoadAllDecks(string customPath)
        {
            if (!File.Exists(customPath))
            {
                Debug.LogWarning("No save file found. Returning empty list.");
                return new List<DeckDataHolder>(); 
            }

            string json = File.ReadAllText(customPath);
            DeckSaveData saveData = JsonUtility.FromJson<DeckSaveData>(json);
            
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