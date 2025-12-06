using TMPro;
using UnityEngine;

[ExecuteAlways]
public class SimpleTextCurve : MonoBehaviour
{
    [SerializeField] private TMP_Text textComponent;
    
    [Tooltip("The radius of the circle. Larger = flatter curve.")]
    [SerializeField] private float radius = 500f; 
    
    [Tooltip("If true, letters curve down (frown). If false, they curve up (smile).")]
    [SerializeField] private bool curveDown = true;

    // We use a simpler Update approach for UI to avoid glitches
    void Update()
    {
        if (textComponent == null) return;
        
        // This forces TMP to regenerate the mesh so we can modify it
        textComponent.ForceMeshUpdate();

        TMP_TextInfo textInfo = textComponent.textInfo;
        int characterCount = textInfo.characterCount;

        if (characterCount == 0) return;

        // Calculate the bounds to center the curvature
        float boundsMinX = textComponent.bounds.min.x;
        float boundsMaxX = textComponent.bounds.max.x;
        float textWidth = boundsMaxX - boundsMinX;
        float centerX = (boundsMinX + boundsMaxX) / 2f;

        // Process every character
        for (int i = 0; i < characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int vertexIndex = charInfo.vertexIndex;
            int materialIndex = charInfo.materialReferenceIndex;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            // Apply transformation to all 4 vertices of the character
            for (int j = 0; j < 4; j++)
            {
                Vector3 orig = vertices[vertexIndex + j];
                
                // 1. Shift X so the text is centered around 0 for calculation
                float xOffset = orig.x - centerX;
                
                // 2. Convert that linear X distance into an angle along the circle
                // Arc Length formula: s = r * theta  ->  theta = s / r
                float angle = xOffset / radius;

                // 3. Calculate the new position on the circle edge
                // We add orig.y to the radius so the top of the letter is further out than the bottom
                float heightFromCenter = radius + (curveDown ? -orig.y : orig.y);

                float newX = Mathf.Sin(angle) * heightFromCenter;
                float newY = Mathf.Cos(angle) * heightFromCenter;

                // 4. Adjust positioning based on curve direction
                if (curveDown)
                {
                    // Frown shape: Center is below, we go up
                    // We subtract radius to bring the center of the text back to Y=0
                    vertices[vertexIndex + j] = new Vector3(newX + centerX, radius - newY, 0);
                }
                else
                {
                    // Smile shape: Center is above
                    vertices[vertexIndex + j] = new Vector3(newX + centerX, -radius + newY, 0);
                }
            }
        }

        // Apply changes to the mesh
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }

    private void OnValidate()
    {
        if (textComponent == null) textComponent = GetComponent<TMP_Text>();
    }
}
