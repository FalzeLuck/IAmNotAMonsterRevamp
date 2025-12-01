using System;
using System.Collections.Generic;
using ShabuStudio.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShabuStudio.Gameplay
{
    public class DeckManager : MonoBehaviour
    {
        public DeckDataHolder playerDeck;
        public DeckDataHolder enemyDeck;

        public void Initialize()
        {
            BattleStateManager.Instance.playerUnit.InitializeDeck();
            playerDeck = BattleStateManager.Instance.playerUnit.deckDataHolder;
            
            BattleStateManager.Instance.enemyUnit.InitializeDeck();
            enemyDeck = BattleStateManager.Instance.enemyUnit.deckDataHolder;
        }
    }
}