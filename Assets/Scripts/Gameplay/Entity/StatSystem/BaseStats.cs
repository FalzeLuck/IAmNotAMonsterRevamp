using UnityEngine;

namespace ShabuStudio.Gameplay
{
    [CreateAssetMenu(fileName = "New Base Stats", menuName = "Stats/Base Stats")]
    public class BaseStats : ScriptableObject
    {
        public int maxHealth;
    }
}