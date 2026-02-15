using ShabuStudio.Chapter;
using UnityEngine;

namespace Roguelite
{
    [CreateAssetMenu(fileName = "StageData", menuName = "Roguelite/StageData")]
    public class RogueliteStageData : ScriptableObject
    {
        public Texture2D chapterBgImage;
        public GameObject chapterBg3DObject;
        public GameObject enemyPrefab;
    }
}