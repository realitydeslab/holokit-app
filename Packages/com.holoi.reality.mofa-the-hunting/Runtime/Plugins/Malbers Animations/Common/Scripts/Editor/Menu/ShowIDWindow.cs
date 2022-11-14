using System.Collections.Generic;
using UnityEngine;
using UnityEditor; 
using System.Linq;

namespace MalbersAnimations
{ 
    public class ShowIDWindow : EditorWindow
    {

        [MenuItem("Tools/Malbers Animations/Show All IDs")]
        public static ShowIDWindow ShowWindow()
        {
            //Editor_Tabs1 = tab;
            var example = (ShowIDWindow)GetWindow(typeof(ShowIDWindow), true, "All Malbers [ID]");
            example.Show();
            return example;
        }

        private List<MAction> actions;
        private List<StanceID> stances;
        private List<Tag> tags;
        private List<ModeID> modes;
        private List<StateID> states;
        private Vector2 Scroll;
        public int Editor_Tabs1;

        private void OnEnable()
        {
            modes = MTools.GetAllInstances<ModeID>();
            states = MTools.GetAllInstances<StateID>();
            stances = MTools.GetAllInstances<StanceID>();
            tags = MTools.GetAllInstances<Tag>();
            actions = MTools.GetAllInstances<MAction>();


            actions = actions.OrderBy(x => x.ID).ToList();
            modes = modes.OrderBy(x => x.ID).ToList();
            stances = stances.OrderBy(x => x.ID).ToList();
            states = states.OrderBy(x => x.ID).ToList();
            tags = tags.OrderBy(x => x.name).ToList();
        }

        private void OnGUI()
        {
            Editor_Tabs1 = GUILayout.Toolbar(Editor_Tabs1, new string[] { "States", "Modes", "Stances", "Actions", "Tags" });

            using (var X = new GUILayout.ScrollViewScope(Scroll))
            {
                Scroll = X.scrollPosition;

                switch (Editor_Tabs1)
                {
                    case 0: foreach (var item in states) DrawID(item); break;
                    case 1: foreach (var item in modes) DrawID(item); break;
                    case 2: foreach (var item in stances) DrawID(item); break;
                    case 3: foreach (var item in actions) DrawID(item); break;
                    case 4: foreach (var item in tags) DrawID(item); break;
                    default:
                        break;
                }
            }
        }

        private static void DrawID(IDs item)
        {
            using (new GUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledGroupScope(true))
                {
                    EditorGUILayout.ObjectField(item, typeof(IDs), false);
                    EditorGUILayout.IntField(item.ID, GUILayout.Width(80));
                }
            }
        }
    }
}
