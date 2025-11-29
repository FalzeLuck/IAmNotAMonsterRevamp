using System.Collections.Generic;
using DefaultNamespace;
using ShabuStudio.Chapter;
using UnityEngine;

namespace ShabuStudio.Data
{
    public class StageDatabase : MonoBehaviour
    {
        public static StageDatabase Instance { get; private set; }
        
        private Dictionary<string, StageData> stageLookup = new Dictionary<string, StageData>();
        private bool isInitialized = false;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            InitializeDatabase();
        }

        void InitializeDatabase()
        {
            if(isInitialized) return;
            
            //Load all data
            StageData[] allStages = Resources.LoadAll<StageData>("Data/StageData");

            foreach (StageData stage in allStages)
            {
                if (!stageLookup.ContainsKey(stage.stageID))
                {
                    stageLookup.Add(stage.stageID, stage);
                }else
                {
                    Debug.LogWarning($"Duplicate stage ID found: {stage.stageID}");
                }
            }
            
            isInitialized = true;
            Debug.Log($"Database Loaded: {stageLookup.Count} stages found.");
        }

        public StageData GetStageByID(string id)
        {
            if (stageLookup.TryGetValue(id, out StageData stage))
            {
                return stage;
            }
            
            Debug.LogError($"Stage ID not found: {id}");
            return null;
        }

        public List<StageData> GetAllStagesByPrefix(string prefix)
        {
            List<StageData> stages = new List<StageData>();
            
            foreach (KeyValuePair<string, StageData> entry in stageLookup)
            {
                if (entry.Key.StartsWith(prefix))
                {
                    stages.Add(entry.Value);
                }
            }
            
            stages.Sort((a, b) => new NaturalStringComparer().Compare(a.stageID, b.stageID));

            if (stages.Count == 0)
            {
                Debug.LogWarning($"No stages found with prefix: {prefix}");
            }
            
            return stages;
        }
    }
}