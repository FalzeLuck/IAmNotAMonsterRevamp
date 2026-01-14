using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ShabuStudio.Data;
using TMPro;
using UnityEngine;

namespace DeckBuild
{
    public class DeckDropdown : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown dropdown;
        private List<DeckDataHolder> currentDecks;

        private void Start()
        {
            PlayerDeckDataManager.Instance.OnDeckLoad += OnDeckLoaded;
        }

        private void OnDeckLoaded()
        {
            UpdateDropdown();
            dropdown.onValueChanged.AddListener(delegate { OnSelectedDeck(); });
            
            //Set current deck index
            dropdown.value = PlayerDeckDataManager.Instance.currentDeckIndex;
            dropdown.RefreshShownValue();
        }

        private void UpdateDropdown()
        {
            dropdown.ClearOptions();
            currentDecks = PlayerDeckDataManager.Instance.savedDecks;
            
            List<string> options = new List<string>();
            
            foreach (DeckDataHolder deck in currentDecks)
            {
                options.Add(deck.deckName);
            }
            
            dropdown.AddOptions(options);
            
        }

        public void OnSelectedDeck()
        {
            int pickEntryIndex = dropdown.value;
            string selectedDeckName = dropdown.options[pickEntryIndex].text;
            Debug.Log(selectedDeckName);
        }

        public void AddNewDeckButton()
        {
            AddNewDeck().Forget();
        }
        
        private async UniTaskVoid AddNewDeck()
        {
            Debug.Log($"{currentDecks.Count} decks before.");
            await PlayerDeckDataManager.Instance.AddNewDeck();
            UpdateDropdown();
            dropdown.value = currentDecks.Count - 1;
            dropdown.RefreshShownValue();
            Debug.Log($"{currentDecks.Count} decks after.");
        }

        public void RemoveDeckByName(string deckName)
        {
            for (int i = 0; i < dropdown.options.Count; i++)
            {
                if (dropdown.options[i].text == deckName)
                {
                    if (dropdown.value == i)
                    {
                        dropdown.value = 0;
                    }
                    
                    dropdown.options.RemoveAt(i);
                    
                    dropdown.RefreshShownValue();
                    break;
                }
            }
        }
    }
}