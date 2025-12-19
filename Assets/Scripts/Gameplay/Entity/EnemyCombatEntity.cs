using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ShabuStudio.Data;
using UnityEngine;

namespace ShabuStudio.Gameplay
{
    public class EnemyCombatEntity : CombatEntity
    {
        [Header("Enemy References")]
        public Animator animator;
        public int fixedCost = 3;
        [Header("Enemy Hand")]
        public List<CardData> hand;
        public int handSize = 5;


        protected override void Start()
        {
            base.Start();
        }

        //-------------------------
        // Enemy Will Draw card then put it in action bar
        //--------------------------
        public async UniTask StartActionSetup(ActionBar actionBar)
        {
            DrawCardToMax();
            await InsertCardToActionBar(actionBar);
        }

        void DrawCardToMax()
        {
            if(hand.Count >= handSize) return;
            
            while (hand.Count < handSize)
            {
                DrawCard();
            }
        }

        void DrawCard()
        {
            hand.Add(deckDataHolder.DrawRandomCard());
        }

        public override void TakeDamage(int damage, out int uiDamage)
        {
            base.TakeDamage(damage, out uiDamage);
            if(damage > 0)
                animator.SetTrigger("Hit");
        }


        async UniTask InsertCardToActionBar(ActionBar actionBar)
        {
            while (hand.Count > 0)
            {
                int index = Random.Range(0, hand.Count);
                CardData card = hand[index];
                int speed = card.cardSpeed;

                if (!actionBar.InsertCard(card, speed, this, out _))
                {
                    return;
                }

                hand.RemoveAt(index);
            }

            await UniTask.NextFrame();
        }
    }
}