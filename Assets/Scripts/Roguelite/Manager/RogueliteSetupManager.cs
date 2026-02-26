using System.Collections;
using Cysharp.Threading.Tasks;
using Roguelite;
using ShabuStudio.Chapter;
using UnityEngine;

namespace ShabuStudio.Gameplay
{
    public class RogueliteSetupManager : MonoBehaviour
    {
        public Transform enemySpawnPoint;
        
        public async UniTask StartSetup(RogueliteStageData stageData,int stageIndex)
        {
            GameObject enemyObject = Instantiate(stageData.enemyPrefab, enemySpawnPoint,false);
            EnemyCombatEntity enemyData = enemyObject.GetComponent<EnemyCombatEntity>();
            if (enemyData == null)
            {
                enemyData = enemyObject.GetComponentInChildren<EnemyCombatEntity>();
            }
            enemyData.Initialize(GetNormalEnemyHP(stageIndex));
            RogueliteBattleStateManager.Instance.enemyUnit = enemyData;

            
            GameObject objectToSpawn = stageData?.chapterBg3DObject;
            if (objectToSpawn != null)
                Instantiate(objectToSpawn, Vector3.zero, Quaternion.identity);
            
            await UniTask.NextFrame();
        }
        
        //Calculate Fixed Enemy HP according to roguelite stage
        int GetNormalEnemyHP(int stageIndex)
        {
            int baseHP = 80;
            float baseMultiplier = 1.0f;
            float stageMultiplier = 0.05f;

            return (int)(baseHP * (baseMultiplier + (stageIndex * stageMultiplier)));
            
        }
    }
}