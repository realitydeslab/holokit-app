using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public enum MagicSchool
{
    MysticArt = 0
}

public enum MagicSpellType
{
    Primary = 0,
    Secondary = 1
}

public class MagicSpell : NetworkBehaviour
{
    [SerializeField] private int _index;

    [SerializeField] private string _name;

    [SerializeField] private MagicSchool _magicSchool;

    [SerializeField] private MagicSpellType _magicSpellType;

    [SerializeField] private Vector3 _positionOffset;

    [SerializeField] private bool _isPerpendicularToTheGround;

    [SerializeField] private float _chargeTime;

    [SerializeField] private int _maxChargeCount;

    [SerializeField] private int _maxUsageCount;

    public int Index => _index;

    public string Name => _name;

    public MagicSchool MagicSchool => _magicSchool;

    public MagicSpellType MagicSpellType => _magicSpellType;

    public Vector3 PositionOffset => _positionOffset;

    public bool IsPerpendicularToTheGround => _isPerpendicularToTheGround;

    public float ChargeTime => _chargeTime;

    public int MaxChargeCount => _maxChargeCount;

    public int MaxUsageCount => _maxUsageCount;
}
