using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace ShabuStudio.Intro
{
    public class IntroManager : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("The name of the scene to load")]
        public int mainMenuSceneIndex;

        
        public void OnPlayerJoined(PlayerInput playerInput)
        {
            Debug.Log($"Player joined using: {playerInput.devices[0].displayName}");

            
            DontDestroyOnLoad(playerInput.gameObject);

            
            playerInput.DeactivateInput();
            
            SceneLoader.Instance.LoadScene(mainMenuSceneIndex);
        }
    }
}