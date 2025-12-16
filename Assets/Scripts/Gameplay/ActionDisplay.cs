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
        public Sprite playerIndicator;
        public Sprite enemyIndicator;
        
        //Hightlight references
        [SerializeField]private Image highlightImage;

        public void Initialize(CardData cardData,int cardSpeed, CombatEntity ownerEntity)
        {
            ActionData = new ActionData(cardData, cardSpeed, ownerEntity);
            speedText.text = ActionData.speed.ToString();
            if(ownerEntity.unitType == ActionOwner.Player)
            {
                ActionBackgroundImage.sprite = playerIndicator;
            }
            else
            {
                ActionBackgroundImage.sprite = enemyIndicator;
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