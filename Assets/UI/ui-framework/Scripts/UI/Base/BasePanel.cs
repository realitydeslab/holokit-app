using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.HoloKit.App.UI
{
    public abstract class BasePanel
    {
        public UIType UIType { get; private set; }
        public UITool UITool { get; private set; }

        public PanelManager PanelManager { get; private set; }
        public UIManager UIManager { get; private set; }

        public BasePanel(UIType uiType)
        {
            UIType = uiType;
        }

        public void Initialize(UITool tool)
        {
            UITool = tool;
        }

        public void Initialize(PanelManager panelManager)
        {
            //PanelManager = panelManager;
            PanelManager = PanelManager.Instance;
        }

        public void Initialize(UIManager uiManager)
        {
            UIManager = uiManager;
        }

        public virtual void OnOpen()
        {

        }

        public virtual void OnPause()
        {
            Debug.Log($"{UIType.Name} panel is paused");
            if (UITool == null)
            {
                Debug.LogError($"{UIType.Name}: not found UITool");
            }
            else if (UITool.GetOrAddComponent<CanvasGroup>() == null)
            {
                Debug.Log($"warning: {UIType.Name} not found CanvasGroup");
            }
            else
            {
                UITool.GetOrAddComponent<CanvasGroup>().blocksRaycasts = false;
            }
        }

        public virtual void OnResume()
        {
            Debug.Log($"{UIType.Name} panel is resume");

            if (UITool == null)
            {
                Debug.LogError("not found UITool");
            }
            else if(UITool.GetOrAddComponent<CanvasGroup>() == null)
            {
                Debug.Log($"warning: {UIType.Name} not found CanvasGroup");

            }
            else
            {
                UITool.GetOrAddComponent<CanvasGroup>().blocksRaycasts = true;
            }
        }
        public virtual void OnClose()
        {
            Debug.Log($"{UIType.Name} close");
            UIManager.DestroyUI(UIType);
        }
    }
}

