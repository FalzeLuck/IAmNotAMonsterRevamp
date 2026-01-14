using System;
using System.Collections.Generic;
using ShabuStudio.Data;
using UnityEngine;

namespace DeckBuild
{
    // Runtime Object
    [Serializable]
    public class InventoryEntry
    {
        public CardData cardData;
        public int amountOwned;
        
        // Constructor
        public InventoryEntry(CardData card, int amount)
        {
            cardData = card;
            amountOwned = amount;
        }
    }

    // Save Object
    [Serializable]
    public class InventorySaveEntry
    {
        public string cardID;
        public int amount;
    }

    public class PlayerInventory : MonoBehaviour
    {
        public static PlayerInventory Instance { get; private set; }
        
        private Dictionary<string, InventoryEntry> inventoryDict = new Dictionary<string, InventoryEntry>();

        // For Debugging: A list to see items in the Inspector
        [SerializeField] private List<InventoryEntry> debugList = new List<InventoryEntry>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            LoadInventory();
        }

        // ---------------------------------------------------------
        // CORE LOGIC
        // ---------------------------------------------------------

        public void AddCard(CardData card, int amountToAdd = 1)
        {
            if (card == null) return;

            if (inventoryDict.ContainsKey(card.cardID))
            {
                // We already have this card, just increase stack
                inventoryDict[card.cardID].amountOwned += amountToAdd;
            }
            else
            {
                // New card discovery! Create new entry.
                InventoryEntry newEntry = new InventoryEntry(card, amountToAdd);
                inventoryDict.Add(card.cardID, newEntry);
            }

            UpdateDebugList(); // Refresh inspector view
        }

        public bool RemoveCard(CardData card, int amountToRemove = 1)
        {
            if (!inventoryDict.ContainsKey(card.cardID)) return false;

            InventoryEntry entry = inventoryDict[card.cardID];

            if (entry.amountOwned >= amountToRemove)
            {
                entry.amountOwned -= amountToRemove;

                UpdateDebugList();
                return true;
            }

            Debug.LogWarning($"Not enough cards! Have {entry.amountOwned}, trying to remove {amountToRemove}");
            return false;
        }

        public int GetCardCount(string cardID)
        {
            if (inventoryDict.TryGetValue(cardID, out InventoryEntry entry))
            {
                return entry.amountOwned;
            }

            return 0;
        }

        // Use this to sync the Dictionary to the List so you can see it in Unity Inspector
        private void UpdateDebugList()
        {
            debugList.Clear();
            foreach (var kvp in inventoryDict)
            {
                debugList.Add(kvp.Value);
            }
        }

        // ---------------------------------------------------------
        // SAVE & LOAD INTEGRATION
        // ---------------------------------------------------------

        public void SaveInventory()
        {
            // 1. Convert Dictionary -> Save List
            List<InventorySaveEntry> saveList = new List<InventorySaveEntry>();
            foreach (var kvp in inventoryDict)
            {
                InventorySaveEntry entry = new InventorySaveEntry();
                entry.cardID = kvp.Key;
                entry.amount = kvp.Value.amountOwned;
                saveList.Add(entry);
            }

            SaveSystem.SaveInventory(saveList);
        }

        public void LoadInventory()
        {
            List<InventorySaveEntry> saveList = SaveSystem.LoadInventory();

            inventoryDict.Clear();

            foreach (InventorySaveEntry saveEntry in saveList)
            {
                CardData cardSO = CardDatabase.Instance.GetCardByID(saveEntry.cardID);
                
                if (cardSO != null)
                {
                    InventoryEntry newEntry = new InventoryEntry(cardSO, saveEntry.amount);
                    inventoryDict.Add(saveEntry.cardID, newEntry);
                }
            }
            
            Debug.Log("Inventory Loaded Successfully.");
        }

    }
}