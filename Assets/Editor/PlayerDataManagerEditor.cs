using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;

namespace ShabuStudio.Data
{
    [CustomEditor(typeof(PlayerDataManager))]
    [InitializeOnLoad]
    public class PlayerDataManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            PlayerDataManager playerData = target as PlayerDataManager;
            
            //Show only when playing
            if (Application.isPlaying)
            {
                GUILayout.Space(10); // Add a little gap for looks

                if (GUILayout.Button("Save All Decks"))
                {
                    playerData.SaveAllDecks();
                }
                if (GUILayout.Button("Load All Decks"))
                {
                    playerData.LoadAllDecks();
                }
            }
            else
            {
                GUILayout.Space(10);
                EditorGUILayout.HelpBox("Enter Play Mode to use Save/Load debugging.", MessageType.Info);
            }
        }
    }
}

#endif
