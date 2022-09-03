using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.HoloKit.App.UI
{
    public class RealityPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/RealityPanel";
        public RealityPanel() : base(new UIType(_path)) { }

        public Holoi.AssetFoundation.Reality RealityData;

        public override void OnEnter()
        {

            SetInitialValue();

            UITool.GetOrAddComponentInChildren<Button>("BackButton").onClick.AddListener(() =>
            {
            // here we do onclick event of this button
            Debug.Log("BackButton is clicked.");

                HomeUIPanel HUP = Transform.FindObjectOfType<HomeUIPanel>();
                if (HUP != null)
                {
                    HUP.SwitchToHomePageLayout();
                }
                else
                {
                    Debug.LogError("Not Found HUP");
                }


                PanelManager.Pop();
            });
            UITool.GetOrAddComponentInChildren<Button>("PlayButton").onClick.AddListener(() =>
            {
            // here we do onclick event of this button
            Debug.Log("PlayButton is clicked.");

                var panel = new RealityOptionPanel();
                PanelManager.Push(panel);
            });
        }

        void SetInitialValue()
        {
            UITool.FindChildGameObject("Index").GetComponent<TMPro.TMP_Text>().text = "Reality " + "#00" + RealityData.realityId;
            UITool.FindChildGameObject("Name").GetComponent<TMPro.TMP_Text>().text = RealityData.displayName;
            UITool.FindChildGameObject("Version").GetComponent<TMPro.TMP_Text>().text = RealityData.version;
            UITool.FindChildGameObject("LastUpdate").GetComponent<TMPro.TMP_Text>().text = "2022.09.02";
            UITool.FindChildGameObject("Author").GetComponent<TMPro.TMP_Text>().text = RealityData.author;
            UITool.FindChildGameObject("Description").GetComponent<TMPro.TMP_Text>().text = RealityData.description;

            string tagString = "";
            foreach (var tag in RealityData.techTags)
            {
                tagString += tag.ToString();
                tagString += "; ";
            }

            UITool.FindChildGameObject("Technic").GetComponent<TMPro.TMP_Text>().text = tagString;
        }
    }
}