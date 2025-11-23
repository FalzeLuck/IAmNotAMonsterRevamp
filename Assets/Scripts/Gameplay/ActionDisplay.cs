using ShabuStudio.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ShabuStudio.Gameplay
{
    public class ActionDisplay : MonoBehaviour
    {
        public Action action;
        private ActionOwner owner;
        
        [Header("References")]
        public TextMeshProUGUI speedText;
        public Image ownerIndicatorImage;

        public void Initialize(CardData cardData,int cardSpeed, ActionOwner owner)
        {
            action = new Action(cardData, cardSpeed, owner);
            this.owner = owner;
            speedText.text = action.speed.ToString();
            if(owner == ActionOwner.Player)
            {
                ownerIndicatorImage.color = Color.green;
            }
            else
            {
                ownerIndicatorImage.color = Color.red;
            }
        }
    }

    public class Action
    {
        public CardData cardData;
        public int speed;
        public ActionOwner owner;
        
        public Action(CardData cardData, int speed, ActionOwner owner)
        {
            this.cardData = cardData;
            this.speed = speed;
            this.owner = owner;
        }
    }

    public enum ActionOwner
    {
        Player,
        Enemy
    }
}