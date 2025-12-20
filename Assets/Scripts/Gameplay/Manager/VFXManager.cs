using System.Threading;
using UnityEngine;
using UnityEngine.Playables;
using Cysharp.Threading.Tasks;
using ShabuStudio.Data;
using UnityEngine.Timeline;

namespace ShabuStudio.Gameplay
{
    public class VFXManager : MonoBehaviour
    {
        public static VFXManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }
        

        public async UniTask PlayTimelineAsync(GameObject vfxPrefab,Transform vfxSpawnParent,DamageTextManager bindingSignalObject, CancellationToken token)
        {
            GameObject vfxObject;
            if (vfxPrefab != null)
            {
                vfxObject = Instantiate(vfxPrefab, vfxSpawnParent);
            }
            else
            {
                return;
            }
            
            PlayableDirector vfxDirector = vfxObject.GetComponent<PlayableDirector>();
            if(vfxDirector == null || vfxDirector.playableAsset == null) return;
            
            BindingSignalTrack(vfxDirector,bindingSignalObject.gameObject);
            
            vfxDirector.Play();

            await UniTask.Delay(
                System.TimeSpan.FromSeconds(vfxDirector.duration),
                ignoreTimeScale:false,
                cancellationToken: token);

            vfxDirector.Stop();
            Destroy(vfxObject);
        }
        
        public async UniTask PlayTimelineAsync(CardData cardData,Transform vfxSpawnParent,DamageTextManager bindingSignalObject, CancellationToken token)
        {
            GameObject vfxObject;
            if (cardData.vfxPrefab != null)
            {
                vfxObject = Instantiate(cardData.vfxPrefab, vfxSpawnParent);
            }
            else
            {
                return;
            }
            
            PlayableDirector vfxDirector = vfxObject.GetComponent<PlayableDirector>();
            if(vfxDirector == null || vfxDirector.playableAsset == null) return;
            
            BindingSignalTrack(vfxDirector,bindingSignalObject.gameObject);
            
            vfxDirector.Play();

            await UniTask.Delay(
                System.TimeSpan.FromSeconds(vfxDirector.duration),
                ignoreTimeScale:false,
                cancellationToken: token);

            vfxDirector.Stop();
            Destroy(vfxObject);
        }

        
        //Binding Signal Track
        void BindingSignalTrack(PlayableDirector director, GameObject signalObject)
        {
            var timelineAsset = director.playableAsset as TimelineAsset;

            foreach (var track in timelineAsset.GetOutputTracks())
            {
                if (track is SignalTrack)
                {
                    director.SetGenericBinding(track, signalObject);
                    return;
                }
            }
        }
    }
}