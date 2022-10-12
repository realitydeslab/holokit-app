using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using HoloKit;

namespace Holoi.Reality.TypoGraphy
{
    public class MeshTypographySceneManager : MonoBehaviour
    {
        [SerializeField] Button _onMeshingDoneButton;
        [SerializeField] GameObject _onStartTip;

        private void Awake()
        {
            var _videoEnhancementMode = VideoEnhancementMode.HighResWithHDR;
            HoloKitARSessionControllerAPI.SetVideoEnhancementMode(_videoEnhancementMode);
        }
        void Start()
        {
            _onMeshingDoneButton.interactable = false;

            StartCoroutine(WaitAndEnableMeshingDoneButton());

            //FindObjectOfType<ARPlaneManager>(true).enabled = true;
            //FindObjectOfType<ARMeshManager>(true).enabled = true;
            //FindObjectOfType<ToggleMeshClassification>(true).enabled = true;
        }

        public void OnMeshingDone()
        {
            FindObjectOfType<ARPlaneManager>(true).enabled = false;
            FindObjectOfType<ARMeshManager>(true).enabled = false;
            FindObjectOfType<ToggleMeshClassification>(true).enabled = false;
        }

        IEnumerator WaitAndEnableMeshingDoneButton()
        {
            yield return new WaitForSeconds(4.5f);
            _onStartTip.gameObject.SetActive(false);
            _onMeshingDoneButton.interactable = true;
        }
    }
}