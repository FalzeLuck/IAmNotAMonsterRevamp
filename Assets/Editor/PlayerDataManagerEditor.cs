using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;

namespace ShabuStudio.Data
{
    [CustomEditor(typeof(PlayerDeckDataManager))]
    [InitializeOnLoad]
    public class PlayerDataManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            PlayerDeckDataManager playerDeckData = target as PlayerDeckDataManager;
            
            //Show only when playing
            if (Application.isPlaying)
            {
                GUILayout.Space(10); // Add a little gap for looks

                if (GUILayout.Button("Save All Decks"))
                {
                    playerDeckData.SaveAllDecks();
                }
                if (GUILayout.Button("Load All Decks"))
                {
                    playerDeckData.LoadAllDecks();
                }
            }
            else
            {
                GUILayout.Space(10);
                if (GUILayout.Button("Save All Decks"))
                {
                    playerDeckData.SaveAllDecks(Path.Combine(Application.persistentDataPath, SaveSystem.DeckPath));
                }
                if (GUILayout.Button("Load All Decks"))
                {
                    playerDeckData.LoadAllDecks(Path.Combine(Application.persistentDataPath, SaveSystem.DeckPath));
                }
            }
        }
    }
}

#endif
