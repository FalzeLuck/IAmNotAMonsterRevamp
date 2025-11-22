using UnityEngine;

public class CharacterFaceController : MonoBehaviour
{
    public Material characterMaterial; // Assign your Endfield_Material here
        public Transform headTransform;     // Drag your character's head bone here
    
        void Update()
        {
            if (characterMaterial != null && headTransform != null)
            {
                // Set the face direction to the head's forward vector
                characterMaterial.SetVector("_FaceDirection", headTransform.forward);
            }
        }
}
