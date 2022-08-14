using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PermissionsPanel : MonoBehaviour
{
    public void LoadRealitiesPage()
    {
        SceneManager.LoadSceneAsync("Realities", LoadSceneMode.Single);
    }
}
