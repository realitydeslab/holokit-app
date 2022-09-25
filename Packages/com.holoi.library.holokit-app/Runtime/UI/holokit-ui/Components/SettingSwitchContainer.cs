using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Holoi.Library.HoloKitApp.UI;

public class SettingSwitchContainer : MonoBehaviour
{
   public Button switchButton;

    private void Awake()
    {
        switchButton = transform.Find("Switch").GetComponent<Button>();

        switchButton.onClick.AddListener(() =>
        {
            if(switchButton.GetComponent<FlexibleUISwitch>().state == FlexibleUISwitch.State.Close)
            {
                switchButton.GetComponent<FlexibleUISwitch>().state = FlexibleUISwitch.State.Open;
            }
            else
            {
                switchButton.GetComponent<FlexibleUISwitch>().state = FlexibleUISwitch.State.Close;
            }

        });
    }
}
