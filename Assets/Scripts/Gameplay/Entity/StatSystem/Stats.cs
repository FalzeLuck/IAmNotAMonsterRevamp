using System.Collections.Generic;

namespace ShabuStudio.Gameplay
{
    public enum StatsType
    {
        MaxHealth,
        MaxHandSize,
        AdditionalDamage,
        AdditionalTakenDamage
    }
    
    public class Stats
    {
        readonly StatsMediator statsMediator;
        readonly BaseStats baseStats;
        
        // This dictionary stores the "Current Base" values for this specific run.
        private Dictionary<StatsType, int> runtimeBaseValues = new Dictionary<StatsType, int>();

        public StatsMediator Mediator => statsMediator;
        
        public Stats(StatsMediator statsMediator, BaseStats baseStats)
        {
            this.statsMediator = statsMediator;
            this.baseStats = baseStats;

            // COPY values from ScriptableObject to Local Dictionary on initialization
            runtimeBaseValues[StatsType.MaxHealth] = baseStats.maxHealth;
            runtimeBaseValues[StatsType.MaxHandSize] = baseStats.maxHandSize;
            runtimeBaseValues[StatsType.AdditionalDamage] = baseStats.additionalDamage;
            runtimeBaseValues[StatsType.AdditionalTakenDamage] = baseStats.additionalTakenDamage;
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
        
        // --- HELPER METHODS ---

        private int GetStat(StatsType type)
        {
            // 1. Get the local runtime base value (e.g., 100 + 10 upgrade = 110)
            int baseValue = runtimeBaseValues[type];

            // 2. Apply temporary modifiers via Mediator (e.g., "Weak" debuff reduces damage)
            var q = new Query(type, baseValue);
            statsMediator.PerformQuery(this, q);
            
            return q.Value;
        }

        private void SetBaseStat(StatsType type, int newValue)
        {
            // This only modifies the dictionary in memory. 
            // The ScriptableObject file remains untouched.
            if (runtimeBaseValues.ContainsKey(type))
            {
                runtimeBaseValues[type] = newValue;
            }
            else
            {
                runtimeBaseValues.Add(type, newValue);
            }
        }
    }
}