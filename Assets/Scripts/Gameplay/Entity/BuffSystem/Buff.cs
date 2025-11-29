using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShabuStudio.Gameplay
{
    [Serializable]
    public abstract class Buff
    {
        public enum BuffType
        {
            OnAction,
            OnTurnStart,
        }
        
        public enum BuffTarget
        {
            Self,
            Enemy
        }

        [System.Flags]
        public enum BuffAffect
        {
            None = 0,
            Entity = 1,
            ActionValue = 2,
            UI = 4,
            
            //helper to select all
            All = ~0
        }
        

        public int buffValue;
        public BuffAffect affectTarget; // Flags to select what things this buff is doing too.
        public BuffTarget target;
        public BuffType buffType;
        public abstract void ApplyBuff(CombatEntity entity);
    }

    [Serializable]
    public class Buff_container
    {
        [SerializeReference] public List<Buff> list;
    }
    
    [Serializable]
    public class StatsModifierBuff : Buff
    {
        private enum OperatorType
        {
            Add,
            Multiply
        }

        [SerializeField] private StatsType type = StatsType.MaxHealth;
        [SerializeField] private OperatorType operatorType = OperatorType.Add;
        [SerializeField] private int countdownTurn = 1;

        public override void ApplyBuff(CombatEntity entity)
        {
            StatsModifier modifier = operatorType switch
            {
                OperatorType.Add => new BasicStatModifier(type, countdownTurn, x => x + buffValue),
                OperatorType.Multiply => new BasicStatModifier(type, countdownTurn, x => x * buffValue),
                _ => throw new System.Exception("Invalid operator type")
            };
            
            entity.Stats.Mediator.AddModifier(modifier);
        }
    }
    
    [Serializable]
    public class CostModifierBuff : Buff
    {
        public override void ApplyBuff(CombatEntity entity)
        {
            if (buffValue > 0)
            {
                entity.AddCost(buffValue);
            }
            else if (buffValue < 0)
            {
                entity.RemoveCost(Math.Abs(buffValue));
            }
        }
    }

    [Serializable]
    public class HealBuff : Buff
    {
        public override void ApplyBuff(CombatEntity entity)
        {
            if(buffValue < 0) return;
            
            entity.Heal(buffValue);
        }
    }
    
}