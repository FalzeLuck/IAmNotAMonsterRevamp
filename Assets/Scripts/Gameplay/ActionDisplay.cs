using DG.Tweening;
using ShabuStudio.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShabuStudio.Gameplay
{
    public class ActionDisplay : MonoBehaviour
    {
        public Action action;
        
        [Header("References")]
        public TextMeshProUGUI speedText;
        public Image ownerIndicatorImage;
        
        //Hightlight references
        [SerializeField]private Image highlightImage;

        public void Initialize(CardData cardData,int cardSpeed, CombatEntity ownerEntity)
        {
            action = new Action(cardData, cardSpeed, ownerEntity);
            speedText.text = action.speed.ToString();
            if(ownerEntity.unitType == ActionOwner.Player)
            {
                ownerIndicatorImage.color = Color.green;
            }
            else
            {
                ownerIndicatorImage.color = Color.red;
            }
            
            highlightImage.DOFade(0, 0);
        }
        
        public void StartHighlight()
        {
            highlightImage.DOFade(1, 0.2f);
        }
        
        public void StopHighlight()
        {
            highlightImage.DOFade(0, 0.2f);
        }
        
    }

    public class Action
    {
        public CardData cardData;
        public int speed;
        public CombatEntity ownerEntity;
        
        public Action(CardData cardData, int speed, CombatEntity ownerEntity)
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