using Apple.CoreHaptics;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Diagnostics;
using System.Numerics;

public class HapticsTest : MonoBehaviour
{
    private CHHapticEngine _hapticsEngine;

    private CHHapticAdvancedPatternPlayer _textureHapticPlayer;
    private CHHapticPatternPlayer _smallCollisionHapticPlayer;
    private CHHapticPatternPlayer _largeCollisionHapticPlayer;
    private CHHapticAdvancedPatternPlayer _implodeHapticPlayer;

    [SerializeField] private TextAsset _hapticsAHAP;
    [SerializeField] private TextAsset _haptics2AHAP;
    [SerializeField] private TextAsset _haptics3AHAP;

    public void Awake()
    {
        _hapticsEngine = new CHHapticEngine { IsAutoShutdownEnabled = false };
        _hapticsEngine.Start(); 
    }

    public void Start()
    {
    }


    public void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            _hapticsEngine.Stop();
        }
        else
        {
            _hapticsEngine.Start();
        }
    }

    //private void SetupHapticPlayers()
    //{
    //    _textureHapticPlayer = _engine.MakeAdvancedPlayer(new CHHapticPattern(_rollingTextureAHAP));
    //    _textureHapticPlayer.LoopEnabled = true;
    //    _textureHapticPlayer.LoopEnd = 0f;

    //    _smallCollisionHapticPlayer = _engine.MakePlayer(new CHHapticPattern(_smallCollisionAHAP));
    //    _largeCollisionHapticPlayer = _engine.MakePlayer(new CHHapticPattern(_largeCollisionAHAP));

    //    _implodeHapticPlayer = _engine.MakeAdvancedPlayer(new CHHapticPattern(_implodeAHAP));
    //    _implodeHapticPlayer.CompletionHandler += ImplosionHapticCompletion;
    //}



    private void UpdateTextureIntensity()
    {
        var currentSpeed = _rigidbody.velocity.magnitude;
        var intensity = Math.Min(currentSpeed / _maximumReasonableVelocity, 1f);
        var hapticParameters = new List<CHHapticParameter>
            {
                new CHHapticParameter(
                    parameterId: CHHapticDynamicParameterID.HapticIntensityControl,
                    parameterValue: intensity
                )
            };

        Debug.Log($"Sending intensity {intensity} to texture player.");
        _textureHapticPlayer.SendParameters(hapticParameters);
    }
}
