using UnityEngine;
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


        }

        void OnMapButtonClicked()
        {
            SceneLoader.LoadScene(3);
        }
        
        void OnSettingButtonClicked()
        {
            
        }
        
        void OnShopButtonClicked()
        {
            
        }
        
        void OnBuildButtonClicked()
        {
            
        }
        
        
    }
}