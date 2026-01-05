using System.Collections.Generic;
using UnityEngine;

namespace ShabuStudio.Gameplay.DoTSystem
{
    public class DoTsHolder
    {
        private CombatEntity holder;
        private List<DoT> doTs = new();
        
        public DoTsHolder(CombatEntity entity)
        {
            holder = entity;
        }
        
        
        public void ApplyDoT(DoT doT)
        {
            doTs.Add(doT);
        }

        public void TriggerDoT()
        {
            foreach (var doT in doTs)
            {
                doT.Trigger(holder);
            }
        }

        public void ReduceTurnsDoT()
        {
            foreach (var doT in doTs)
            {
                doT.ReduceTurns();
            }
            doTs.RemoveAll(x => x.turnsToEnd <= 0);
        }
        
        public bool HaveThisDot(DoT.DotType dotType)
        {
            return doTs.Find(x => x.dotType == dotType) != null;
        }
        
    }

    public class DoT
    {
        public enum DotType
        {
            Burn,
            Poison
        }
        
        public int damage;
        public int turnsToEnd;
        public DotType dotType;
        public Color damageColor;
        
        public DoT(int damage,int turnsToEnd,Color color,DotType dotType)
        {
            this.damage = damage;
            this.turnsToEnd = turnsToEnd;
            damageColor = color;
            this.dotType = dotType;
        }

        public virtual void Trigger(CombatEntity entity)
        {
            entity.TakeDamage(damage);

            Vector3 damageSpawnPoint;
            if (entity is PlayerCombatEntity)
            {
                damageSpawnPoint = CombatManager.Instance.damageSpawnPointPlayer.position;
            }
            else
            {
                damageSpawnPoint = CombatManager.Instance.damageSpawnPointEnemy.position;
            }

            DamageTextManager.Instance.SpawnText(damageSpawnPoint, damage,
                damageColor, false, "-");
        }

        public void ReduceTurns()
        {
            turnsToEnd--;
        }
    }
}