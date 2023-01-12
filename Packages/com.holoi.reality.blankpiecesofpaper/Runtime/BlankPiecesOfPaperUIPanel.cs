using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace Holoi.Reality.BlankPiecesOfPaper
{
    public class BlankPiecesOfPaperUIPanel : MonoBehaviour
    {
        [SerializeField] TMP_InputField InputField;
        [SerializeField] Button createButton;
        [SerializeField] Button activeButton;
        [SerializeField] Button inactiveButton;
        [SerializeField] Button toPageButton;


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
            activeButton.onClick.AddListener(spc.AnimatorActive);
            inactiveButton.onClick.AddListener(spc.AnimatorInActive);
            toPageButton.onClick.AddListener(spc.AnimatorToPage);
        }
    }
}
