using UnityEngine;

namespace ShabuStudio.Gameplay
{
    [CreateAssetMenu(menuName = "Cards/Conditions/Target Low Health")]
    public class ConditionTargetLowHealth : CardCondition
    {
        [Range(0f, 1f)] public float healthPercentage = 0.5f;

        public override bool IsConditionMet(CombatEntity entity)
        {
            if (entity == null ) return false;

            float percentage = (float)entity.currentHealth / entity.Stats.MaxHealth;
            return percentage <= healthPercentage;
        }
    }
}