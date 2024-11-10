// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using Apple.CoreHaptics;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Numerics;

public class HapticsTest : MonoBehaviour
{
    private CHHapticEngine _hapticsEngine;

    private CHHapticAdvancedPatternPlayer _textureHapticPlayer;
    private CHHapticPatternPlayer _smallCollisionHapticPlayer;
    private CHHapticPatternPlayer _largeCollisionHapticPlayer;
    private CHHapticAdvancedPatternPlayer _implodeHapticPlayer;

   [SerializeField] Scrollbar _sb;


    [SerializeField] private TextAsset[] _hapticsList = new TextAsset[12];
    [SerializeField] List<TextAsset> _textureHapticsList = new List<TextAsset>();

    //[SerializeField] private TextAsset _hapticsAHAP;
    //[SerializeField] private TextAsset _haptics2AHAP;
    //[SerializeField] private TextAsset _haptics3AHAP;

    int _index = 0;
    int _indexTexture = 0;
    bool _playTexture = false;

    public void Awake()
    {
        _hapticsEngine = new CHHapticEngine { IsAutoShutdownEnabled = false };
        _hapticsEngine.Start(); 
    }

    public void Start()
    {
        SetupHapticAdvancedPlayers(_textureHapticsList[0]);
		_textureHapticPlayer.Start();
    }

    void FixedUpdate(){
    }

    private void SetupHapticAdvancedPlayers(TextAsset textureAHAP)
	{
		_textureHapticPlayer = _hapticsEngine.MakeAdvancedPlayer(new CHHapticPattern(textureAHAP));
		_textureHapticPlayer.LoopEnabled = true;
		_textureHapticPlayer.LoopEnd = 0f;
    }

    private void UpdateTextureIntensity()
	{
		// var currentSpeed = _rigidbody.velocity.magnitude;
		var intensity = _sb.value * 1f;
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


    public void OnApplicationPause(bool pause)
    {
        if(_hapticsEngine!=null){
        if (pause)
        {
            _hapticsEngine.Stop();
        }
        else
        {
            _hapticsEngine.Start();
        }
        }

    }

    public void PlayTexture()
    {
        _indexTexture++;
        if (_indexTexture > _textureHapticsList.Count) _indexTexture = 0;

        _textureHapticPlayer.Destroy();
        // set new player
        SetupHapticAdvancedPlayers(_textureHapticsList[_indexTexture]);

        _textureHapticPlayer.Start();

    }

    public void Play()
    {

        _index++;
        if (_index > 20) _index = 0;

        if (!(_hapticsList[_index] is null))
        {
            _hapticsEngine.PlayPatternFromAhap(_hapticsList[_index]);
        }
    }
}
