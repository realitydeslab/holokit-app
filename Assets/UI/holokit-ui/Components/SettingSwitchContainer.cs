using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Holoi.Library.HoloKitApp.UI;

public class SettingSwitchContainer : MonoBehaviour
{
    Button _switchButton;
    private void Awake()
    {
        _switchButton = transform.Find("Switch").GetComponent<Button>();

        _switchButton.onClick.AddListener(() =>
        {
            if(_switchButton.GetComponent<FlexibleUISwitch>().state == FlexibleUISwitch.State.Close)
            {
                _switchButton.GetComponent<FlexibleUISwitch>().state = FlexibleUISwitch.State.Open;
            }
            else
            {
                _switchButton.GetComponent<FlexibleUISwitch>().state = FlexibleUISwitch.State.Close;
            }

        });
    }
}
