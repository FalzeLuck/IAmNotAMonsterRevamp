using System.Threading;
using UnityEngine;
using UnityEngine.Playables;
using Cysharp.Threading.Tasks;
using ShabuStudio.Data;
using UnityEngine.Timeline;
using UnityEngine.VFX;

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
        

        public async UniTask PlayTimelineAsync(GameObject vfxPrefab,Transform targetVFXPos,DamageTextManager bindingSignalObject, CancellationToken token)
        {
            GameObject vfxObject;
            if (vfxPrefab != null)
            {
                vfxObject = Instantiate(vfxPrefab, targetVFXPos);
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
        
        public async UniTask PlayTimelineAsync(CardData cardData,Transform targetVFXPos,Transform selfVfxPos,DamageTextManager bindingSignalObject, CancellationToken token)
        {
            GameObject vfxObject;
            
            //Create VFX object
            if (cardData.vfxPrefab != null)
            {
                vfxObject = Instantiate(cardData.vfxPrefab, targetVFXPos);
            }
            else
            {
                return;
            }
            
            //Set VFX Self Pos
            VisualEffect vfx = vfxObject.GetComponentInChildren<VisualEffect>();
            if(vfx.HasVector3("TargetPos"))
            {
                vfx.SetVector3("TargetPos", selfVfxPos.position);
            }
            
            //Set Timeline
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