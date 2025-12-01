using System;
using System.Collections.Generic;
using System.IO;
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
    
        public List<DeckDataHolder> savedDecks = new List<DeckDataHolder>(); // < DeckData>
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
        #if UNITY_EDITOR
        public void LoadAllDecks(string customPath)
        {
            SaveSystem saveSystem = FindFirstObjectByType<SaveSystem>();
            savedDecks = saveSystem.LoadAllDecks(customPath);
        }
        #endif 
    }
}