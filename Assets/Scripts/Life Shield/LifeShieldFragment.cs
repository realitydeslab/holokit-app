using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class LifeShieldFragment : MonoBehaviour
{
    [SerializeField] private LifeShield _lifeShield;

    [SerializeField] private LifeShieldArea _area;

    public void OnHit()
    {
        switch (_area)
        {
            case LifeShieldArea.Center:
                _lifeShield.TopDestroyed = true;
                _lifeShield.BotDestroyed = true;
                _lifeShield.LeftDestroyed = true;
                _lifeShield.RightDestroyed = true;
                break;
            case LifeShieldArea.Top:
                _lifeShield.TopDestroyed = true;
                break;
            case LifeShieldArea.TopRight:
                _lifeShield.TopDestroyed = true;
                _lifeShield.RightDestroyed = true;
                break;
            case LifeShieldArea.Right:
                _lifeShield.RightDestroyed = true;
                break;
            case LifeShieldArea.BotRight:
                _lifeShield.BotDestroyed = true;
                _lifeShield.RightDestroyed = true;
                break;
            case LifeShieldArea.Bot:
                _lifeShield.BotDestroyed = true;
                break;
            case LifeShieldArea.BotLeft:
                _lifeShield.BotDestroyed = true;
                _lifeShield.LeftDestroyed = true;
                break;
            case LifeShieldArea.Left:
                _lifeShield.LeftDestroyed = true;
                break;
            case LifeShieldArea.TopLeft:
                _lifeShield.TopDestroyed = true;
                _lifeShield.LeftDestroyed = true;
                break;
            default:
                break;
        }
    }
}
