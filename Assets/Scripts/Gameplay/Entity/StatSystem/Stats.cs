namespace ShabuStudio.Gameplay
{
    public enum StatsType
    {
        MaxHealth,
        MaxCost
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

        public int MaxCost
        {
            get
            {
                var q = new Query(StatsType.MaxCost, baseStats.maxCost);
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