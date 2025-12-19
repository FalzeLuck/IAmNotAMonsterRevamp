using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
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

        [Header("Unit HealthBar")] 
        [SerializeField]private UnitHealthbar playerHealthbar;

        public void Initialize(DamageTextManager damageTextManager)
        {
            _playerUnit = BattleStateManager.Instance.playerUnit;
            _enemyUnit = BattleStateManager.Instance.enemyUnit;
            _damageTextManager = damageTextManager;
            playerHealthbar.SetTarget(_playerUnit);
        }

        
        
        public async UniTask<bool> PlayCard(ActionData actionData,CancellationToken token)
        {
            bool isDead = false;
            
            CardData card = actionData.cardData;
            ActionOwner ownerType = actionData.ownerEntity.unitType;
            List<Buff> buffList = card.buffsToApply.list;
            List<Buff> buffsToApplyToTarget = new List<Buff>();
            List<Buff> buffsToApplyToSelf = new List<Buff>();
            //Define Target
            CombatEntity target = null;
            CombatEntity self = null;
            Transform damageSpawnPoint = null;

            //Define target
            if (ownerType == ActionOwner.Player)
            {
                target = _enemyUnit;
                self = _playerUnit;
                damageSpawnPoint = damageSpawnPointEnemy;
            }
            else if (ownerType == ActionOwner.Enemy)
            {
                target = _playerUnit;
                self = _enemyUnit;
                damageSpawnPoint = damageSpawnPointPlayer;
            }

            //Define Buff to Apply to target
            foreach (Buff buff in buffList)
            {
                if (buff.target == Buff.BuffTarget.Enemy)
                {
                    buffsToApplyToTarget.Add(buff);
                }
            }
            
            //Define Buff to Apply to target
            foreach (Buff buff in buffList)
            {
                if (buff.target == Buff.BuffTarget.Self)
                {
                    buffsToApplyToSelf.Add(buff);
                }
            }

            
            //Start Action
            await UniTask.WhenAll(
                    target.ApplyBuff(buffsToApplyToTarget,token),
                    self.ApplyBuff(buffsToApplyToSelf,token)
                );
            target.TakeDamage(card.damage + self.Stats.AdditionalDamage, out var damage);
            _damageTextManager.SpawnDamageText(damageSpawnPoint.position,damage,false);
            
            
            
            
            
            

            if (actionData.ownerEntity.unitType == ActionOwner.Player && _enemyUnit.isDead)
            {
                isDead = true;
            }
            else if (actionData.ownerEntity.unitType == ActionOwner.Enemy && _playerUnit.isDead)
            {
                isDead = true;
            }
            else
            {
                isDead = false;
            }
            
            return isDead;
        }

        public void OnStartTurn()
        {
            _enemyUnit.OnStartTurn();
            _playerUnit.OnStartTurn();
        }
    }
    
}