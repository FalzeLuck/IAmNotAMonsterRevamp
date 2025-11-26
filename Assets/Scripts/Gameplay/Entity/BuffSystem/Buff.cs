using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShabuStudio.Gameplay
{
    [Serializable]
    public abstract class Buff
    {
        public string buffName = "TestBuff";
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
        public enum OperatorType
        {
            Add,
            Multiply
        }

        [SerializeField] private StatsType type = StatsType.MaxHealth;
        [SerializeField] private OperatorType operatorType = OperatorType.Add;
        [SerializeField] private int value = 10;
        [SerializeField] private int countdownTurn = 1;

        public override void ApplyBuff(CombatEntity entity)
        {
            StatsModifier modifier = operatorType switch
            {
                OperatorType.Add => new BasicStatModifier(type, countdownTurn, x => x + value),
                OperatorType.Multiply => new BasicStatModifier(type, countdownTurn, x => x * value),
                _ => throw new System.Exception("Invalid operator type")
            };
            
            entity.Stats.Mediator.AddModifier(modifier);
        }
    }
    
}