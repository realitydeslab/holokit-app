using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace Holoi.Reality.BlankPiecesOfPaper
{
    public class TypedRealityBlankPiecesofPaperUIPanel : MonoBehaviour
    {
        [SerializeField] TMP_InputField InputField;
        [SerializeField] Button _createButton;
        [SerializeField] Button _activeButton;
        [SerializeField] Button _inactiveButton;
        [SerializeField] Button _toPageButton;


        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void SetUpUIPanel()
        {
            var spc = FindObjectOfType<SinglePieceController>();
            //_createButton.onClick.AddListener()
            _activeButton.onClick.AddListener(spc.AnimatorActive);
            _inactiveButton.onClick.AddListener(spc.AnimatorInActive);
            _toPageButton.onClick.AddListener(spc.AnimatorToPage);
        }
    }
}
