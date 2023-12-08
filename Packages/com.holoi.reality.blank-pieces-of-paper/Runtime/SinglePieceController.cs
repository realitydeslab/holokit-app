// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
// SPDX-FileContributor: Sizheng Hao <sizheng@holoi.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Reality.BlankPiecesOfPaper
{
    public class SinglePieceController : MonoBehaviour
    {
        [SerializeField] Animator _animator;
        // Start is called before the first frame update
        void Start()
        {
            //_animator = GetComponent<Animator>();

            // Set Up UI Panel
            var uiPanel = FindObjectOfType<BlankPiecesOfPaperUIPanel>();
            uiPanel.SetUpUIPanel();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void AnimatorActive()
        {
            _animator.SetBool("Active", true);
        }

        public void AnimatorInActive()
        {
            _animator.SetBool("Active", false);
        }

        public void AnimatorToPage()
        {
            _animator.SetTrigger("ToPage");
        }
    }
}
