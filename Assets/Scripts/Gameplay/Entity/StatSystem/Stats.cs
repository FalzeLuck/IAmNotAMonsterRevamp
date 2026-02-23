using System.Collections.Generic;

namespace ShabuStudio.Gameplay
{
    public enum StatsType
    {
        MaxHealth,
        MaxHandSize,
        AdditionalDamage,
        AdditionalTakenDamage,
        DamageMultiplier
    }
    
    public class Stats
    {
        readonly StatsMediator statsMediator;
        readonly BaseStats baseStats;
        
        // This dictionary stores the "Current Base" values for this specific run.
        private Dictionary<StatsType, float> runtimeBaseValuesFloat = new Dictionary<StatsType, float>();

        public StatsMediator Mediator => statsMediator;
        
        public Stats(StatsMediator statsMediator, BaseStats baseStats)
        {
            this.statsMediator = statsMediator;
            this.baseStats = baseStats;

            // COPY values from ScriptableObject to Local Dictionary on initialization
            runtimeBaseValuesFloat[StatsType.MaxHealth] = baseStats.maxHealth;
            runtimeBaseValuesFloat[StatsType.MaxHandSize] = baseStats.maxHandSize;
            runtimeBaseValuesFloat[StatsType.AdditionalDamage] = baseStats.additionalDamage;
            runtimeBaseValuesFloat[StatsType.AdditionalTakenDamage] = baseStats.additionalTakenDamage;
            runtimeBaseValuesFloat[StatsType.DamageMultiplier] = baseStats.damageMultiplier;
        }

        // --- PROPERTIES ---

        public int MaxHealth
        {
            get => GetStat(StatsType.MaxHealth);
            set => SetBaseStat(StatsType.MaxHealth, value);
        }
        
        public int MaxHandSize
        {
            get => GetStat(StatsType.MaxHandSize);
            set => SetBaseStat(StatsType.MaxHandSize, value);
        }

        public int AdditionalDamage
        {
            get => GetStat(StatsType.AdditionalDamage);
            set => SetBaseStat(StatsType.AdditionalDamage, value);
        }

        public int AdditionalTakenDamage
        {
            get => GetStat(StatsType.AdditionalTakenDamage);
            set => SetBaseStat(StatsType.AdditionalTakenDamage, value);
        }

        public float DamageMultiplier
        {
            get => GetStatFloat(StatsType.DamageMultiplier);
            set => SetBaseStat(StatsType.DamageMultiplier, value);
        }
        
        // --- HELPER METHODS ---

        private int GetStat(StatsType type)
        {
            // 1. Get the local runtime base value (e.g., 100 + 10 upgrade = 110)
            float baseValue = runtimeBaseValuesFloat[type];

            // 2. Apply temporary modifiers via Mediator (e.g., "Weak" debuff reduces damage)
            var q = new Query(type, baseValue);
            statsMediator.PerformQuery(this, q);
            
            return (int)q.Value;
        }
        
        private float GetStatFloat(StatsType type)
        {
            // 1. Get the local runtime base value (e.g., 100 + 10 upgrade = 110)
            float baseValue = runtimeBaseValuesFloat[type];

            // 2. Apply temporary modifiers via Mediator (e.g., "Weak" debuff reduces damage)
            var q = new Query(type, baseValue);
            statsMediator.PerformQuery(this, q);
            
            return q.Value;
        }

        private void SetBaseStat(StatsType type, float newValue)
        {
            // This only modifies the dictionary in memory. 
            // The ScriptableObject file remains untouched.
            if (runtimeBaseValuesFloat.ContainsKey(type))
            {
                runtimeBaseValuesFloat[type] = newValue;
            }
            else
            {
                runtimeBaseValuesFloat.Add(type, newValue);
            }
        }
    }
}