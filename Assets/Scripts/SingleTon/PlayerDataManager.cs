using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShabuStudio.Data
{
    public class PlayerDataManager : MonoBehaviour
    {
        public static PlayerDataManager Instance { get; private set; }
    
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
    
        public List<DeckData> savedDecks = new List<DeckData>(); // < DeckData>
        public int currentDeckIndex = 0;


        private void Start()
        {
            LoadAllDecks();
        }

        //Save all decks that player have.
        public void SaveAllDecks()
        {
            SaveSystem saveSystem = FindFirstObjectByType<SaveSystem>();
            saveSystem.SaveAllDecks(savedDecks);
        }
        
        //Load all decks that player have.
        public void LoadAllDecks()
        {
            SaveSystem saveSystem = FindFirstObjectByType<SaveSystem>();
            savedDecks = saveSystem.LoadAllDecks();
        }
    }
}