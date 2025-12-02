using UnityEngine;

namespace ShabuStudio.Camera
{
    [ExecuteAlways]
    public class ProjectionSetup : MonoBehaviour
    {
        public UnityEngine.Camera projectorCamera; // Assign the "Ghost Camera" here
        public Material targetMaterial; // Assign the Floor Material here
        public string matrixPropertyName = "_ProjectorVP"; // Must match Shader Graph property name

        void Update()
        {
            if (projectorCamera == null || targetMaterial == null) return;

            // 1. Get the View Matrix (World -> Camera)
            Matrix4x4 viewMat = projectorCamera.worldToCameraMatrix;
            

            // 2. Get the Projection Matrix (Camera -> Clip)
            // We use GL.GetGPUProjectionMatrix to handle graphics API differences automatically
            Matrix4x4 projMat = GL.GetGPUProjectionMatrix(projectorCamera.projectionMatrix, false);

            // 3. Combine them (Projection * View)
            Matrix4x4 viewProjMat = projMat * viewMat;

            // 4. Send to Shader
            targetMaterial.SetMatrix(matrixPropertyName, viewProjMat);
        }
    }
}