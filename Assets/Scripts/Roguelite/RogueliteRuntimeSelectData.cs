using ShabuStudio.Gameplay;
using UnityEngine;

namespace Roguelite
{
    [CreateAssetMenu(fileName = "RogueliteBuffData", menuName = "Roguelite/SelectData")]
    public class RogueliteRuntimeSelectData : ScriptableObject
    {
        public string buffText;
        public RogueliteStatsType statsType;
        public int amount;

        public void ApplyEffect(RogueliteRunState runState)
        {
            switch (statsType)
            {
                case RogueliteStatsType.MaxHealth: 
                    runState.currentPlayerEntity.Stats.MaxHealth += amount;
                    runState.currentPlayerEntity.Heal(amount);
                    break;
                case RogueliteStatsType.MaxHandCard: runState.currentPlayerEntity.Stats.MaxHandSize += amount; break;
                case RogueliteStatsType.AddingDamage: runState.currentPlayerEntity.Stats.AdditionalDamage += amount; break;
                default:break;
            }
            
        }
    }

    public enum RogueliteStatsType
    {
        MaxHealth,
        MaxHandCard,
        AddingDamage
    }
}