// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchenz27@outlook.com>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealityDesignLab.MOFA.TheHunting
{
    public class PolyAvatarController : MonoBehaviour
    {
        [SerializeField] private Transform _room;

        [SerializeField] private Animator _animator;

        [SerializeField] private GameObject _magicBallPrefab;

        [SerializeField] private float _castSpellDelay = 0;

        [SerializeField] private float _castSpellInterval = 3f;

        private void Start()
        {
            StartCoroutine(PeriodicCastSpell());
        }

        private IEnumerator PeriodicCastSpell()
        {
            yield return new WaitForSeconds(_castSpellDelay);
            while(true)
            {
                _animator.SetTrigger("Attack A");
                yield return new WaitForSeconds(0.25f);
                SpawnMagicBall();
                yield return new WaitForSeconds(_castSpellInterval);
            }
        }

        private void SpawnMagicBall()
        {
            var magicBall = Instantiate(_magicBallPrefab, _room);
            magicBall.transform.LookAt(transform.forward * 10f);
            magicBall.transform.position = transform.position + Vector3.up * 1.5f + transform.forward * 1f;
            magicBall.GetComponent<Rigidbody>().velocity = transform.forward * 3f;
        }
    }
}
