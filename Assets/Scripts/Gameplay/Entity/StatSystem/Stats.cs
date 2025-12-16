namespace ShabuStudio.Gameplay
{
    public enum StatsType
    {
        MaxHealth,
        AdditionalDamage,
        AdditionalTakenDamage
    }
    
    public class Stats
    {
        readonly StatsMediator statsMediator;
        readonly BaseStats baseStats;

        public StatsMediator Mediator => statsMediator;


        public int MaxHealth
        {
            get
            {
                var q = new Query(StatsType.MaxHealth, baseStats.maxHealth);
                statsMediator.PerformQuery(this, q);
                return q.Value;
            }
        }

        public int AdditionalTakenDamage
        {
            get
            {
                var q = new Query(StatsType.AdditionalTakenDamage, baseStats.additionalTakenDamage);
                statsMediator.PerformQuery(this, q);
                return q.Value;
            }
        }

        public int AdditionalDamage
        {
            get
            {
                var q = new Query(StatsType.AdditionalDamage, baseStats.additionalDamage);
                statsMediator.PerformQuery(this, q);
                return q.Value;
            }
        }

        
        public Stats(StatsMediator statsMediator, BaseStats baseStats)
        {
            this.statsMediator = statsMediator;
            this.baseStats = baseStats;
        }
    }
}