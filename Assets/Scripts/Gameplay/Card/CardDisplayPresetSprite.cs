using UnityEngine;

namespace ShabuStudio.Gameplay
{
    [CreateAssetMenu(fileName = "CardDisplayPreset", menuName = "Card/CardDisplayPresetSprite", order = 0)]
    public class CardDisplayPresetSprite : ScriptableObject
    {
        public Sprite attackCardBG;
        public Sprite buffCardBG;
        public Sprite debuffCardBG;
    }
}