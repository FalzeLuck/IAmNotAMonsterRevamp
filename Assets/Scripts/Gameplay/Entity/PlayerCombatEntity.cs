using UnityEngine;

namespace ShabuStudio.Gameplay
{
    public class PlayerCombatEntity : CombatEntity
    {
        public override void Heal(int amount)
        {
            base.Heal(amount);
            DamageTextManager.Instance.SpawnText(CombatManager.Instance.damageSpawnPointPlayer.position,amount,Color.green, false,"+");
        }
    }
}