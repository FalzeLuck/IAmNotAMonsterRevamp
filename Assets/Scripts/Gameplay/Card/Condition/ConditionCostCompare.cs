using UnityEngine;

namespace ShabuStudio.Gameplay.Condition
{
    [CreateAssetMenu(menuName = "Cards/Conditions/Cost Compare")]
    public class ConditionCostCompare : CardCondition
    {
        private enum OperatorType
        {
            LessThan, GreaterThan, Equal
        }

        [SerializeField] private OperatorType operatorType = OperatorType.Equal;
        [SerializeField][Min(0)] private int compareValue = 0;

        public override bool IsConditionMet(CombatEntity entity)
        {
            if (entity == null ) return false;

            
            switch (operatorType)
            {
                case OperatorType.Equal: return entity.currentCost == compareValue;
                case OperatorType.LessThan: return entity.currentCost < compareValue;
                case OperatorType.GreaterThan: return entity.currentCost > compareValue;
                default: return false;
            }
        }
    }
}