using System;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace ShabuStudio.Story
{
    public class CharacterController : MonoBehaviour
    {
        [Header("UI Reference")]
        public SpriteRenderer characterSpriteRenderer;
        
        
        public DialogueRunner dialogueRunner;

        private void Awake()
        {
            dialogueRunner.AddCommandHandler<string,string>("set_sprite", SetSprite);;
        }

        public void SetSprite(string charName, string emotion)
        {
            
            string fileName = $"{charName}_{emotion}";

            
            Sprite newSprite = Resources.Load<Sprite>($"Sprites/{fileName}");

            if (newSprite != null)
            {
                characterSpriteRenderer.sprite = newSprite;
                characterSpriteRenderer.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"Sprite not found: {fileName}");
                characterSpriteRenderer.gameObject.SetActive(false);
            }
        }
    }
}