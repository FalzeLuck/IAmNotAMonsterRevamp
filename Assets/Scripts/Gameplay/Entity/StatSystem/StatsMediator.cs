using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace ShabuStudio.Gameplay
{
    public class StatsMediator
    {
        private readonly LinkedList<StatsModifier> modifiers = new();
        
        public event EventHandler<Query> Queries;
        public void PerformQuery(object sender, Query query) => Queries?.Invoke(sender, query);

        public void AddModifier(StatsModifier modifier)
        {
            modifiers.AddLast(modifier);
            Queries += modifier.Handle;
            

            modifier.OnDispose += _ =>
            {
                modifiers.Remove(modifier);
                Queries -= modifier.Handle;
            };
        }


        public void DecreaseTurnCountdown(int value)
        {
            var node = modifiers.First;
            while (node != null)
            {
                var modifier = node.Value;
                modifier.DecreaseTurnCountdown(value);
                node = node.Next;
            }
            
            //Dispose any that are mark for removel.
            node = modifiers.First;
            while (node != null)
            {
                var nextNode = node.Next;

                if (node.Value.MarkedForRemoval)
                {
                    node.Value.Dispose();
                }
                
                node = nextNode;
            }
        }
    }

    public class Query
    {
        public readonly StatsType StatsType;
        public float Value;
        
        public Query(StatsType statsType, float value)
        {
            StatsType = statsType;
            Value = value;
        }
    }
}