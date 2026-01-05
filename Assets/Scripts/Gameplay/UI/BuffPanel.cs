using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShabuStudio.Gameplay.UI
{
    public class BuffPanel : MonoBehaviour
    {
        public GameObject buffPrefab;
        
        private List<BuffDataForShow> buffsForShow = new List<BuffDataForShow>();
        public void UpdatePanel(Buff buffForShow)
        {
            BuffDataForShow showingBuff = new BuffDataForShow(buffForShow.buffName,buffForShow.buffDescription,buffForShow.buffTurnsToEnd,buffForShow.buffIcon);
            buffsForShow.Add(showingBuff);
            
            GameObject buffIcon = Instantiate(buffPrefab,transform);
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