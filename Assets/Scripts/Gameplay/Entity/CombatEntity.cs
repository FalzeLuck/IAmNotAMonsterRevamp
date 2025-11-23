using System.Collections.Generic;
using ShabuStudio.Data;
using UnityEngine;

namespace ShabuStudio.Gameplay
{
    public class CombatEntity : MonoBehaviour
    {
        [Header("Stats")]
        public string unitName;
        public ActionOwner unitType;
        public int maxHealth = 100;
        public int currentHealth;
        public bool isDead = false;
        
        //[Header("Buff")]
        
        public System.Action<int, int> OnHealthChanged; // Current, Max
        public System.Action OnStatusChanged; // When buffs are added/removed
        
        [Header("Deck Data")]
        public DeckData deckData;
        public List<CardData> availableCards = new List<CardData>();
        public List<CardData> droppedCards = new List<CardData>();
        
        private void Start()
        {
            currentHealth = maxHealth;
            UpdateUI();
        }
        
        // ----------------------------------------------
        // Deck Manage
        // --------------------------------------------

        public void InitializeDeck()
        {
            if (unitType == ActionOwner.Player) //If player
            {
                deckData = PlayerDataManager.Instance.savedDecks[PlayerDataManager.Instance.currentDeckIndex];
            }

            if (deckData == null)
            {
                Debug.LogError($"Deck Data of {unitName} is null!");
            }
            
            deckData.ResetAvailableCards();
        }

        // ----------------------------------------------
        // Health Logic
        // ----------------------------------------------
        
        public void TakeDamage(int damage)
        {
            int finalDamage = damage;
            
            //Future Calculate Damage method HERE...
            
            currentHealth -= finalDamage;
            if(currentHealth < 0) currentHealth = 0;
            
            
            UpdateUI();
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        public void Heal(int amount)
        {
            currentHealth += amount;
            if (currentHealth > maxHealth) currentHealth = maxHealth;
            UpdateUI();
        }
        
        
        // -------------------
        // Buff Logic
        // -------------------
        
        private void UpdateUI()
        {
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        void Die()
        {
            isDead = true;
        }
    }
}