using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloKit;
using Holoi.AssetFoundation;

/// <summary>
/// panel manager, using a stack to pop and push panels for a smooth management
/// </summary>
namespace Holoi.Library.HoloKitApp.UI
{
    public class PanelManager
    {
        private static PanelManager _instance; // using instance to maintain the state of ui scenes.
        public static PanelManager Instance
        {
            get
            {
                if (null == _instance)
                {
                    _instance = new PanelManager();
                }
                return _instance;
            }
        }

        public Stack<BasePanel> _panelStack;
        public UIManager _uiManager;
        public GameObject _activePanel;
        private BasePanel _panel;
        public int _realityIndex = 0;


        public PanelManager()
        {
            _panelStack = new Stack<BasePanel>();
            _uiManager = new UIManager();
        }

        //[Header("")]
        public static Action OnStartedSharingReality;
        public static Action OnStoppedSharingReality;

        public static Action<HoloKit.HoloKitRenderMode> OnRenderModeChanged;
        public static Action OnExitReality;

        public static Action OnStartedRecording;
        public static Action OnStoppedRecording;


        public static Action<AssetFoundation.Reality> OnEnteredRealityAsHost;
        public static Action<AssetFoundation.Reality> OnJoinedRealityAsSpectator;

        // star action
        public static Action OnStARTriggered;
        public static Action OnStARBoosted;
        public static Action OnStARStartedPause;
        public static Action OnStARStoppedPause;

        /// <summary>
        ///  create a ui panel, and push it to stack
        /// </summary>
        /// <param name="nextPanel"> the panel you want to display</param>
        public void Push(BasePanel nextPanel)
        {
            if (_panelStack.Count > 0)
            {
                _panel = _panelStack.Peek();
                _panel.OnPause();
            }
            else
            {
                //Debug.Log("_panelStack.Count = 0, do not need Pause the previous UI");
            }

            Debug.Log("Push() panelGO Create");
            var panelGO = _uiManager.CreateUIGO(nextPanel.UIType);
            Debug.Log($"Push() panelGO Create Done with {panelGO.name}");

            nextPanel.Initialize(new UITool(panelGO));
            nextPanel.Initialize(this);
            nextPanel.Initialize(_uiManager);

            if (nextPanel.UITool.ActivePanelGO == null)
            {
                Debug.LogError("ActivePanelGO NULL!!!");
            }
            else
            {
                Debug.Log("push new panel with a go:" + nextPanel.UITool.ActivePanelGO.name);
            }
            
            nextPanel.OnOpen();

            _panelStack.Push(nextPanel);
        }

        public void Push(BasePanel nextPanel, bool disableOldPanel)
        {
            if (_panelStack.Count > 0)
            {
                _panel = _panelStack.Peek();
                _panel.OnPause(disableOldPanel);
            }
            else
            {
                //Debug.Log("_panelStack.Count = 0, do not need Pause the previous UI");
            }

            Debug.Log("Push() panelGO Create");
            var panelGO = _uiManager.CreateUIGO(nextPanel.UIType);
            Debug.Log($"Push() panelGO Create Done with {panelGO.name}");

            nextPanel.Initialize(new UITool(panelGO));
            nextPanel.Initialize(this);
            nextPanel.Initialize(_uiManager);

            if (nextPanel.UITool.ActivePanelGO == null)
            {
                Debug.LogError("ActivePanelGO NULL!!!");
            }
            else
            {
                Debug.Log("push new panel with a go:" + nextPanel.UITool.ActivePanelGO.name);
            }

            nextPanel.OnOpen();

            _panelStack.Push(nextPanel);
        }

        public void Pop()
        {
            if (_panelStack.Count > 0)
            {
                _panelStack.Peek().OnClose();
                _panelStack.Pop();
            }
            if (_panelStack.Count > 0)
            {
                _panelStack.Peek().OnResume();
            }
        }

        public void PopAll()
        {
            while(_panelStack.Count > 0)
            {
                _panelStack.Pop().OnClose();
            }
        }

        public BasePanel GetActivePanel()
        {
            if (_panelStack.Count > 0)
            {
               return _panel = _panelStack.Peek();
            }
            return null;
        }

        /// <summary>
        /// when switch back to ui scenes, it should be called and recover all ui state in new scene
        /// </summary>
        public void RecoverPanel()
        {
            
            int num = _panelStack.Count;
            var panelList = new BasePanel[num];

            for (int i = 0; i < num; i++)
            {
                Debug.Log($"pop panel with name {_panelStack.Peek().UIType.Name}");
                panelList[num - 1 - i] = _panelStack.Peek();
                _panelStack.Pop().OnClose();
            }

            Debug.Log($"panel number after pop {_panelStack.Count}");

            for (int i = 0; i < num; i++)
            {
                Debug.Log($"push panel with name {panelList[i].UIType.Name}");
                Push(panelList[i]);
                if (panelList[i].UIType.Name == "StartPanel")
                {
                    panelList[i].UITool.GetOrAddComponent<HomeUIPanel>().RecoverHomePage();
                }
            }

            Debug.Log($"recover panels with a number{num}");
        }
    }
}
