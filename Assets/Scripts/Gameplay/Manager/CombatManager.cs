using System;
using ShabuStudio.Data;
using UnityEngine;

namespace ShabuStudio.Gameplay
{
    public class CombatManager : MonoBehaviour
    {
        private CombatEntity _playerUnit;
        private CombatEntity _enemyUnit;

        public void Initialize()
        {
            _playerUnit = BattleStateManager.Instance.playerUnit;
            _enemyUnit = BattleStateManager.Instance.enemyUnit;
        }

        
        
        public void PlayCard(Action action,out bool isDead)
        {
            CardData card = action.cardData;
            if (card.cardType == CardType.Attack)
            {
                PlayAttack(card.cardDamage, action.owner);
            }

            if (action.owner == ActionOwner.Player && _enemyUnit.isDead)
            {
                isDead = true;
            }else if (action.owner == ActionOwner.Enemy && _playerUnit.isDead)
            {
                isDead = true;
            }
            else
            {
                isDead = false;
            }
        }

        void PlayAttack(int damage, ActionOwner owner)
        {
            if(owner == ActionOwner.Player)
            {
                _enemyUnit.TakeDamage(damage);
            }
            else _playerUnit.TakeDamage(damage);
        }
    }
    
}