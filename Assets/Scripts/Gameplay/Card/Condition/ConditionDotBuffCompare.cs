using ShabuStudio.Gameplay;
using ShabuStudio.Gameplay.DoTSystem;
using UnityEngine;

namespace Gameplay.Card.Condition
{
    [CreateAssetMenu(menuName = "Cards/Conditions/Buff Compare")]
    public class ConditionDotBuffCompare : CardCondition
    {
        [SerializeField]private DoT.DotType dotType;

        public override bool IsConditionMet(CombatEntity entity)
        {
            if (entity == null) return false;
            
            return entity.HaveThisDot(dotType);
        }
    }
}