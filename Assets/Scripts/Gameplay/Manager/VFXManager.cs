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
            
            Debug.LogError("No Signal Track Found");
        }
    }
}