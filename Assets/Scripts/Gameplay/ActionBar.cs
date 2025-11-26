using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ShabuStudio.Data;
using TMPro;
using UnityEngine;

namespace ShabuStudio.Gameplay
{
    public class ActionBar : MonoBehaviour
    {
        public List<CardData> actionCardList = new List<CardData>();
        public List<int> actionCardSpeedList = new List<int>();
        public List<ActionDisplay> displayActions = new List<ActionDisplay>(); //For track action that match with card data.
        
        [Header("UI References")]
        public Transform container;
        public ActionDisplay actionPrefab;
        public TextMeshProUGUI actionCostText;
        
        [Header("Action Cost Parameter")]
        private readonly Dictionary<ActionOwner,int> _currentActionCost = new Dictionary<ActionOwner, int>(); // Action owner , cost
        


        public void Initialize()
        {
            _currentActionCost.Add(ActionOwner.Player,0);
            _currentActionCost.Add(ActionOwner.Enemy,0);
        }

        public void ResetAllCost()
        {
            //Temporary Reset. May have a better method to reset in the future.
            _currentActionCost[ActionOwner.Player] = 0;
            _currentActionCost[ActionOwner.Enemy] = 0;
            
            UpdateText(ActionOwner.Player);
        }

        /// <summary>
        /// Inserts a card into the action bar based on its speed, updates the action cost,
        /// and animates the card's insertion into the UI.
        /// </summary>
        /// <param name="cardData">The data object representing the card's characteristics.</param>
        /// <param name="cardSpeed">The speed of the card, determining its position in the action bar.</param>
        /// <param name="owner">The owner of the card (e.g., Player or Enemy) for determining action cost limitations.</param>
        /// <returns>Returns true if the card was successfully inserted. Returns false if the insertion fails due to exceeding the maximum action cost.</returns>
        public bool InsertCard(CardDisplay card, CombatEntity entity,out ActionDisplay actionDisplay)
        {
            return InsertCard(card.cardData, card.currentSpeed, entity,out actionDisplay);
        }

        /// <summary>
        /// Inserts a card into the action bar at a position determined by its speed.
        /// Also updates the action cost and animates the new card's insertion.
        /// </summary>
        /// <param name="cardData">The data object representing the characteristics of the card being inserted.</param>
        /// <param name="cardSpeed">The speed parameter of the card, determining its position in the action bar.</param>
        /// <param name="owner">The owner of the card (e.g., Player or Enemy).</param>
        /// <returns>Returns true if the card is successfully inserted. Returns false if the insertion fails due to exceeding the maximum action cost.</returns>
        public bool InsertCard(CardData cardData, int cardSpeed, CombatEntity entity,out ActionDisplay actionDisplay)
        {
            ActionOwner owner = entity.unitType;
            int maxActionCost = entity.Stats.MaxCost; 
            //Return false if can't insert card anymore
            if (cardData.cardCost + _currentActionCost[owner] > maxActionCost)
            {
                actionDisplay = null;
                return false;
            }
            
            //Add action cost first to prevent logic error
            AddActionCost(cardData.cardCost,owner);
            
            //Continue insert card
            int index = GetActionIndexBySpeed(cardSpeed);
            actionCardSpeedList.Insert(index, cardSpeed);
            actionCardList.Insert(index, cardData);
            ActionDisplay newAction = Instantiate(actionPrefab, container);
            newAction.Initialize(cardData, cardSpeed, entity);
            newAction.transform.SetSiblingIndex(index);
            displayActions.Insert(index, newAction);
            
            newAction.transform.localScale = new Vector3(0, 0, 0);
            newAction.transform.DOScale(1f,0.3f).SetEase(Ease.OutBack);
            
            actionDisplay = newAction;
            return true;
        }



        /// <summary>
        /// Determines the index at which a new card should be inserted into the action card list based on its speed.
        /// </summary>
        /// <param name="cardSpeed">The card to evaluate and determine its appropriate position in the list.</param>
        /// <returns>The index at which the card should be inserted based on its speed.</returns>
        int GetActionIndexBySpeed(int cardSpeed)
        {
            int index = 0;
            for (int i = 0; i < actionCardList.Count; i++)
            {
                if (cardSpeed < actionCardSpeedList[i]) //Move card down if speed is lower.
                {
                    index++;
                }
            }
            return index;
        }

        /// <summary>
        /// Removes a card from the action bar, and performs the removal animation.
        /// </summary>
        /// <param name="card">The data object representing the card's display to be removed.</param>
        /// <param name="owner">The owner of the card (e.g., Player or Enemy) to adjust the appropriate action cost.</param>
        public void RemoveCard(CardDisplay card, CombatEntity entity)
        {
            RemoveCard(card.cardData, card.currentSpeed,entity);
        }

        
        
        public void RemoveCard(CardData cardData, int cardSpeed, CombatEntity entity)
        {
            ActionOwner owner = entity.unitType;
            
            //Remove action cost first to prevent logic error
            AddActionCost(-cardData.cardCost,owner);
            
            int index = actionCardSpeedList.IndexOf(cardSpeed);
            if (index < 0 || index >= displayActions.Count) return;
            
            GameObject objToRemove = displayActions[index].gameObject;

            // Remove from lists
            RemoveFromAllList(index);

            // Animate and Destroy
            RemoveActionObjWithAnim(objToRemove);
        }

        public IEnumerator RemoveCardWaitFinish(int index)
        {
            if (index < 0 || index >= displayActions.Count) yield break;
            
            GameObject objToRemove = displayActions[index].gameObject;

            // Remove from lists
            RemoveFromAllList(index);

            // Animate and Destroy
            Tween tween = RemoveActionObjWithAnim(objToRemove);
            
            yield return tween.WaitForCompletion();
        }

        public IEnumerator RemoveCardSameOwner(ActionOwner owner)
        {
            for (int i = displayActions.Count - 1; i >= 0; i--)
            {
                if (displayActions[i].action.ownerEntity.unitType == owner)
                {
                    GameObject objToRemove = displayActions[i].gameObject;
                    RemoveFromAllList(i);
                    RemoveActionObjWithAnim(objToRemove);
                    yield return null;
                }
            }
        }

        void RemoveFromAllList(int index)
        {
            actionCardList.RemoveAt(index);
            actionCardSpeedList.RemoveAt(index);
            displayActions.RemoveAt(index);
        }

        Tween RemoveActionObjWithAnim(GameObject objToRemove)
        {
            return objToRemove.transform
                .DOScale(0f, 0.3f)  
                .SetEase(Ease.InBack)   
                .OnComplete(() =>        
                {
                    Destroy(objToRemove);
                });
        }


        void AddActionCost(int cost,ActionOwner owner)
        {
            _currentActionCost[owner] += cost;
            UpdateText(owner);
        }

        void UpdateText(ActionOwner owner)
        {
            if(owner == ActionOwner.Player)
            {
                actionCostText.text = $"{_currentActionCost[owner].ToString()} / {BattleStateManager.Instance.playerUnit.Stats.MaxCost.ToString()}";
            }
        }

        public void UpdateUI()
        {
            UpdateText(ActionOwner.Player);
            UpdateText(ActionOwner.Enemy);
        }
        
    }
}