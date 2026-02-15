using System;
using System.Collections.Generic;
using Roguelite;
using ShabuStudio.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShabuStudio.Gameplay
{
    public class RogueliteDeckManager : DeckManager
    {
        public override void Initialize()
        {
            RogueliteBattleStateManager.Instance.playerUnit.InitializeDeck();
            playerDeck = RogueliteBattleStateManager.Instance.playerUnit.deckDataHolder;
            
            RogueliteBattleStateManager.Instance.enemyUnit.InitializeDeck();
            enemyDeck = RogueliteBattleStateManager.Instance.enemyUnit.deckDataHolder;
        }
    }
}