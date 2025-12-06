using System;

namespace ShabuStudio.Gameplay
{
    public class BasicStatModifier : StatsModifier
    {
        public readonly StatsType type;
        readonly Func<int, int> operation;

        public BasicStatModifier(StatsType type, int countdownTurn, Func<int, int> operation) : base(countdownTurn)
        {
            this.type = type;
            this.operation = operation;
        }

        public override void Handle(object sender, Query query)
        {
            if (query.StatsType == type)
            {
                query.Value = operation(query.Value);
            }
        }
    }
    public abstract class StatsModifier : IDisposable
    {
        public bool MarkedForRemoval { get; private set; }
        
        public event Action<StatsModifier> OnDispose = delegate { };

        private int countdownTurn;
        
        protected StatsModifier(int countdownTurn)
        {
            if(countdownTurn <= 0) return;
            
            this.countdownTurn = countdownTurn;
        }

        public void DecreaseTurnCountdown(int value)
        {
            if (countdownTurn > 0)
            {
                countdownTurn -= value;
            }

            if (countdownTurn <= 0)
            {
                MarkedForRemoval = true;
            }
        }
        
        public abstract void Handle(object sender, Query query);
        
        public void Dispose()
        {
            OnDispose.Invoke(this);
        }
    }
}