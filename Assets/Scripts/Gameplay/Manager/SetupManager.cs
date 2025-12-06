using System.Collections;
using ShabuStudio.Chapter;
using UnityEngine;

namespace ShabuStudio.Gameplay
{
    public class SetupManager : MonoBehaviour
    {
        public Transform enemySpawnPoint;

        public IEnumerator StartSetup(StageData stageData)
        {
            GameObject enemyObject = Instantiate(stageData.enemyPrefab, enemySpawnPoint,false);
            EnemyCombatEntity enemyData = enemyObject.GetComponent<EnemyCombatEntity>();
            if (enemyData == null)
            {
                enemyData = enemyObject.GetComponentInChildren<EnemyCombatEntity>();
            }
            BattleStateManager.Instance.enemyUnit = enemyData;
            
            
            yield return null;
        }
    }
}