using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
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
        [SerializeField] private List<Buff> OnStartTurnBuffs;
        public bool isDead = false;
        public System.Action<int, int> OnHealthChanged; // Current, Max
        public System.Action OnStatusChanged; // When buffs are added/removed
        
        
        [Header("References")]
        public Transform vfxSpawnPoint;
        
        [Header("Deck Data")]
        public DeckDataHolder deckDataHolder;
        public List<CardData> availableCards = new List<CardData>();
        public List<CardData> droppedCards = new List<CardData>();

        void Awake()
        {
            Stats = new Stats(new StatsMediator(), baseStats);
        }
        
        protected virtual void Start()
        {
            OnStartTurnBuffs = new List<Buff>();
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
        
        public virtual void TakeDamage(int damage)
        {
            if(damage <= 0)
            {
                return;
            }
            
            
            //Future Calculate Damage method HERE...
            int finalDamage = CalculateDamageTaken(damage);
            
            currentHealth -= finalDamage;
            if(currentHealth < 0) currentHealth = 0;
            
            if (currentHealth <= 0)
            {
                Die();
            }
            
        }
        
        public virtual void Heal(int amount)
        {
            currentHealth += amount;
            if (currentHealth > Stats.MaxHealth) currentHealth = Stats.MaxHealth;
        }

        public int CalculateDamageDealt(int rawDamage)
        {
            return rawDamage + Stats.AdditionalDamage;
        }

        public int CalculateDamageTaken(int dealtDamage)
        {
            return dealtDamage + Stats.AdditionalTakenDamage;
        }
        
        
        // -------------------
        // Buff Logic
        // -------------------

        public async UniTask ApplyBuff(List<Buff> buffList,CancellationToken token)
        {
            foreach (var buff in buffList)
            {
                if (buff.buffType == Buff.BuffType.OnAction)
                {
                    await buff.ApplyBuff(this,token);
                }
                else if(buff.buffType == Buff.BuffType.OnTurnStart)
                {
                    OnStartTurnBuffs.Add(buff);
                }
            }
            
            UpdateUI();
            await UniTask.NextFrame(token);
        }

        public void OnStartTurn()
        {
            foreach (var buff in OnStartTurnBuffs)
            {
                buff.ApplyBuff(this,this.GetCancellationTokenOnDestroy()).Forget();
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