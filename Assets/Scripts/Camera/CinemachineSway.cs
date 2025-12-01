using System;
using Unity.Cinemachine;
using UnityEngine;

namespace ShabuStudio.Camera
{
    [ExecuteInEditMode]
    [SaveDuringPlay]
    [AddComponentMenu("")]
    public class CinemachineSway : CinemachineExtension
    {
        [Header("Settings")] public float swayAmount = 5f;
        public float smoothSpeed = 2f;

        [Header("Safety")] public bool pauseOnTabOut = true;

        private Vector3 targetEuler;
        private Quaternion currentSwayRotation = Quaternion.identity;
        private Quaternion defaultRotation;

        private float xInput;
        private float yInput;

        private void Start()
        {
            defaultRotation = transform.rotation;
        }

        // We use Update to read Input (Input should not be read in the Cinemachine callback)
        void Update()
        {
            // SAFETY: Stop reading input if not focused (Alt-Tab protection)
            if (Application.isPlaying && pauseOnTabOut && !Application.isFocused) return;

            // Don't run input logic in Edit Mode
            if (!Application.isPlaying) return;

            CalculateInput();

            // Calculate the target rotation (Euler angles)
            // Inverting yInput for intuitive "Look Up" feel, or removing minus for "Flight" feel
            targetEuler = new Vector3(-yInput * swayAmount, xInput * swayAmount, 0);
        }

        void CalculateInput()
        {
            float mouseX = Input.mousePosition.x;
            float mouseY = Input.mousePosition.y;

            // Normalize -1 to 1
            float normalizedX = (mouseX / Screen.width) * 2 - 1;
            float normalizedY = (mouseY / Screen.height) * 2 - 1;

            // Clamp to prevent errors when mouse leaves window
            xInput = Mathf.Clamp(normalizedX, -1f, 1f);
            yInput = Mathf.Clamp(normalizedY, -1f, 1f);
        }

        // This is the special Cinemachine method that runs every frame
        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage,
            ref CameraState state,
            float deltaTime)
        {
            
            if (stage == CinemachineCore.Stage.Aim)
            {
                if (Application.isPlaying)
                {
                    Quaternion targetRot = Quaternion.Euler(targetEuler);
                    currentSwayRotation = Quaternion.Slerp(currentSwayRotation, targetRot, deltaTime * smoothSpeed);
                }

                Quaternion offset = defaultRotation * currentSwayRotation;
                // Apply the sway to the Cinemachine State
                state.RawOrientation = offset;
            }
        }
    }
}