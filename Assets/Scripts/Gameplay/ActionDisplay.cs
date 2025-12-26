using System;
using System.Collections.Generic;
using DG.Tweening;
using ShabuStudio.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShabuStudio.Gameplay
{
    public class ActionDisplay : MonoBehaviour
    {
        public ActionData ActionData;
        
        [Header("References")]
        public TextMeshProUGUI speedText;
        public Image ActionBackgroundImage;
        
        //Hightlight references
        [SerializeField]private Image highlightImage;
        [SerializeField]private Image iconImage;
        
        //For Update Data
        private ActionData _currentActionData;
        private static List<ActionDisplay> allActionDisplays = new List<ActionDisplay>();

        private void OnEnable()
        {
            allActionDisplays.Add(this);
        }

        private void OnDisable()
        {
            allActionDisplays.Remove(this);
        }

        public void Initialize(CardData cardData,int cardSpeed, CombatEntity ownerEntity)
        {
            ActionData = new ActionData(cardData, cardSpeed, ownerEntity);
            UpdateUI();
            _currentActionData = ActionData;
            if(ownerEntity.unitType == ActionOwner.Player)
            {
                iconImage.sprite = cardData.cardIcon;
            }
            else
            {
                EnemyCombatEntity enemy = ownerEntity as EnemyCombatEntity;
                iconImage.sprite = enemy?.enemyIcon;
            }
            
            highlightImage.DOFade(0, 0);
        }

        void UpdateUI()
        {
            speedText.text = ActionData.speed.ToString();
        }
        
        public void StartHighlight()
        {
            highlightImage.DOFade(1, 0.2f);
        }
        
        public void StopHighlight()
        {
            highlightImage.DOFade(0, 0.2f);
        }

        public int GetCurrentSpeed()
        {
            return _currentActionData.speed;
        }

        public ActionOwner GetOwner()
        {
            return _currentActionData.ownerEntity.unitType;
        }

        //Add and update ActionDisplay speed amount and UI for all ActionDisplays objects.
        public static void AddActionDataSpeed(int amount, ActionOwner owner)
        {
            foreach (ActionDisplay actionDisplay in allActionDisplays)
            {
                if (actionDisplay._currentActionData.ownerEntity.unitType == owner)
                {
                    actionDisplay._currentActionData.speed += amount;
                    actionDisplay.UpdateUI();
                }
            }
        }
        
    }

    public class ActionData
    {
        public CardData cardData;
        public int speed;
        public CombatEntity ownerEntity;
        
        public ActionData(CardData cardData, int speed, CombatEntity ownerEntity)
        {
            this.cardData = cardData;
            this.speed = speed;
            this.ownerEntity = ownerEntity;
        }
    }

    public enum ActionOwner
    {
        Player,
        Enemy
    }
}