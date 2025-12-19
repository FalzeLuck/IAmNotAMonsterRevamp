using System.Collections;
using Cysharp.Threading.Tasks;
using ShabuStudio.Chapter;
using UnityEngine;

namespace ShabuStudio.Gameplay
{
    public class SetupManager : MonoBehaviour
    {
        public Transform enemySpawnPoint;

        public async UniTask StartSetup(StageData stageData)
        {
            GameObject enemyObject = Instantiate(stageData.enemyPrefab, enemySpawnPoint,false);
            EnemyCombatEntity enemyData = enemyObject.GetComponent<EnemyCombatEntity>();
            if (enemyData == null)
            {
                enemyData = enemyObject.GetComponentInChildren<EnemyCombatEntity>();
            }
            BattleStateManager.Instance.enemyUnit = enemyData;
            
            await UniTask.NextFrame();
        }
    }
}