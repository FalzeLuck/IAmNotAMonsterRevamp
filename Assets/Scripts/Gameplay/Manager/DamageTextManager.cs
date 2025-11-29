using System.Collections.Generic;
using UnityEngine;

namespace ShabuStudio.Gameplay
{
    public class DamageTextManager : MonoBehaviour
    {
        [SerializeField] private DamageText damageTextPrefab;
        [SerializeField] private Transform damageTextCanvasTransform;
        private List<DamageText> _damageTextList = new List<DamageText>();
        

        public void Initialize()
        {
            
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