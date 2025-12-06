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
        public int currentCost{private set; get;}
        public List<Buff> OnStartTurnBuffs = new List<Buff>();
        public bool isDead = false;
        
        
        public System.Action<int, int> OnHealthChanged; // Current, Max
        public System.Action OnStatusChanged; // When buffs are added/removed
        
        [Header("Deck Data")]
        public DeckDataHolder deckDataHolder;
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
                deckDataHolder = PlayerDataManager.Instance.savedDecks[PlayerDataManager.Instance.currentDeckIndex];
            }

            if (deckDataHolder == null)
            {
                Debug.LogError($"Deck Data of {unitName} is null!");
            }
            
            deckDataHolder.ResetAvailableCards();
        }
        
        // -----------------------------------------
        // Cost Manage
        // -----------------------------------------

        public void AddCost(int amount)
        {
            if(amount >= 0)
                currentCost += amount;
        }
        
        public void RemoveCost(int amount)
        {
            if(amount >= 0)
                currentCost -= amount;
        }

        public void SetCost(int amount)
        {
            currentCost = amount;
            if(currentCost < 0) currentCost = 0;
        }

        // ----------------------------------------------
        // Health Logic
        // ----------------------------------------------
        
        public void TakeDamage(int damage,out int uiDamage)
        {
            if(damage <= 0)
            {
                uiDamage = 0;
                return;
            }
            
            
            //Future Calculate Damage method HERE...
            int finalDamage = damage + Stats.AdditionalTakenDamage;
            
            currentHealth -= finalDamage;
            uiDamage = finalDamage;
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
                if (buff.buffType == Buff.BuffType.OnAction)
                {
                    buff.ApplyBuff(this);
                }
                else if(buff.buffType == Buff.BuffType.OnTurnStart)
                {
                    OnStartTurnBuffs.Add(buff);
                }
            }
            
            UpdateUI();
        }

        public void OnStartTurn()
        {
            foreach (var buff in OnStartTurnBuffs)
            {
                buff.ApplyBuff(this);
            }
            UpdateUI();
            OnStartTurnBuffs.Clear();
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