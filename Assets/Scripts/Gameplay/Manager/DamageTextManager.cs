using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace ShabuStudio.Gameplay
{
    public class DamageTextManager : MonoBehaviour, INotificationReceiver
    {
        [SerializeField] private DamageText damageTextPrefab;
        [SerializeField] private Transform damageTextCanvasTransform;
        private List<DamageText> _damageTextList = new List<DamageText>();
        
        //Queue to store pre-calculated damage
        private Queue<DamageInfo> _damageQueue = new Queue<DamageInfo>();
        private Transform damagePos;
        
        public struct DamageInfo
        {
            public int damageAmount;
        }

        public void Initialize()
        {
            
        }
        
        public void PrepareDamage(int damageAmount,Transform damageSpawnPoint, float[] ratios)
        {
            _damageQueue.Clear();
            damagePos = damageSpawnPoint;
            float ratioSum = 0;
            foreach (float ratio in ratios) ratioSum += ratio;
            
            foreach (float ratio in ratios)
            {
                int eachHitDamage = Mathf.RoundToInt(damageAmount * (ratio/ratioSum));
                
                _damageQueue.Enqueue(new DamageInfo { damageAmount = eachHitDamage });
            }
        }
        
        public void OnNotify(Playable sender, INotification notification, object context)
        {
            if (notification is SignalEmitter emitter && emitter.asset.name == "Signal_OnHit")
            {
                ProcessOnHitDamage();
            }
        }

        public void FlushRamainingDamage()
        {
            while (_damageQueue.Count > 0)
            {
                ProcessOnHitDamage();
            }
        }

        void ProcessOnHitDamage()
        {
            if (_damageQueue.Count == 0) return;
            
            DamageInfo damageInfo = _damageQueue.Dequeue();
            Debug.Log(damageInfo.damageAmount);
            
            SpawnDamageText(damagePos.position,damageInfo.damageAmount,false);
            
        }
        
        public void SpawnDamageText(Vector3 screenPos, int damageAmount, bool isCriticalHit)
        {
            if(damageAmount <= 0) return;
            
            //Check if there is any inactive damage text in list.
            foreach (DamageText item in _damageTextList)
            {
                if (!item.gameObject.activeInHierarchy) 
                {
                    item.gameObject.SetActive(true);
                    item.SetupAndStart(screenPos,damageAmount, isCriticalHit);
                    return;
                }
            }
            
            //Else create new damageText
            DamageText damageText = Instantiate(damageTextPrefab,damageTextCanvasTransform);
            _damageTextList.Add(damageText);
            damageText.SetupAndStart(screenPos,damageAmount, isCriticalHit);
            
        }
    }
}