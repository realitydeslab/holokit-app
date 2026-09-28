// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Holoi.Library.HoloKitAppLib;
using Holoi.Library.HoloKitAppLib.UI;

namespace Holoi.Library.ARUX
{
    /// <summary>
    /// Starts when a hand dwells on it. Because hand tracking often misses, it also starts
    /// from the Star mode Trigger button, or from a tap on the button in phone mode.
    /// </summary>
    [RequireComponent(typeof(HoverableObject), typeof(Animator))]
    public class HoverableStartButton : MonoBehaviour
    {
        // Smallest on-screen tap target, in pixels, for a distant button.
        private const float MinTapRadiusPixels = 90f;

        private Animator _animator;

        private MeshRenderer _meshRenderer;

        private HoverableObject _hoverableObject;

        private void OnEnable()
        {
            HoloKitAppUIEventManager.OnStarUITriggered += TriggerWithoutHand;
        }

        private void OnDisable()
        {
            HoloKitAppUIEventManager.OnStarUITriggered -= TriggerWithoutHand;
        }

        private void Start()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _hoverableObject = GetComponent<HoverableObject>();
        }

        private void Update()
        {
            _meshRenderer.material.SetFloat("_Load", _hoverableObject.CurrentLoadPercentage);
            if (Input.GetMouseButtonDown(0))
            {
                TryTriggerFromScreenTap(Input.mousePosition);
            }
        }

        /// <summary>
        /// Starts immediately, as if a hand had finished dwelling on the button.
        /// </summary>
        public void TriggerWithoutHand()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }
            if (_hoverableObject == null)
            {
                _hoverableObject = GetComponent<HoverableObject>();
            }
            _hoverableObject.Trigger();
        }

        /// <summary>
        /// Phone mode: a tap on the button starts it. Taps on app UI or elsewhere are ignored.
        /// Returns whether the tap started the button.
        /// </summary>
        public bool TryTriggerFromScreenTap(Vector2 screenPosition)
        {
            if (!isActiveAndEnabled || IsOverUi(screenPosition))
            {
                return false;
            }
            var camera = Camera.main;
            if (camera == null)
            {
                return false;
            }
            if (_hoverableObject == null)
            {
                _hoverableObject = GetComponent<HoverableObject>();
            }
            var center = camera.WorldToScreenPoint(_hoverableObject.CenterPosition);
            if (center.z <= 0f)
            {
                return false;
            }
            var edge = camera.WorldToScreenPoint(_hoverableObject.CenterPosition + camera.transform.right * _hoverableObject.Radius);
            var radius = Mathf.Max(MinTapRadiusPixels, Vector2.Distance(center, edge));
            if (Vector2.Distance(screenPosition, center) > radius)
            {
                return false;
            }
            TriggerWithoutHand();
            return true;
        }

        private static bool IsOverUi(Vector2 screenPosition)
        {
            var eventSystem = EventSystem.current;
            if (eventSystem == null)
            {
                return false;
            }
            var hits = new List<RaycastResult>();
            eventSystem.RaycastAll(new PointerEventData(eventSystem) { position = screenPosition }, hits);
            return hits.Count > 0;
        }

        public void OnAppear()
        {
            gameObject.SetActive(true);
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
            _animator.Rebind();
            _animator.Update(0);
        }

        public void OnDisappear()
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
            _animator.SetTrigger("Die");
            StartCoroutine(HoloKitAppUtils.WaitAndDo(0.3f, () =>
            {
                gameObject.SetActive(false);
            }));
        }

        public void OnDeath()
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
            _animator.SetTrigger("Die");
            StartCoroutine(HoloKitAppUtils.WaitAndDo(0.3f, () =>
            {
                Destroy(gameObject);
            }));
        }
    }
}
