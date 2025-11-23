using System.Collections;
using System.Collections.Generic;
using ShabuStudio.Data;
using UnityEngine;

namespace ShabuStudio.Gameplay
{
    public class EnemyCombatEntity : CombatEntity
    {
        [Header("Enemy Hand")]
        public List<CardData> hand;
        public int handSize = 5;
        
        //-------------------------
        // Enemy Will Draw card then put it in action bar
        //--------------------------
        public IEnumerator StartActionSetup(ActionBar actionBar)
        {
            yield return StartCoroutine(DrawCardToMax());
            yield return StartCoroutine(InsertCardToActionBar(actionBar));
        }

        IEnumerator DrawCardToMax()
        {
            if(hand.Count >= handSize) yield break;
            
            while (hand.Count < handSize)
            {
                DrawCard();
            }
        }

        void DrawCard()
        {
            //Draw card from deck
            hand.Add(deckData.DrawRandomCard());
        }


        IEnumerator InsertCardToActionBar(ActionBar actionBar)
        {
            while (hand.Count > 0)
            {
                int index = Random.Range(0, hand.Count);
                CardData card = hand[index];
                int speed = Random.Range(card.cardMinSpeed, card.cardMaxSpeed + 1);

                if (!actionBar.InsertCard(card, speed, ActionOwner.Enemy))
                {
                    yield break;
                }

                hand.RemoveAt(index);
            }

            yield return null;
        }
    }
}