using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// panel manager, using a stack to pop and push panels for a smooth management
/// </summary>
namespace Holoi.HoloKit.App.UI
{
    public class PanelManager
    {
        private Stack<BasePanel> _panelStack;
        private UIManager _uiManager;
        private BasePanel _panel;

        public PanelManager()
        {
            _panelStack = new Stack<BasePanel>();
            _uiManager = new UIManager();
        }

        /// <summary>
        ///  excute it, a new ui shows up
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
                Debug.Log("_panelStack.Count = 0, do not need Pause the previous UI");
            }

            var panelGO = _uiManager.GetUIGO(nextPanel.UIType);
            nextPanel.Initialize(new UITool(panelGO));
            nextPanel.Initialize(this);
            nextPanel.Initialize(_uiManager);
            nextPanel.OnEnter();

            _panelStack.Push(nextPanel);
        }

        public void Pop()
        {
            if (_panelStack.Count > 0)
            {
                _panelStack.Peek().OnExit();
                _panelStack.Pop();
            }
            if (_panelStack.Count > 0)
                _panelStack.Peek().OnResume();
        }
    }
}
