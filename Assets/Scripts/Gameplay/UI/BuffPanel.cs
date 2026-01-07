using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShabuStudio.Gameplay.UI
{
    public class BuffPanel : MonoBehaviour
    {
        public GameObject buffPrefab;
        
        List<ActiveBuff> activeBuffs = new List<ActiveBuff>();
        
        public void AddBuffToPanel(Buff buffForShow)
        {
            
            //Add buff icon only buff that have turnsToEnd
            if(buffForShow.buffTurnsToEnd > 0)
            {
                BuffDataForShow showingBuff = new BuffDataForShow(buffForShow.buffName,buffForShow.buffDescription,buffForShow.buffTurnsToEnd,buffForShow.buffIcon);
                GameObject buffIcon = Instantiate(buffPrefab, transform);
                
                ActiveBuff activeBuff = new ActiveBuff(showingBuff,buffIcon);
                activeBuffs.Add(activeBuff);
                
            }
        }
        
        public void DecreaseBuffTurnInPanel()
        {
            foreach (var buff in activeBuffs)
            {
                buff.buffData.buffTurns--;
            }
            
            ActiveBuff[] buffsToRemove = activeBuffs.FindAll(buff => buff.buffData.buffTurns <= 0).ToArray();
            //Remove buff that have 0 or less turns.
            activeBuffs.RemoveAll(buff => 
            {
                if (buff.buffData.buffTurns <= 0)
                {
                    if (buff.buffIconObj != null) 
                    {
                        Destroy(buff.buffIconObj);
                    }
        
                    return true; 
                }
                return false; 
            });
        }
    }

    public class ActiveBuff
    {
        public BuffDataForShow buffData;
        public GameObject buffIconObj;
        
        public ActiveBuff(BuffDataForShow buffData, GameObject buffIconObj)
        {
            this.buffData = buffData;
            this.buffIconObj = buffIconObj;
        }
    }
    
    public class BuffDataForShow
    {
        public string buffName;
        public string buffDescription;
        public int buffTurns;
        public Sprite icon;
        
        public BuffDataForShow(string name, string description, int turns, Sprite icon)
        {
            buffName = name;
            buffDescription = description;
            buffTurns = turns;
            this.icon = icon;
        }
    }
}