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
        public static CombatManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }
        
        private CombatEntity _playerUnit;
        private CombatEntity _enemyUnit;
        private DamageTextManager _damageTextManager;
        
        [Header("Damage Spawn Point References")]
        public Transform damageSpawnPointPlayer;
        public Transform damageSpawnPointEnemy;

        [Header("Unit HealthBar")] 
        [SerializeField]private UnitHealthbar playerHealthbar;
        
        [Header("VFX References")]
        public VFXManager vfxManager;

        public void Initialize(DamageTextManager damageTextManager)
        {
            _playerUnit = BattleStateManager.Instance.playerUnit;
            _enemyUnit = BattleStateManager.Instance.enemyUnit;
            _damageTextManager = damageTextManager;
            playerHealthbar.SetTarget(_playerUnit);
        }



        public async UniTask<bool> PlayCard(ActionData actionData, CancellationToken token)
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

            if (card.canInflictBuff)
            {

                //Define Buff to Apply to target
                foreach (Buff buff in buffList)
                {
                    if (buff.target == Buff.BuffTarget.Target)
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
                    target.ApplyBuff(buffsToApplyToTarget, token),
                    self.ApplyBuff(buffsToApplyToSelf, token)
                );
            }



            //Prepare Damage
            int calculatedDamage = card.damage;

            if (card.conditionalDamageBonus != null && card.haveConditionalDamageBonus)
            {
                foreach (var bonus in card.conditionalDamageBonus)
                {
                    if (bonus.condition != null && bonus.condition.target == CardCondition.ConditionTarget.Self)
                    {
                        if (bonus.condition.IsConditionMet(self))
                            calculatedDamage += bonus.bonusDamageAmount;
                    }
            
                    if (bonus.condition != null && bonus.condition.target == CardCondition.ConditionTarget.Target)
                    {
                        if (bonus.condition.IsConditionMet(target))
                            calculatedDamage += bonus.bonusDamageAmount;
                    }
                }
            }

            int totalDamage = target.CalculateDamageTaken(self.CalculateDamageDealt(calculatedDamage));
            if (!card.canDealDamage) totalDamage = 0;
            float[] hitPattern = card.hitRatio;
            _damageTextManager.PrepareNumber(totalDamage, damageSpawnPoint, target, hitPattern);


            //Play VFX
            try
            {
                await vfxManager.PlayTimelineAsync(card,
                    target.vfxSpawnPoint,
                    self.vfxSpawnPoint,
                    _damageTextManager,
                    token);
            }
            finally
            {
                _damageTextManager.FlushRemainingDamage();
            }

            //Apply Damage
            if (card.canDealDamage) target.TakeDamage(totalDamage);

            //Update Ui
            self.UpdateUI();
            target.UpdateUI();






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

        public async UniTask OnStartTurn()
        {
            await _playerUnit.OnStartTurn();
            await _enemyUnit.OnStartTurn();
        }
    }
    
}