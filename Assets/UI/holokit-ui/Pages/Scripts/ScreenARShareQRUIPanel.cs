using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.HoloKit.App.UI
{
    public class ScreenARShareQRUIPanel : MonoBehaviour
    {
        public List<string> connectedDeviceNames;

        [Header("UI Elements")]
        [SerializeField] Transform _scrollView;
        [SerializeField] Transform _exitButton;
        [SerializeField] ConnectedDeviceContainer _devieceContainer;
        [SerializeField] Transform _devieceContainerSpacing;

        float _height1 = 1212;
        float _height2 = 1560;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UpdateConnectedDeviceUI()
        {
            if (connectedDeviceNames.Count > 0)
            {
                _devieceContainer.gameObject.SetActive(true);
                _devieceContainerSpacing.gameObject.SetActive(true);
                _scrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(1170, _height2);
                _exitButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-84, 1638);

                for (int i = 0; i < connectedDeviceNames.Count; i++)
                {
                    if (connectedDeviceNames[i] != null)
                    {
                        _devieceContainer.names[i].text.text = connectedDeviceNames[i];
                    }
                }
            }
            else
            {
                _devieceContainer.gameObject.SetActive(false);
                _devieceContainerSpacing.gameObject.SetActive(false);
                _scrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(1170, _height1);
                _exitButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(-84, 1290);
            }
        }
    }
}
