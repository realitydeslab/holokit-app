using UnityEngine;
using UnityEngine.UI;
using Holoi.AssetFoundation;


[ExecuteInEditMode]
public class RealityDetailUIPanel : MonoBehaviour
{
    public Holoi.AssetFoundation.Reality reality;

    [SerializeField] TMPro.TMP_Text _id;
    [SerializeField] TMPro.TMP_Text _name;
    [SerializeField] TMPro.TMP_Text _version;
    [SerializeField] TMPro.TMP_Text _lastUpdate;
    [SerializeField] TMPro.TMP_Text _author;
    [SerializeField] TMPro.TMP_Text _description;
    [SerializeField] TMPro.TMP_Text _technic;
    [SerializeField] Transform _videoContainer;


    private void Awake()
    {

    }

    public void UpdateInformation()
    {
        _id.text = "Reality #" + reality.realityId;
        _name.text = reality.name;
        _version.text = reality.version;
        _lastUpdate.text = "2022. 09. 28";
        _author.text = reality.author;
        _description.text = reality.description;
    }
}
