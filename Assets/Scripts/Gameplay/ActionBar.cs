using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using ShabuStudio.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShabuStudio.Gameplay
{
    public class ActionBar : MonoBehaviour
    {
        public static ActionBar Instance { get; private set; }

        private void Awake()
        {
            if(Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        public List<CardData> actionCardList = new List<CardData>();
        public List<int> actionCardSpeedList = new List<int>();
        public List<ActionDisplay> displayActions = new List<ActionDisplay>(); //For track action that match with card data.
        
        [Header("UI References")]
        public Transform container;
        public ActionDisplay actionPrefab;
        public TextMeshProUGUI actionCostText;
        
        


        public void Initialize()
        {
            
        }


        public async UniTask RearrangeAction(CancellationToken token = default)
        {
            //Prepare Dictionary
            Dictionary<ActionDisplay, int> originalIndices = new Dictionary<ActionDisplay, int>();
            for (int i = 0; i < displayActions.Count; i++)
            {
                originalIndices[displayActions[i]] = i;
            }


            List<Vector2> slotPositions = new List<Vector2>();

            // Force Unity to calculate the layout NOW so positions are correct
            Canvas.ForceUpdateCanvases();

            // loop through the TRANSFORM children (visual order), not the list
            foreach (Transform child in container)
            {
                if (child.gameObject.activeSelf)
                {
                    slotPositions.Add(child.GetComponent<RectTransform>().anchoredPosition);
                }
            }


            displayActions.Sort((a, b) =>
            {
                int compareResult = b.GetCurrentSpeed().CompareTo(a.GetCurrentSpeed());

                if (compareResult != 0)
                {
                    return compareResult;
                }

                if (a.ActionData.ownerEntity.unitType == ActionOwner.Player) return -1;
                if (b.ActionData.ownerEntity.unitType == ActionOwner.Player) return 1;

                return GetIndexActionDisplay(a).CompareTo(GetIndexActionDisplay(b));
            });

            // Animate
            VerticalLayoutGroup layoutGroup = container.GetComponent<VerticalLayoutGroup>();
            if (layoutGroup != null) layoutGroup.enabled = false;

            Sequence sortSequence = DOTween.Sequence();

            for (int i = 0; i < displayActions.Count; i++)
            {
                ActionDisplay card = displayActions[i];
                RectTransform cardRect = card.GetComponent<RectTransform>();

                // If have a valid slot position for this index, move there
                if (i < slotPositions.Count)
                {
                    // Join all movements into one sequence
                    sortSequence.Join(cardRect.DOAnchorPos(slotPositions[i], 0.3f).SetEase(Ease.OutQuad));
                }
            }

            await sortSequence.ToUniTask(cancellationToken: token);
            
            sortSequence.Kill(true);

            for (int i = 0; i < displayActions.Count; i++)
            {
                displayActions[i].transform.SetSiblingIndex(i);
            }

            if (layoutGroup != null)
            {
                layoutGroup.enabled = true;
                
                LayoutRebuilder.ForceRebuildLayoutImmediate(container.GetComponent<RectTransform>());
            }
        }

        private int GetIndexActionDisplay(ActionDisplay actionDisplay)
        {
            return displayActions.IndexOf(actionDisplay);
        }

        public static bool CanInsertCard(CardData cardData, CombatEntity entity)
        {
            return entity.currentCost - cardData.cardCost >= 0;
        }


        /// <summary>
        /// Inserts a card into the action bar based on its speed, updates the action cost,
        /// and animates the card's insertion into the UI.
        /// </summary>
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
        /// <returns>Returns true if the card is successfully inserted. Returns false if the insertion fails due to exceeding the maximum action cost.</returns>
        public bool InsertCard(CardData cardData, int cardSpeed, CombatEntity entity,out ActionDisplay actionDisplay)
        {
            //Return false if can't insert card anymore
            if (entity.currentCost - cardData.cardCost < 0)
            {
                actionDisplay = null;
                return false;
            }
            
            //Remove playing entity action cost first to prevent logic error
            entity.RemoveCost(cardData.cardCost);
            UpdateText(entity);
            
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
        public void RemoveCard(CardDisplay card, CombatEntity entity)
        {
            RemoveCard(card.cardData, card.currentSpeed,entity);
        }

        
        
        public void RemoveCard(CardData cardData, int cardSpeed, CombatEntity entity)
        {
            //Add playing entity action cost first to prevent logic error
            entity.AddCost(cardData.cardCost);
            UpdateText(entity);
            
            int index = actionCardSpeedList.IndexOf(cardSpeed);
            if (index < 0 || index >= displayActions.Count) return;
            
            GameObject objToRemove = displayActions[index].gameObject;

            // Remove from lists
            RemoveFromAllList(index);

            // Animate and Destroy
            RemoveActionObjWithAnim(objToRemove);
        }

        public async UniTask RemoveCardWaitFinish(int index,CancellationToken token)
        {
            if (index < 0 || index >= displayActions.Count) return;
            
            GameObject objToRemove = displayActions[index].gameObject;

            // Remove from lists
            RemoveFromAllList(index);

            // Animate and Destroy
            Tween tween = RemoveActionObjWithAnim(objToRemove);
            
            //Wait for tween finish and cancel when objToRemove is destroy
            await tween.ToUniTask(cancellationToken:token);
        }

        public async UniTask RemoveCardSameOwner(ActionOwner owner,CancellationToken token)
        {
            for (int i = displayActions.Count - 1; i >= 0; i--)
            {
                if (displayActions[i].ActionData.ownerEntity.unitType == owner)
                {
                    GameObject objToRemove = displayActions[i].gameObject;
                    RemoveFromAllList(i);
                    await RemoveActionObjWithAnim(objToRemove).ToUniTask(cancellationToken:token);
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
                    objToRemove.transform.DOKill();
                    Destroy(objToRemove);
                });
        }
        

        void UpdateText(CombatEntity entity)
        {
            if(entity != null && entity.unitType == ActionOwner.Player)
            {
                actionCostText.text = $"Cost : {entity.currentCost}";
            }
        }

        public void UpdateUI()
        {
            UpdateText(BattleStateManager.Instance.playerUnit);
            UpdateText(BattleStateManager.Instance.enemyUnit);
        }
        
    }
}