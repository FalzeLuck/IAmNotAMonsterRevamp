using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpeedButton : MonoBehaviour, IPointerClickHandler
{
    [Header("Settings")]
        public float normalSpeed = 1.0f;
        public float fastSpeed = 2.0f;
    
        [Header("UI References")]   
        public TextMeshProUGUI buttonText;
        
        // Reference to the background image
        public Image buttonImage; 
    
        // Colors for the states
        public Color onColor = Color.green;
        public Color offColor = Color.gray;
    
        private bool isFast = false;
    
        void Start()
        {
            // Initialize state
            SetSpeed(isFast);
        }
    
        private void OnToggleClicked()
        {
            isFast = !isFast;
            SetSpeed(isFast);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            OnToggleClicked();
        }
    
        void SetSpeed(bool fast)
        {
            if (fast)
            {
                Time.timeScale = fastSpeed;
                if (buttonText) buttonText.text = "2x";
                
                // Change to Green 
                if (buttonImage) buttonImage.color = onColor;
            }
            else
            {
                Time.timeScale = normalSpeed;
                if (buttonText) buttonText.text = "1x";
    
                // Change to Grey
                if (buttonImage) buttonImage.color = offColor;
            }
    
            // Prevent physics error (Even this game don't have physic, I just want to make sure:) )
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
}
