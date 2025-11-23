using System;
using System.Collections.Generic;
using ShabuStudio.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShabuStudio.Gameplay
{
    public class DeckManager : MonoBehaviour
    {
        public DeckData playerDeck;
        public DeckData enemyDeck;

        public void Initialize()
        {
            BattleStateManager.Instance.playerUnit.InitializeDeck();
            playerDeck = BattleStateManager.Instance.playerUnit.deckData;
            
            BattleStateManager.Instance.enemyUnit.InitializeDeck();
            enemyDeck = BattleStateManager.Instance.enemyUnit.deckData;
        }
    }
}