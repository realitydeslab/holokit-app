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

        [HideInInspector] public LifeShield LifeShield;

        private void Awake()
        {
            LifeShield = GetComponentInParent<LifeShield>();
        }

        public void OnDamaged(ulong attackerClientId)
        {
            //Debug.Log($"[LifeShieldFragment] OnDamaged {Area}");
            LifeShield.LastAttackerClientId.Value = (int)attackerClientId;
            switch (Area)
            {
                case LifeShieldArea.Center:
                    LifeShield.TopDestroyed.Value = true;
                    LifeShield.BotDestroyed.Value = true;
                    LifeShield.LeftDestroyed.Value = true;
                    LifeShield.RightDestroyed.Value = true;
                    break;
                case LifeShieldArea.Top:
                    LifeShield.TopDestroyed.Value = true;
                    break;
                case LifeShieldArea.TopRight:
                    LifeShield.TopDestroyed.Value = true;
                    LifeShield.RightDestroyed.Value = true;
                    break;
                case LifeShieldArea.Right:
                    LifeShield.RightDestroyed.Value = true;
                    break;
                case LifeShieldArea.BotRight:
                    LifeShield.BotDestroyed.Value = true;
                    LifeShield.RightDestroyed.Value = true;
                    break;
                case LifeShieldArea.Bot:
                    LifeShield.BotDestroyed.Value = true;
                    break;
                case LifeShieldArea.BotLeft:
                    LifeShield.BotDestroyed.Value = true;
                    LifeShield.LeftDestroyed.Value = true;
                    break;
                case LifeShieldArea.Left:
                    LifeShield.LeftDestroyed.Value = true;
                    break;
                case LifeShieldArea.TopLeft:
                    LifeShield.TopDestroyed.Value = true;
                    LifeShield.LeftDestroyed.Value = true;
                    break;
                default:
                    break;
            }
        }
    }
}