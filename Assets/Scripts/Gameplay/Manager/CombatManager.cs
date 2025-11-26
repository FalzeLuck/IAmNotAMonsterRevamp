using System;
using ShabuStudio.Data;
using UnityEngine;

namespace ShabuStudio.Gameplay
{
    public class CombatManager : MonoBehaviour
    {
        private CombatEntity _playerUnit;
        private CombatEntity _enemyUnit;
        private DamageTextManager _damageTextManager;
        
        [Header("Damage Spawn Point References")]
        public Transform damageSpawnPointPlayer;

        public Transform damageSpawnPointEnemy;

        public void Initialize(DamageTextManager damageTextManager)
        {
            _playerUnit = BattleStateManager.Instance.playerUnit;
            _enemyUnit = BattleStateManager.Instance.enemyUnit;
            _damageTextManager = damageTextManager;
        }

        
        
        public void PlayCard(Action action,out bool isDead)
        {
            CardData card = action.cardData;
            if (card.cardType == CardType.AttackCard)
            {
                PlayAttack(card.cardAmount, action.ownerEntity);
            }
            else if (card.cardType == CardType.BuffCard)
            {
                PlayBuff(card,action.ownerEntity);
            }

            if (action.ownerEntity.unitType == ActionOwner.Player && _enemyUnit.isDead)
            {
                isDead = true;
            }
            else if (action.ownerEntity.unitType == ActionOwner.Enemy && _playerUnit.isDead)
            {
                isDead = true;
            }
            else
            {
                isDead = false;
            }
        }

        void PlayAttack(int damage, CombatEntity ownerEntity)
        {
            ActionOwner ownerType = ownerEntity.unitType;
            if(ownerType == ActionOwner.Player)
            {
                _enemyUnit.TakeDamage(damage);
                _damageTextManager.SpawnDamageText(damageSpawnPointEnemy.position, damage, false);
            }
            else
            {
                _playerUnit.TakeDamage(damage);
                _damageTextManager.SpawnDamageText(damageSpawnPointPlayer.position, damage, false);
            }
        }

        void PlayBuff(CardData card, CombatEntity ownerEntity)
        {
            ownerEntity.ApplyBuff(card.buffsToApply.list);
        }
    }
    
}