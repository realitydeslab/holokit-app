using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MOFATheTrainingIntroPanel : MonoBehaviour
{
    public void StartHost()
    {
        App.Instance.CreateReality(Reality.MOFATheTraining);
    }

    public void StartSpectator()
    {
        App.Instance.Reality = Reality.MOFATheTraining;
        SceneManager.LoadSceneAsync("MOFATheTrainingAR", LoadSceneMode.Single);
    }

    public void LoadRealitiesPage()
    {
        SceneManager.LoadSceneAsync("Realities", LoadSceneMode.Single);
    }
}
