using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ShabuStudio.Data;
using UnityEngine;
using UnityEngine.Splines;

namespace ShabuStudio.Gameplay
{
    public class HandManager : MonoBehaviour
    {
        [SerializeField] private int maxHandSize = 5;
        [SerializeField] private CardDisplay cardPrefab;
        [SerializeField] private SplineContainer splineContainer;
        [SerializeField] private Transform cardSpawnPoint;
        
        private List<GameObject> handCards = new List<GameObject>();
        [Tooltip("True: Last card is on top. False: First card is on top.")]
        public bool lastCardOnTop = true;

        //For exchanging Data from deck.
        private DeckManager deckManager;

        //Tween to wait for draw card.
        Tween drawCardTween;

        public void Initialize(DeckManager deckManager)
        {
            this.deckManager = deckManager;
            SetHandCardInteractable(false);
        }

        
        //Draw Card to hand.
        private void DrawCard()
        {
            if(handCards.Count >= maxHandSize) return;
            CardDisplay newCard = Instantiate(cardPrefab, cardSpawnPoint.position, cardSpawnPoint.rotation,splineContainer.transform);
            handCards.Add(newCard.gameObject);
            
            //Set Sort Order
            if (lastCardOnTop)
            {
                newCard.InitializeCard(deckManager.playerDeck.DrawRandomCard(),newCard.transform.GetSiblingIndex());
            }
            else
            {
                newCard.InitializeCard(deckManager.playerDeck.DrawRandomCard(),-newCard.transform.GetSiblingIndex());
            }
            UpdateCardPositions();
        }

        //Draw Card to hand until max hand size reached.
        public IEnumerator DrawCardToMax()
        {
            if(handCards.Count >= maxHandSize) yield break;

            while (handCards.Count < maxHandSize)
            {
                DrawCard();
                yield return new WaitForSeconds(0.1f);
            }
            
            yield return drawCardTween.WaitForCompletion();
        }
        
        //Update Card Position in hand.
        private void UpdateCardPositions()
        {
            if(handCards.Count <= 0) return;

            float cardSpacing = 1f / maxHandSize;
            float firstCardPosition = 0.5f - (handCards.Count - 1) * cardSpacing / 2;
            Spline spline = splineContainer.Spline;
            for (int i = 0; i < handCards.Count; i++)
            {
                CardMovement cardMovement = handCards[i].GetComponent<CardMovement>();
                
                float p = firstCardPosition + i * cardSpacing;
                Vector3 splinePosition = spline.EvaluatePosition(p);
                Vector3 forward = spline.EvaluateTangent(p);
                Vector3 up = spline.EvaluateUpVector(p);
                Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);
                drawCardTween = handCards[i].transform.DOMove(splinePosition + splineContainer.transform.position, 0.25f).OnComplete(
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
            
                    Destroy(card);
                }
            }
            UpdateCardPositions();
        }

    }
}