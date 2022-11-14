using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MalbersAnimations.Scriptables
{
    [AddComponentMenu("Malbers/Utilities/Managers/Scritable Var to Player Pref")]
    public class ScriptableVarToPlayerPref : MonoBehaviour
    {
        [Tooltip("Set of Scriptable variables you want to save on Player Pref")]
        public ScriptableVar[] userPreferences;
        [CreateScriptableAsset] 
        [Tooltip("Restore the Array of Variables to their default Options")]
        public ResetScriptableVarsAsset defaultUserOptions;
        [Tooltip("All values will be save to <PlayerPref> On Disable")]
        public bool SaveOnExit = true;
        public bool debug = true;

        void Start()
        {
            switch (PlayerPrefs.GetInt("GameInitalized"))
            {
                case 0:  // not intialized
                    PlayerPrefs.SetInt("GameInitalized", 1);
                    defaultUserOptions?.Restart();
                    SaveUserPreferences();
                    break;
                default:
                    GetUserPreferences();
                    break;
            }

            DontDestroyOnLoad(this);
        }

        private void OnDisable()
        {
            if (SaveOnExit) SaveUserPreferences();
        }


        /// <summary>Restore all the variables to their Default Values</summary>
        public void RestoreToDefault()
        {
            defaultUserOptions.Restart();
        }

        /// <summary>  uses Scriptable Varible name as reference  </summary>
        public void GetUserPreferences()
        {
            foreach (ScriptableVar userPreference in userPreferences)
            {
                var val = "";

                if (userPreference is IntVar)
                {
                    var result = PlayerPrefs.GetInt(userPreference.name);
                    (userPreference as IntVar).Value = result;
                    val = result.ToString();
                }
                else if (userPreference is BoolVar)
                {
                    var result = StringToBool(PlayerPrefs.GetString(userPreference.name));
                    (userPreference as BoolVar).Value = result;
                    val = result.ToString();
                }
                else if (userPreference is FloatVar)
                {
                    var result = PlayerPrefs.GetFloat(userPreference.name);
                    (userPreference as FloatVar).Value = result;
                    val = result.ToString();
                }
                else if (userPreference is StringVar)
                {
                    var result = PlayerPrefs.GetString(userPreference.name);
                    (userPreference as StringVar).Value = result;
                    val = result.ToString();
                }
                else
                {
                    Debug.LogError("Unacceptable ScriptableVar used: " + userPreference.name);
                }

                if (debug) Debug.Log($"Get Value From Player Pref: {userPreference.name} -> [{val}]",this);
            }
        }

        /// <summary> Stores to PlayerPrefs using Scriptable Varible name as reference  </summary>
        public void SaveUserPreferences() 
        {
            foreach (ScriptableVar userPreference in userPreferences)
            {
                var val = "";

                if (userPreference is IntVar)
                {
                    var result = (userPreference as IntVar).Value;
                    PlayerPrefs.SetInt(userPreference.name, result);
                    val = result.ToString();
                }
                else if (userPreference is BoolVar)
                {
                    var result = (userPreference as BoolVar).Value.ToString();
                    PlayerPrefs.SetString(userPreference.name, result);
                    val = result;
                }
                else if (userPreference is FloatVar)
                {
                    var result = (userPreference as FloatVar).Value;
                    PlayerPrefs.SetFloat(userPreference.name, result);
                    val = result.ToString();
                }
                else if (userPreference is StringVar)
                {
                    var result = (userPreference as StringVar).Value;
                    PlayerPrefs.SetString(userPreference.name, result);
                    val = result;
                }
                else
                {
                    Debug.LogError("Unacceptable ScriptableVar used: " + userPreference.name);
                }

                if (debug) Debug.Log($"Set Value to Player Pref: {userPreference.name} -> [{val}]", this);
            }

            PlayerPrefs.Save();
        }

        public void DeleteAllPreferences() => PlayerPrefs.DeleteAll();

        private bool StringToBool(string value)
        {
            if (value == "true")
                return true;
            else if (value == "false")
                return false;
            else
            {
                Debug.Log("A string is neither 'true' nor 'false', returning false");
                return false;
            }
        }

       // private string BoolToString(bool value) => value ? "true" : "false";
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ScriptableVarToPlayerPref))]
    public class PlayerPreferenceEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ScriptableVarToPlayerPref playerPreferenceManager = (ScriptableVarToPlayerPref)target;

            if (GUILayout.Button("Save all preferences."))
            {
                playerPreferenceManager.SaveUserPreferences();
            }

            if (GUILayout.Button("Delete all preferences."))
            {
                playerPreferenceManager.DeleteAllPreferences();
                if (playerPreferenceManager.debug) Debug.Log($"All Preferences Deleted", this);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}