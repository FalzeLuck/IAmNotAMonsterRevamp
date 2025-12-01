using System;
using UnityEngine;

namespace ShabuStudio.Gameplay
{
    public class GameplayUpdateComponent : MonoBehaviour
    {
        public CardDetailDisplay cardDetailDisplay;

        private void Update()
        {
            cardDetailDisplay.Reload();
        }
    }
}