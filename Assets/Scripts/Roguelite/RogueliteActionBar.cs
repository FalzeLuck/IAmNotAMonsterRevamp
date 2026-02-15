using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Roguelite;
using ShabuStudio.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShabuStudio.Gameplay
{
    public class RogueliteActionBar : ActionBar
    {
        public static RogueliteActionBar Instance { get; private set; }
        

        void UpdateText(CombatEntity entity)
        {
            if(entity != null && entity.unitType == ActionOwner.Player)
            {
                actionCostText.text = $"Cost : {entity.currentCost}";
            }
        }

        public override void UpdateUI()
        {
            UpdateText(RogueliteBattleStateManager.Instance.playerUnit);
            UpdateText(RogueliteBattleStateManager.Instance.enemyUnit);
        }
        
    }
}