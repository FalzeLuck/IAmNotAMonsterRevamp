using System;
using ShabuStudio.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace ShabuStudio.MainMenu
{
    public class SettingUIManager : MonoBehaviour
    {
        public VisualElement ui;
        
        public Button exitSettingButton;
        
        //Audio Settings
        public Slider masterVolumeSlider;
        public Slider sfxVolumeSlider;
        public Slider musicVolumeSlider;
        
        private void Awake()
        {
            ui = GetComponent<UIDocument>().rootVisualElement;
        }
        
        private void OnEnable()
        {
            exitSettingButton = ui.Q<Button>("ExitSettingButton");
            exitSettingButton.clicked += OnExitSettingButtonClicked;
            
            //Audio Slider
            masterVolumeSlider = ui.Q<Slider>("MasterVolumeSlider");
            masterVolumeSlider.RegisterCallback<ChangeEvent<float>>(OnMasterVolumeChanged);
            sfxVolumeSlider = ui.Q<Slider>("SFXVolumeSlider");
            sfxVolumeSlider.RegisterCallback<ChangeEvent<float>>(OnSFXVolumeChanged);
            musicVolumeSlider = ui.Q<Slider>("MusicVolumeSlider");
            musicVolumeSlider.RegisterCallback<ChangeEvent<float>>(OnMusicVolumeChanged);
        }
        
        //Unhhok all register event to prevent memory leak.
        private void OnDisable()
        {
            exitSettingButton.clicked -= OnExitSettingButtonClicked;
            
            //Audio
            masterVolumeSlider.UnregisterCallback<ChangeEvent<float>>(OnMasterVolumeChanged);
            sfxVolumeSlider.UnregisterCallback<ChangeEvent<float>>(OnSFXVolumeChanged);
            musicVolumeSlider.UnregisterCallback<ChangeEvent<float>>(OnMusicVolumeChanged);
        }
        
        private void Update()
        {
            masterVolumeSlider.value = AudioManager.Instance.masterVolume;
            sfxVolumeSlider.value = AudioManager.Instance.sfxVolume;
            musicVolumeSlider.value = AudioManager.Instance.musicVolume;
        }
        
        void OnMasterVolumeChanged(ChangeEvent<float> evt)
        {
            AudioManager.Instance.masterVolume = evt.newValue;
        }
        
        void OnSFXVolumeChanged(ChangeEvent<float> evt)
        {
            AudioManager.Instance.sfxVolume = evt.newValue;
        }
        
        void OnMusicVolumeChanged(ChangeEvent<float> evt)
        {
            AudioManager.Instance.musicVolume = evt.newValue;
        }

        void OnExitSettingButtonClicked()
        {
            SceneManager.UnloadSceneAsync("Scene_Setting");
        }   
    }
}