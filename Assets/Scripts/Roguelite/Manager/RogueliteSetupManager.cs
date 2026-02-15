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
        
        public async UniTask StartSetup(RogueliteStageData stageData)
        {
            GameObject enemyObject = Instantiate(stageData.enemyPrefab, enemySpawnPoint,false);
            EnemyCombatEntity enemyData = enemyObject.GetComponent<EnemyCombatEntity>();
            if (enemyData == null)
            {
                enemyData = enemyObject.GetComponentInChildren<EnemyCombatEntity>();
            }
            RogueliteBattleStateManager.Instance.enemyUnit = enemyData;

            
            GameObject objectToSpawn = stageData?.chapterBg3DObject;
            if (objectToSpawn != null)
                Instantiate(objectToSpawn, Vector3.zero, Quaternion.identity);
            
            await UniTask.NextFrame();
        }
    }
}