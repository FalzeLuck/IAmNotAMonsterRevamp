using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ShabuStudio.Gameplay.DoTSystem;
using UnityEngine;
using UnityEngine.UI;

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
            Target
        }
        
        public string buffName;
        public string buffDescription;
        public Sprite buffIcon;
        public int buffValue;
        public int buffTurnsToEnd;
        public BuffTarget target;
        public BuffType buffType;
        public GameObject vfxPrefab;

        public virtual async UniTask ApplyBuff(CombatEntity buffTarget, CancellationToken token)
        {
            if (vfxPrefab != null)
            {
                await VFXManager.Instance.PlayTimelineAsync(vfxPrefab,buffTarget.vfxSpawnPoint,DamageTextManager.Instance, token);
            }
            buffTarget.UpdateBuffPanel(this);
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

        public override async UniTask ApplyBuff(CombatEntity buffTarget, CancellationToken token)
        {
            await base.ApplyBuff(buffTarget, token);
            StatsModifier modifier = operatorType switch
            {
                OperatorType.Add => new BasicStatModifier(type, buffTurnsToEnd, x => x + buffValue),
                OperatorType.Multiply => new BasicStatModifier(type, buffTurnsToEnd, x => x * buffValue),
                _ => throw new System.Exception("Invalid operator type")
            };
            
            buffTarget.Stats.Mediator.AddModifier(modifier);
        }
    }
    
    [Serializable]
    public class CostModifierBuff : Buff
    {
        public override async UniTask ApplyBuff(CombatEntity buffTarget,CancellationToken token)
        {
            await base.ApplyBuff(buffTarget, token);
            if (buffValue > 0)
            {
                buffTarget.AddCost(buffValue);
            }
            else if (buffValue < 0)
            {
                buffTarget.RemoveCost(Math.Abs(buffValue));
            }
        }
    }

    [Serializable]
    public class HealBuff : Buff
    {
        public override async UniTask ApplyBuff(CombatEntity buffTarget,CancellationToken token)
        {
            if(buffValue < 0) return;
            await base.ApplyBuff(buffTarget, token);
            
            buffTarget.Heal(buffValue);
        }
    }

    [Serializable]
    public class SpeedBuff : Buff
    {
        public override async UniTask ApplyBuff(CombatEntity buffTarget, CancellationToken token)
        {
            if(buffValue == 0) return;
            await base.ApplyBuff(buffTarget, token);
            
            ActionDisplay.AddActionDataSpeed(buffValue, buffTarget.unitType);
            await ActionBar.Instance.RearrangeAction(token);
        }
    }
    
    
    [Serializable]
    public class DotBuff : Buff
    {
        [Header("DoT Properties")]
        public Color dotColor;
        public DoT.DotType dotType;
        public override async UniTask ApplyBuff(CombatEntity buffTarget, CancellationToken token)
        {
            if(buffValue == 0) return;
            await base.ApplyBuff(buffTarget, token);

            DoT doT = new DoT(buffValue, buffTurnsToEnd, dotColor, dotType);
            
            await buffTarget.ApplyDoT(doT,token);
        }
    }
    
    [Serializable]
    public class ConditionBuff : Buff
    {
        [BF_SubclassList.SubclassList(typeof(Buff)), SerializeField]public Buff_container buffsToApplyOnConditionMet;
        [BF_SubclassList.SubclassList(typeof(Buff)), SerializeField]public Buff_container buffsToApplyOnConditionNotMet;
        [Header("Condition Properties")]
        public CardCondition condition;
        
        public override async UniTask ApplyBuff(CombatEntity buffTarget, CancellationToken token)
        {
            CombatEntity conditionCheckTarget = null;
            
            if (condition.target == CardCondition.ConditionTarget.Self && buffTarget is PlayerCombatEntity)
            {
                conditionCheckTarget = buffTarget;
            }
            else if (condition.target == CardCondition.ConditionTarget.Target && buffTarget is PlayerCombatEntity)
            {
                conditionCheckTarget = BattleStateManager.Instance.enemyUnit;
            }
            else if (condition.target == CardCondition.ConditionTarget.Self && buffTarget is EnemyCombatEntity)
            {
                conditionCheckTarget = buffTarget;
            }
            else if (condition.target == CardCondition.ConditionTarget.Target && buffTarget is EnemyCombatEntity)
            {
                conditionCheckTarget = BattleStateManager.Instance.playerUnit;
            }

            if (conditionCheckTarget != null)
            {
                Debug.Log(conditionCheckTarget.name);
                if (condition.IsConditionMet(conditionCheckTarget))
                {
                    foreach (var buff in buffsToApplyOnConditionMet.list)
                    {
                        //Show buff animation if can
                        await buff.ApplyBuff(buffTarget,token);
                    }
                }
                else
                {
                    foreach (var buff in buffsToApplyOnConditionNotMet.list)
                    {
                        //Show buff animation if can
                        await buff.ApplyBuff(buffTarget,token);
                    }
                }
            }
            
            //Final Buff animation
            await base.ApplyBuff(buffTarget, token);
        }
    }
    
}