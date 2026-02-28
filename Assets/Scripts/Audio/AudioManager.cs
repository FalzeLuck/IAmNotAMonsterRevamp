using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace ShabuStudio.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        
        [Header("Volume")]
        [Range(0f, 1f)] public float masterVolume = 1f; // [ 0 , 1]
        [Range(0f, 1f)] public float sfxVolume = 1f; // [ 0 , 1]
        [Range(0f, 1f)] public float musicVolume = 1f; // [ 0 , 1]
        
        private Bus masterBus;
        private Bus sfxBus;
        private Bus musicBus;
        
        private void Awake()
        {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            }
            else {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            
            masterBus = RuntimeManager.GetBus("bus:/");
            sfxBus = RuntimeManager.GetBus("bus:/SFX");
            musicBus = RuntimeManager.GetBus("bus:/Music");
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            masterBus.setVolume(masterVolume);
            sfxBus.setVolume(sfxVolume);
            musicBus.setVolume(musicVolume);
        }
    }
}