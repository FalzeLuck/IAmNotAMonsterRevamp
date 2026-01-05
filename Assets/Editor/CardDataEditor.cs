using UnityEditor;

namespace ShabuStudio.Data
{
    [CustomEditor(typeof(CardData))]
    public class CardDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Start iterating from the very first property
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;

            // Loop through every property in the class
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false; // standard convention to handle the root object

                // 1. Skip drawing the Script field
                if (iterator.name == "m_Script") continue;

                // 2. Skip the Lists normally (so we don't draw them twice or in the wrong spot)
                if (iterator.name == "conditionalDamageBonus") continue;
                if (iterator.name == "buffsToApply") continue;

                // 3. Draw the current property
                EditorGUILayout.PropertyField(iterator, true);

                // 4. CHECK: Did we just draw the "Damage Toggle"?
                if (iterator.name == "haveConditionalDamageBonus" && iterator.boolValue)
                {
                    // If yes, immediately draw the damage list right here
                    SerializedProperty damageList = serializedObject.FindProperty("conditionalDamageBonus");
                    EditorGUILayout.PropertyField(damageList, true);
                }

                // 5. CHECK: Did we just draw the "Buff Toggle"?
                if (iterator.name == "canInflictBuff" && iterator.boolValue)
                {
                    // If yes, immediately draw the buff list right here
                    SerializedProperty buffList = serializedObject.FindProperty("buffsToApply");
                    EditorGUILayout.PropertyField(buffList, true);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}