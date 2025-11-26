using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ShabuStudio.Gameplay
{
    public class UnitHealthbar : MonoBehaviour
    {
        [Header("References")]
        public CombatEntity targetUnit;
        public Slider healthSlider;
        public TMP_Text healthText;

        private void OnEnable()
        {
            if (targetUnit != null)
            {
                targetUnit.OnHealthChanged += UpdateHealthUI;
            }
        }
        
        
        private void OnDisable()
        {
            if (targetUnit != null) targetUnit.OnHealthChanged -= UpdateHealthUI;
        }

        /// <summary>
        /// Updates the health UI including the health bar and the health text display.
        /// </summary>
        /// <param name="currentHealth">The current health value of the target unit.</param>
        /// <param name="maxHealth">The maximum health value of the target unit.</param>
        void UpdateHealthUI(int currentHealth, int maxHealth)
        {
            float fillAmount = (float)currentHealth / maxHealth;


            healthSlider.DOKill(); 
            healthSlider.DOValue(fillAmount, 0.3f).SetEase(Ease.OutCubic);

            // Update Text
            if (healthText != null)
            {
                healthText.text = $"{currentHealth} / {maxHealth}";
            }
        }
    }
}