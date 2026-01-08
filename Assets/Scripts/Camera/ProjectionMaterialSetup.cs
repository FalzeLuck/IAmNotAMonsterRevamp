using System;
using UnityEngine;

namespace ShabuStudio.Camera
{
    public class ProjectionMaterialSetup : MonoBehaviour
    {
        public Material targetMaterial;
        public string mainTexPropertyName = "_MainTex";

        private void Start()
        {
            if (GameManager.Instance.currentChapterData != null)
            {
                targetMaterial.SetTexture(mainTexPropertyName,GameManager.Instance.currentChapterData.chapterBgImage);
            }
        }
    }
}