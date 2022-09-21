using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    public class PermissionsPanel : MonoBehaviour
    {
        [SerializeField] Button _camButton;
        [SerializeField] Button _microButton;
        [SerializeField] Button _photoButton;
        [SerializeField] Button _locationButton;

        private void Start()
        {
            // yc todo:
            _camButton.onClick.AddListener(() => {
                // todo:
            });
        }

        void UpdateAllPermisionButtons()
        {
            if (true)
            {
                _camButton.GetComponent<FlexibleUIPermissionButton>().state = FlexibleUIPermissionButton.GoOnState.Checked;
            }
            else
            {
                _camButton.GetComponent<FlexibleUIPermissionButton>().state = FlexibleUIPermissionButton.GoOnState.Uncheck;
            }

            if (true)
            {
                _microButton.GetComponent<FlexibleUIPermissionButton>().state = FlexibleUIPermissionButton.GoOnState.Checked;
            }
            else
            {
                _microButton.GetComponent<FlexibleUIPermissionButton>().state = FlexibleUIPermissionButton.GoOnState.Uncheck;
            }

            if (true)
            {
                _photoButton.GetComponent<FlexibleUIPermissionButton>().state = FlexibleUIPermissionButton.GoOnState.Checked;
            }
            else
            {
                _photoButton.GetComponent<FlexibleUIPermissionButton>().state = FlexibleUIPermissionButton.GoOnState.Uncheck;
            }

            if (true)
            {
                _locationButton.GetComponent<FlexibleUIPermissionButton>().state = FlexibleUIPermissionButton.GoOnState.Checked;
            }
            else
            {
                _locationButton.GetComponent<FlexibleUIPermissionButton>().state = FlexibleUIPermissionButton.GoOnState.Uncheck;
            }
        }
    }
}
