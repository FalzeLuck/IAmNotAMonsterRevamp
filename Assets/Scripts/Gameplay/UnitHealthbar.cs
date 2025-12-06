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
        [SerializeField]private CombatEntity targetUnit;
        public Image barForeground;
        public TMP_Text healthText;

        private void OnEnable()
        {
            if (targetUnit != null)
            {
                targetUnit.OnHealthChanged += UpdateHealthUI;
            }
        }

        public void SetTarget(CombatEntity target)
        {
            if (targetUnit == null)
            {
                targetUnit = target;
                OnEnable();
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


            barForeground.DOKill(); 
            barForeground.DOFillAmount(fillAmount, 0.3f).SetEase(Ease.OutCubic);

            // Update Text
            if (healthText != null)
            {
                healthText.text = $"{currentHealth} / {maxHealth}";
            }
        }
    }
}