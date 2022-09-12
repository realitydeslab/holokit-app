using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Mofa.Base
{
    public enum LifeShieldArea
    {
        Center = 0,
        Top = 1,
        TopRight = 2,
        Right = 3,
        BotRight = 4,
        Bot = 5,
        BotLeft = 6,
        Left = 7,
        TopLeft = 8
    }

    public class LifeShieldFragment : MonoBehaviour, IDamageable
    {
        public LifeShieldArea Area;

        private LifeShield _lifeShield;

        private void Awake()
        {
            _lifeShield = GetComponentInParent<LifeShield>();
        }

        public void OnHit()
        {
            switch (Area)
            {
                case LifeShieldArea.Center:
                    _lifeShield.TopDestroyed.Value = true;
                    _lifeShield.BotDestroyed.Value = true;
                    _lifeShield.LeftDestroyed.Value = true;
                    _lifeShield.RightDestroyed.Value = true;
                    break;
                case LifeShieldArea.Top:
                    _lifeShield.TopDestroyed.Value = true;
                    break;
                case LifeShieldArea.TopRight:
                    _lifeShield.TopDestroyed.Value = true;
                    _lifeShield.RightDestroyed.Value = true;
                    break;
                case LifeShieldArea.Right:
                    _lifeShield.RightDestroyed.Value = true;
                    break;
                case LifeShieldArea.BotRight:
                    _lifeShield.BotDestroyed.Value = true;
                    _lifeShield.RightDestroyed.Value = true;
                    break;
                case LifeShieldArea.Bot:
                    _lifeShield.BotDestroyed.Value = true;
                    break;
                case LifeShieldArea.BotLeft:
                    _lifeShield.BotDestroyed.Value = true;
                    _lifeShield.LeftDestroyed.Value = true;
                    break;
                case LifeShieldArea.Left:
                    _lifeShield.LeftDestroyed.Value = true;
                    break;
                case LifeShieldArea.TopLeft:
                    _lifeShield.TopDestroyed.Value = true;
                    _lifeShield.LeftDestroyed.Value = true;
                    break;
                default:
                    break;
            }
        }
    }
}