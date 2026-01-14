using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ShabuStudio.Data
{
    public class PlayerDeckDataManager : MonoBehaviour
    {
        public static PlayerDeckDataManager Instance { get; private set; }
    
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

        public System.Action OnDeckLoad;


        private async void Start()
        {
            await LoadAllDecks();
            OnDeckLoad?.Invoke();
        }

        
        public async UniTask AddNewDeck()
        {
            string deckName = "New Deck";
            string deckID = Guid.NewGuid().ToString();
            DeckDataHolder newDeck = new DeckDataHolder(deckID, deckName);
            savedDecks.Add(newDeck);
            
            await SaveAllDecks();
        }
        
        //Save all decks that player have.
        public async UniTask SaveAllDecks()
        {
            await SaveSystem.SaveAllDecks(savedDecks);
        }
        
        //Load all decks that player have.
        public async UniTask LoadAllDecks()
        {
            savedDecks = await SaveSystem.LoadAllDecks();
        }
        
        #if UNITY_EDITOR
        public void SaveAllDecks(string customPath)
        {
            SaveSystem.SaveAllDecks(savedDecks,customPath);
        }
        public void LoadAllDecks(string customPath)
        {
            savedDecks = SaveSystem.LoadAllDecks(customPath);
        }
        #endif 
    }
}