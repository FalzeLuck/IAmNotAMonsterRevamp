using ShabuStudio.Chapter;
using UnityEngine;

namespace Roguelite
{
    [CreateAssetMenu(fileName = "StageData", menuName = "Roguelite/StageData")]
    public class RogueliteStageData : ScriptableObject
    {
        public StageType stageType;
        public Sprite chapterBgImage;
        public GameObject chapterBg3DObject;
        public GameObject enemyPrefab;
        public Sprite enemySpriteArt;
        
        public enum StageType
        {
            Normal,
            Boss,
            Elite
        }
    }
}