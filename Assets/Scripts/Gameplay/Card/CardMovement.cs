using System;
using DG.Tweening;
using Roguelite;
using ShabuStudio.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ShabuStudio.Gameplay
{
    [RequireComponent(typeof(CardDisplay))]
    public class CardMovement : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private int _currentState = 0;
        private Vector3 _startLocalScale;
        [SerializeField] private Vector3 _hoverLocalScale = new Vector3(1.1f, 1.1f, 1.1f);
        private float _startYPosition;
        [SerializeField] private float _hoverYOffset = 0.1f;
        [SerializeField] private Vector3 _cannotClickOffset = new Vector3(10f, 10f, 10f);
        [SerializeField] private float _transitionSpeed = 0.3f;
        [SerializeField] private CardDisplay cardDisplay;
        [SerializeField] private ActionDisplay _actionDisplay;
        [SerializeField] private Image cardHighlight;
        [SerializeField] private Color selectedColor;
        [SerializeField] private Color canSelectColor;
        [SerializeField] private Color cannotSelectColor;
        private CardData cardData;
        private ActionBar actionBar;

        public int defaultCardOrder = 0;
        
        //Locking Card
        private bool _isLocked = false;
        public bool isInteractable = true;

        private void Start()
        {
            _startLocalScale = transform.localScale;
            _startYPosition = transform.localPosition.y;
            _currentState = 0;
            
            cardDisplay = GetComponent<CardDisplay>();
            cardData = cardDisplay.cardData;
            actionBar = FindFirstObjectByType<ActionBar>();

            cardHighlight.DOFade(0f, 0.0f);
        }


        
        public void UpdateLocal()
        {
            _startLocalScale = transform.localScale;
            _startYPosition = transform.localPosition.y;
        }

        public void SetInteractable(bool value)
        {
            isInteractable = value;
        }
        

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!isInteractable) return;
            
            CardDetailDisplay.CurrentCardData = cardData;
            
            //Action Display relate to this card
            if (_isLocked)
            {
                _actionDisplay?.StartHighlight();
            }
            
            if (_currentState == 0)
            {
                cardDisplay.UpdateCardOrder(999 + defaultCardOrder);
                transform.DOScale(_hoverLocalScale, _transitionSpeed).SetEase(Ease.OutBack);

                if (BattleStateManager.Instance?.playerUnit.currentCost - cardData.cardCost >= 0 || RogueliteBattleStateManager.Instance?.playerUnit.currentCost - cardData.cardCost >= 0)
                {
                    cardHighlight.color = canSelectColor;
                }
                else
                {
                    cardHighlight.color = cannotSelectColor;
                }
                cardHighlight.DOKill();
                cardHighlight.DOFade(1f, 0.5f);
                
                
                _currentState = 1;
                
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isInteractable) return;
            
            CardDetailDisplay.CurrentCardData = null;
            
            //Action Display relate to this card
            if (_isLocked)
            {
                _actionDisplay?.StopHighlight();
            }
            else if (_currentState == 1)
            {
                cardDisplay.UpdateCardOrder(defaultCardOrder);
                transform.DOScale(_startLocalScale, _transitionSpeed).SetEase(Ease.OutBack);
                cardHighlight.DOKill();
                cardHighlight.DOFade(0f, 0.5f);
                _currentState = 0;
                
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isInteractable) return;
            
            //Get player Reference
            CombatEntity playerUnit = BattleStateManager.Instance?.playerUnit;

            if (playerUnit == null)
            {
                playerUnit = RogueliteBattleStateManager.Instance.playerUnit;
            }
            
            if (!_isLocked)
            {
                if (actionBar.InsertCard(cardDisplay, playerUnit,out ActionDisplay actionDisplay)) // If card can be inserted
                {
                    //Action connect to this card
                    _actionDisplay = actionDisplay;
                    
                    cardDisplay.isPlayed = true;
                    _currentState = 2;
                    _isLocked = true;
                    transform.DOLocalMoveY(_startYPosition + _hoverYOffset, _transitionSpeed).SetEase(Ease.OutBack);
                    cardHighlight.DOKill();
                    cardHighlight.DOColor(selectedColor, 0.5f);
                    cardHighlight.DOFade(1f, 0.5f);
                }
                else
                {
                    transform.DOKill(true);
                    transform.DOShakePosition(0.2f, _cannotClickOffset, 30, 90, false, true);
                }
            }
            else
            {
                _actionDisplay = null;
                cardDisplay.isPlayed = false;
                _currentState = 1;
                _isLocked = false;
                actionBar.RemoveCard(cardDisplay, playerUnit);
                transform.DOLocalMoveY(_startYPosition, _transitionSpeed).SetEase(Ease.OutBack);
            }
        }
    }
}