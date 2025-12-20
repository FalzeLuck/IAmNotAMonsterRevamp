using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
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
        
        public string buffName;
        public int buffValue;
        public BuffAffect affectTarget; // Flags to select what things this buff is doing too.
        public BuffTarget target;
        public BuffType buffType;
        public GameObject vfxPrefab;

        public virtual async UniTask ApplyBuff(CombatEntity entity, CancellationToken token)
        {
            if (vfxPrefab != null)
            {
                await VFXManager.Instance.PlayTimelineAsync(vfxPrefab,entity.vfxSpawnPoint,DamageTextManager.Instance, token);
            }
        }
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

        public override async UniTask ApplyBuff(CombatEntity entity, CancellationToken token)
        {
            await base.ApplyBuff(entity, token);
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
        public override async UniTask ApplyBuff(CombatEntity entity,CancellationToken token)
        {
            await base.ApplyBuff(entity, token);
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
        public override async UniTask ApplyBuff(CombatEntity entity,CancellationToken token)
        {
            if(buffValue < 0) return;
            await base.ApplyBuff(entity, token);
            
            entity.Heal(buffValue);
        }
    }

    [Serializable]
    public class RequireConditionBuff : Buff
    {
        private enum BuffConditionType
        {
            MaxHealth,
            AdditionalDamage,
            AdditionalTakenDamage,
        
            // -- EXCLUSIVE TYPE --
            CurrentCost, 
        }
        
        private enum ConditionType
        {
            Equal,
            NotEqual,
            MoreThanOrEqual,
            MoreThan,
            LessThanOrEqual,
            LessThan,
        }
        
        [Header( "Condition" )]
        [SerializeField] private BuffConditionType conditionStatsType = BuffConditionType.MaxHealth;
        [SerializeField] private ConditionType conditionType = ConditionType.Equal;
        [SerializeField] private int conditionValue = 0;
        [BF_SubclassList.SubclassList(typeof(Buff)), SerializeField]public Buff_container buffsToApply;
        
        public override async UniTask ApplyBuff(CombatEntity entity,CancellationToken token)
        {
            if (CheckCondition(entity))
            {
                if (buffsToApply != null && buffsToApply.list.Count > 0)
                {
                    await entity.ApplyBuff(buffsToApply.list, token);
                }
            }
        }

        bool CheckCondition(CombatEntity entity)
        {
            var stats = entity.Stats;
            if(stats == null) return false;
            

            int valueToCheck = 0;

            switch (conditionStatsType)
            {
                case BuffConditionType.MaxHealth:
                    valueToCheck = stats.MaxHealth;
                    break;
                case BuffConditionType.AdditionalDamage:
                    valueToCheck = stats.AdditionalDamage;
                    break;
                case BuffConditionType.AdditionalTakenDamage:
                    valueToCheck = stats.AdditionalTakenDamage;
                    break;
                case BuffConditionType.CurrentCost:
                    valueToCheck = entity.currentCost;
                    break;
            }
            
            
            
            switch (conditionType)
            {
                case ConditionType.Equal:
                    return valueToCheck == conditionValue;
                case ConditionType.NotEqual:
                    return valueToCheck != conditionValue;
                case ConditionType.MoreThanOrEqual:
                    return valueToCheck >= conditionValue;
                case ConditionType.MoreThan:
                    return valueToCheck > conditionValue;
                case ConditionType.LessThanOrEqual:
                    return valueToCheck <= conditionValue;
                case ConditionType.LessThan:
                    return valueToCheck < conditionValue;
                default:
                    return false;
            }
        }
    }
    
}