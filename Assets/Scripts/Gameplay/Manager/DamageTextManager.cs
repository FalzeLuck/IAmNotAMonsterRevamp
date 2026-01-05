using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace ShabuStudio.Gameplay
{
    public class DamageTextManager : MonoBehaviour, INotificationReceiver
    {
        public static DamageTextManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        [Header("Properties")]
        [SerializeField] private Vector2 damageTextOffset = new Vector2(100f, 100f);
        
        [Header("References")]
        [SerializeField] private FloatNumberText floatNumberTextPrefab;
        [SerializeField] private Transform damageTextCanvasTransform;
        [SerializeField] private CinemachineImpulseSource impulseSource;
        [SerializeField] private Color damageColor = Color.red;
        private List<FloatNumberText> _damageTextList = new List<FloatNumberText>();
        
        //Queue to store pre-calculated damage
        private Queue<DamageInfo> _damageQueue = new Queue<DamageInfo>();
        private Transform damagePos;
        
        private CombatEntity _currentTakeDamageEntity;
        
        public struct DamageInfo
        {
            public int damageAmount;
        }

        public void Initialize()
        {
            
        }
        
        public void PrepareNumber(int damageAmount,Transform damageSpawnPoint,CombatEntity entity, float[] ratios)
        {
            _damageQueue.Clear();
            _currentTakeDamageEntity = entity;
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
            if (notification is SignalEmitter emitter)
            {
                if (emitter.asset.name == "Signal_OnHit")
                {
                    ProcessOnHitDamage();
                }
            }
            
        }

        public void FlushRemainingDamage()
        {
            _currentTakeDamageEntity = null;
            while (_damageQueue.Count > 0)
            {
                ProcessOnHitDamage();
            }
        }

        void ProcessOnHitDamage()
        {
            if (_damageQueue.Count == 0) return;
            
            DamageInfo damageInfo = _damageQueue.Dequeue();

            if (_currentTakeDamageEntity is EnemyCombatEntity && _currentTakeDamageEntity!=null)
            {
                (_currentTakeDamageEntity as EnemyCombatEntity).PlayTrigger("Hit");
            }
            
            if (impulseSource != null && damageInfo.damageAmount > 0)
            {
                float shakeForce = 0.2f; 
                impulseSource.GenerateImpulse(shakeForce);
            }
            
            SpawnText(damagePos.position,damageInfo.damageAmount,damageColor,false, "-");
            
        }

        
        public void SpawnText(Vector3 screenPos, int damageAmount,Color textColor, bool isCriticalHit,string prefix = "")
        {
            if(damageAmount <= 0) return;
            
            float xPos = screenPos.x + Random.Range(0,damageTextOffset.x);
            float yPos = screenPos.y + Random.Range(0,damageTextOffset.y);
            
            Vector3 realSpawnPos = new Vector3(xPos,yPos,screenPos.z);
            
            //Check if there is any inactive damage text in list.
            foreach (FloatNumberText item in _damageTextList)
            {
                if (!item.gameObject.activeInHierarchy) 
                {
                    item.gameObject.SetActive(true);
                    item.SetupAndStart(realSpawnPos,damageAmount,textColor, isCriticalHit,prefix);
                    return;
                }
            }
            
            //Else create new damageText
            FloatNumberText floatNumberText = Instantiate(floatNumberTextPrefab,damageTextCanvasTransform);
            _damageTextList.Add(floatNumberText);
            floatNumberText.SetupAndStart(realSpawnPos,damageAmount,textColor, isCriticalHit,prefix);
        }
    }
}