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
        
        [SerializeField] BaseStats baseStats;
        public Stats Stats { get; private set; }
        public int currentHealth;
        public bool isDead = false;
        
        
        public System.Action<int, int> OnHealthChanged; // Current, Max
        public System.Action OnStatusChanged; // When buffs are added/removed
        
        [Header("Deck Data")]
        public DeckData deckData;
        public List<CardData> availableCards = new List<CardData>();
        public List<CardData> droppedCards = new List<CardData>();

        void Awake()
        {
            Stats = new Stats(new StatsMediator(), baseStats);
        }
        
        private void Start()
        {
            currentHealth = baseStats.maxHealth;
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
            if (currentHealth > Stats.MaxHealth) currentHealth = Stats.MaxHealth;
            UpdateUI();
        }
        
        
        // -------------------
        // Buff Logic
        // -------------------

        public void ApplyBuff(List<Buff> buffList)
        {
            foreach (var buff in buffList)
            {
                buff.ApplyBuff(this);
            }
            
            UpdateUI();
        }

        public void DecreaseBuffTurn(int value)
        {
            Stats.Mediator.DecreaseTurnCountdown(value);
        }
        
        // ----------
        // UI Things
        // ----------
        public void UpdateUI()
        {
            OnHealthChanged?.Invoke(currentHealth, Stats.MaxHealth);
        }

        void Die()
        {
            isDead = true;
        }
    }
}