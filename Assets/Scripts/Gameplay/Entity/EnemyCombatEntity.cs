using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ShabuStudio.Data;
using UnityEngine;

namespace ShabuStudio.Gameplay
{
    public class EnemyCombatEntity : CombatEntity
    {
        [Header("UI")] public Sprite enemyIcon;
        [Header("Enemy References")]
        public Animator animator;
        public int fixedCost = 3;
        [Header("Enemy Hand")]
        public List<CardData> hand;


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
        
        public async UniTask StartActionSetup(RogueliteActionBar actionBar)
        {
            DrawCardToMax();
            await InsertCardToActionBar(actionBar);
        }

        void DrawCardToMax()
        {
            if(hand.Count >= Stats.MaxHandSize) return;
            
            while (hand.Count < Stats.MaxHandSize)
            {
                DrawCard();
            }
        }

        void DrawCard()
        {
            hand.Add(deckDataHolder.DrawRandomCard());
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
        }
        
        public override void Heal(int amount)
        {
            base.Heal(amount);
            DamageTextManager.Instance.SpawnText(CombatManager.Instance.damageSpawnPointEnemy.position,amount,Color.green, false,"+");
        }
        

        public void PlayTrigger(string trigger)
        {
            animator.SetTrigger(trigger);
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
        
        async UniTask InsertCardToActionBar(RogueliteActionBar actionBar)
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

        protected override void Die()
        {
            base.Die();
            Destroy(gameObject);
        }
    }
}