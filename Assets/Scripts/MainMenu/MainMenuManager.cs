using System;
using ShabuStudio.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace ShabuStudio.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        public VisualElement ui;

        public Button mapButton;
        public Button settingButton;
        public Button shopButton;
        public Button buildButton;
        public Button exitButton;
        
        

        private void Awake()
        {
            ui = GetComponent<UIDocument>().rootVisualElement;
        }

        private void OnEnable()
        {
            mapButton = ui.Q<Button>("MapButton");
            mapButton.clicked += OnMapButtonClicked;
            
            settingButton = ui.Q<Button>("SettingButton");
            settingButton.clicked += OnSettingButtonClicked;
            
            shopButton = ui.Q<Button>("ShopButton");
            shopButton.clicked += OnShopButtonClicked;
            
            buildButton = ui.Q<Button>("BuildButton");
            buildButton.clicked += OnBuildButtonClicked;
            
            exitButton = ui.Q<Button>("ExitButton");
            exitButton.clicked += OnExitButtonClicked;
        }

        
        //Unhhok all register event to prevent memory leak.
        private void OnDisable()
        {
            mapButton.clicked -= OnMapButtonClicked;
            settingButton.clicked -= OnSettingButtonClicked;
            shopButton.clicked -= OnShopButtonClicked;
            buildButton.clicked -= OnBuildButtonClicked;
            exitButton.clicked -= OnExitButtonClicked;
        }

        

        void OnMapButtonClicked()
        {
            SceneLoader.LoadSceneWithTransitionCanvas("Scene_ChapterSelect",SceneLoader.Instance.defaultTransitionCanvas);
        }
        
        void OnSettingButtonClicked()
        {
            SceneManager.LoadSceneAsync("Scene_Setting", LoadSceneMode.Additive);
        }
        
        
        void OnShopButtonClicked()
        {
            
        }
        
        void OnBuildButtonClicked()
        {
            
        }

        void OnExitButtonClicked()
        {
            Application.Quit();
        }
    }
}