using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Roguelite;
using ShabuStudio.Data;
using UnityEngine;

namespace ShabuStudio.Gameplay
{
    public class RogueliteCombatManager : CombatManager
    {
        public static RogueliteCombatManager Instance { get; private set; }

        public override void Initialize(DamageTextManager damageTextManager)
        {
            _playerUnit = RogueliteBattleStateManager.Instance.playerUnit;
            _enemyUnit = RogueliteBattleStateManager.Instance.enemyUnit;
            _damageTextManager = damageTextManager;
            playerHealthbar.SetTarget(_playerUnit);
        }
    }
    
}