using System.Collections.Generic;
using ShabuStudio.Gameplay;
using UnityEngine;
using UnityEngine.Localization;

namespace ShabuStudio.Data
{
    [ CreateAssetMenu(fileName = "CardData", menuName = "Cards/New Card")]
    public class CardData : ScriptableObject
    {
        //Unique ID example:"Card_fire_ball_001"
        public string cardID;
        public CardType cardType;
        [BF_SubclassList.SubclassList(typeof(Buff)), SerializeField]public Buff_container buffsToApply;
        public Sprite cardImage;
        public Sprite cardIcon;
        public Sprite cardFloatIcon;
        public Vector2 cardFloatIconPosition;
        public LocalizedString cardName;
        public LocalizedString cardDescription;
        [Min(0)] public int cardCost;
        [Min(0)] public int cardSpeed;
        [Min(0)] public int damage;
        public bool canInflictBuff = false;
        
        [Header("Animation & Timeline")]
        public GameObject vfxPrefab;
        public float[] hitRatio = {1f};

    }

    public enum CardType
    {
        Attack,
        Buff,
        Debuff
    }
}