using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ShabuStudio.Gameplay
{
    public class FloatNumberText : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]private TextMeshProUGUI damageText;
        
        [Header("Settings")]
        [SerializeField] private float floatDistance = 100f;
        [SerializeField] private float duration = 1f;
        [SerializeField] private float randomXOffset = 10f;
    
        [Header("Animation")]
        [SerializeField] private Ease movementEase = Ease.OutCirc;
        [SerializeField] private Vector3 punchScale = new Vector3(0.5f, 0.5f, 0.5f);
        
        public void SetupAndStart(Vector3 screenPos,int numberAmount,Color textColor, bool isCriticalHit,string prefix = "")
        {
            damageText.text = $"{prefix}{numberAmount}";
            damageText.color = textColor;    
        
            transform.position = screenPos;
            
            if (isCriticalHit) //If damage can critical in future.
            {
                damageText.fontSize *= 1.5f;
            }

            
            // add a slight random X offset so texts don't overlap perfectly
            float xOffset = Random.Range(-randomXOffset, randomXOffset);
            Vector3 targetPos = transform.position + new Vector3(xOffset, floatDistance, 0);

            // Create the DoTween Sequence
            Sequence sequence = DOTween.Sequence();

            // Makes it pop out
            sequence.Append(transform.DOPunchScale(punchScale, 0.3f));

            // Move Up
            // Join this so it happens at the same time as the scale
            sequence.Join(transform.DOMove(targetPos, duration).SetEase(movementEase));

            // Fade out
            // Join but add a delay so it stays visible for a bit first
            sequence.Join(damageText.DOFade(0f, duration / 2).SetDelay(duration / 2));

            // 4. Cleanup
            sequence.OnComplete(() => {
                gameObject.SetActive(false);
            });
        }
    }
}