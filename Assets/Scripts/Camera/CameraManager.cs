using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

namespace ShabuStudio.Camera
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance { get; private set; }
        
        public List<CinemachineCamera> cinemachineCameras;
        private CinemachineCamera currentCamera;
        [SerializeField]private CinemachineBrain brain;
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            cinemachineCameras = new List<CinemachineCamera>(
                FindObjectsByType<CinemachineCamera>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID));

            currentCamera = brain.ActiveVirtualCamera as CinemachineCamera;
            
        }


        /// <summary>
        /// Switches the active camera to the specified camera by name.
        /// </summary>
        /// <param name="cameraName">The name of the camera to switch to.</param>
        public void SwitchCamera(string cameraName)
        {
            currentCamera = cinemachineCameras.Find(camera => camera.name == cameraName);

            foreach (var camera in cinemachineCameras)
            {
                camera.Priority = camera == currentCamera ? 1 : 0;
            }
        }

        /// <summary>
        /// Changes the target for the camera to look at and follow.
        /// </summary>
        /// <param name="target">The Transform of the new target for the camera.</param>
        public void ChangeCameraTarget(Transform target)
        {
            currentCamera.LookAt = target;
            currentCamera.Follow = target;
        }

        /// <summary>
        /// Changes the LookAt target of the currently active Cinemachine camera.
        /// </summary>
        /// <param name="target">The Transform to set as the LookAt target for the camera.</param>
        public void ChangeCameraLookAt(Transform target)
        {
            currentCamera.LookAt = target;
        }

        /// <summary>
        /// Updates the camera's follow target to the provided transform.
        /// </summary>
        /// <param name="target">The transform to set as the new follow target for the current camera.</param>
        public void ChangeCameraFollow(Transform target)
        {
            currentCamera.Follow = target;
        }
    }
}