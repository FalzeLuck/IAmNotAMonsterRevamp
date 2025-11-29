using UnityEditor;

namespace ShabuStudio.Data
{
    [CustomEditor(typeof(CardData))]
    public class CardDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
        
            // 1. Draw everything EXCEPT the script field and the buffs list
            // This replaces your while-loop and is much safer
            DrawPropertiesExcluding(serializedObject, "m_Script", "buffsToApply");
        
            // 2. Get the card type
            SerializedProperty cardTypeProp = serializedObject.FindProperty("canInflictBuff");
            bool canBuff = (bool)cardTypeProp.boolValue;
        
            // 3. Logic: Only draw the list if it is a BuffCard
            if (canBuff)
            {
                SerializedProperty buffsProp = serializedObject.FindProperty("buffsToApply");
                
                // The 'true' here ensures children (the actual buff data) are drawn
                EditorGUILayout.PropertyField(buffsProp, true); 
            }
        
            serializedObject.ApplyModifiedProperties();
        }
    }
}