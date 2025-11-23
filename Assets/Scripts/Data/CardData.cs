using UnityEngine;

namespace ShabuStudio.Data
{
    [ CreateAssetMenu(fileName = "CardData", menuName = "Cards/New Card")]
    public class CardData : ScriptableObject
    {
        //Unique ID example:"Card_fire_ball_001"
        public string cardID;
        public CardType cardType;
        public string cardName;
        public Sprite cardImage;
        public string cardDescription;
        [Min(0)] public int cardCost;
        [Min(0)] public int cardMinSpeed;
        [Min(1)] public int cardMaxSpeed = 1;
        [Min(0)] public int cardDamage;

    }

    public enum CardType
    {
        Attack,
        Buff
    }
}