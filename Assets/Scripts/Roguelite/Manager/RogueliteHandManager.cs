using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Roguelite;
using ShabuStudio.Data;
using UnityEngine;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

namespace ShabuStudio.Gameplay
{
    public class RogueliteHandManager : HandManager
    {
        //Draw Card to hand.
        protected override void DrawCard()
        {
            if(handCards.Count >= RogueliteBattleStateManager.Instance.playerUnit.Stats.MaxHandSize) return;

            bool haveAttack = IsHandCardsHaveType(CardType.Attack);
            Debug.Log(haveAttack);
            
            //Prepare card to initialize
            CardDisplay newCard = Instantiate(cardPrefab, cardSpawnPoint.position, cardSpawnPoint.rotation,splineContainer.transform);
            handCards.Add(newCard.gameObject);
            
            //Set Sort Order
            if (lastCardOnTop)
            {
                if(!haveAttack)
                {
                    newCard.InitializeCard(deckManager.playerDeck.DrawRandomCardFixedType(CardType.Attack),
                        newCard.transform.GetSiblingIndex());
                }
                else
                {
                    newCard.InitializeCard(deckManager.playerDeck.DrawRandomCard(),
                        newCard.transform.GetSiblingIndex());
                }
            }
            else
            {
                if(!haveAttack)
                {
                    newCard.InitializeCard(deckManager.playerDeck.DrawRandomCardFixedType(CardType.Attack),
                        -newCard.transform.GetSiblingIndex());
                }
                else
                {
                    newCard.InitializeCard(deckManager.playerDeck.DrawRandomCard(),
                        -newCard.transform.GetSiblingIndex());
                }
            }
            UpdateCardPositions();
        }

        private bool IsHandCardsHaveType(CardType type)
        {
            if (handCards.Count <= 0) return false;
            bool handHaveType = false;
            foreach (GameObject card in handCards)
            {
                CardDisplay cardDisplay = card.GetComponent<CardDisplay>();
                if (cardDisplay != null)
                {
                    if (cardDisplay.cardData.cardType == type) handHaveType = true;
                }
            }
            
            return handHaveType;
        }

        //Draw Card to hand until max hand size reached.
        public override async UniTask DrawCardToMax()
        {
            if(handCards.Count >= RogueliteBattleStateManager.Instance.playerUnit.Stats.MaxHandSize) return;

            while (handCards.Count < RogueliteBattleStateManager.Instance.playerUnit.Stats.MaxHandSize)
            {
                DrawCard();
                await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
            }
            
            await drawCardTween.ToUniTask();
        }
        
        //Update Card Position in hand.
        protected override void UpdateCardPositions()
        {
            if(handCards.Count <= 0) return;

            float cardSpacing = 1f / RogueliteBattleStateManager.Instance.playerUnit.Stats.MaxHandSize;
            float firstCardPosition = 0.5f - (handCards.Count - 1) * cardSpacing / 2;
            Spline spline = splineContainer.Spline;
            for (int i = 0; i < handCards.Count; i++)
            {
                CardMovement cardMovement = handCards[i].GetComponent<CardMovement>();
                
                //Calculate world position and rotation with spline
                float p = firstCardPosition + i * cardSpacing;
                Vector3 splinePosition = spline.EvaluatePosition(p);
                Vector3 forward = spline.EvaluateTangent(p);
                Vector3 up = spline.EvaluateUpVector(p);
                Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);
                
                //Calculate local position and rotation of card.
                drawCardTween = handCards[i].transform.DOLocalMove(splinePosition, 0.25f).OnComplete(
                    () => cardMovement.UpdateLocal());
                handCards[i].transform.DOLocalRotateQuaternion(rotation, 0.25f);
            }
            SetHandCardInteractable(false);
        }

        public void SetHandCardInteractable(bool interactable)
        {
            foreach (GameObject card in handCards)
            {
                card.GetComponent<CardMovement>().SetInteractable(interactable);
            }
        }
        
        //Hide hand card
        public void HideHand(bool hide)
        { 
            if (hide)
            {
                Vector3 handPos = defaultHandPostion + hideHandOffset;
                splineContainer.transform.DOMove(handPos, 0.25f).SetEase(Ease.OutBack);
            }
            else
            {
                splineContainer.transform.DOMove(defaultHandPostion, 0.25f).SetEase(Ease.OutBack);
            }
        }

        public void RemovePlayedCard()
        {
            //Reverse loop to prevent error
            for (int i = handCards.Count - 1; i >= 0; i--)
            {
                GameObject card = handCards[i];
                CardDisplay cardDisplay = card.GetComponent<CardDisplay>();

                if (cardDisplay.isPlayed)
                {
                    handCards.RemoveAt(i);
                    cardDisplay.RemoveCard();
                }
            }
            UpdateCardPositions();
        }

    }
}