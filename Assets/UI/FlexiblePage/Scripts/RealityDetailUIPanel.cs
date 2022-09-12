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
        _id.text = "Reality #" + reality.RealityId;
        _name.text = reality.name;
        _version.text = reality.Version;
        _lastUpdate.text = "2022. 09. 28";
        _author.text = reality.Author;
        _description.text = reality.Description;
    }

    private void Update()
    {

    }
}
