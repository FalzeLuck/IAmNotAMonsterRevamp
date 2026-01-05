using UnityEngine;

namespace ShabuStudio.Gameplay
{
    public abstract class CardCondition :ScriptableObject
    {
        public enum ConditionTarget
        {
            Self,
            Target
        }
        public ConditionTarget target;
        
        public abstract bool IsConditionMet(CombatEntity entity);
    }
    
    
    
    
}